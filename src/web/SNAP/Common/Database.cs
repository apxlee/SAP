using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Controls;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Web.Common
{
	public class Database
	{
		#region Header and Footer

		public static int AccessTeamCount()
		{
			using (var db = new SNAPDatabaseDataContext())
			{
				int result = db.SNAP_Requests
							 .Where(o => o.statusEnum == 0 || o.statusEnum == 1 || o.statusEnum == 2)
							 .Select(s => s.statusEnum)
							 .Count();

				return result;
			}
		}

		public static int ApprovalCount(string[] userIds)
		{
			using (var db = new SNAPDatabaseDataContext())
			{
				int result = (from sr in db.SNAP_Requests
							  join sw in db.SNAP_Workflows on sr.pkId equals sw.requestId
							  join sws in db.SNAP_Workflow_States on sw.pkId equals sws.workflowId
							  join sa in db.SNAP_Actors on sw.actorId equals sa.pkId
							  where userIds.Contains(sa.userId)
							  && sws.workflowStatusEnum == 7
							  && sws.completedDate == null
							  && sr.statusEnum == 2
							  select new { sr.pkId }).GroupBy(p => p.pkId).Count();

				return result;
			}
		}

		public static void SetGroupMembership()
		{
			try
			{
				using (var db = new SNAPDatabaseDataContext())
				{
					var result = (from sr in db.SNAP_Requests
								  join sw in db.SNAP_Workflows on sr.pkId equals sw.requestId
								  join sws in db.SNAP_Workflow_States on sw.pkId equals sws.workflowId
								  join sa in db.SNAP_Actors on sw.actorId equals sa.pkId
								  where sa.isActive == true
								  && sa.isGroup == true
								  && SnapSession.CurrentUser.MemberOf.Contains(sa.userId)
								  && sws.workflowStatusEnum == 7
								  && sws.completedDate == null
								  && sr.statusEnum == 2
								  select new { sa.userId });

					if (result.Count() > 0)
					{
						SnapSession.CurrentUser.DistributionGroup = result.First().userId;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("SetGroupMembership \r\nMessage: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
			}
		}

		#endregion

        #region Request Blades

        public static List<UI.RequestBlade> GetRequests(ViewIndex view, UI.Search search)
        {
            try
            {
                string userId = SnapSession.CurrentUser.LoginId;
                string condition = "";
                var requestList = new List<UI.RequestBlade>();
                DateTime closedDateLimit = new DateTime();
                closedDateLimit = DateTime.Now.AddDays(-30);
                string dateTime = closedDateLimit.Year + "," + closedDateLimit.Month + "," + closedDateLimit.Day;
                string closedString = string.Format("statusEnum=={0}&&lastModifiedDate >= DateTime({1})", (int)RequestState.Closed, dateTime); ;
                string openString = string.Format("statusEnum=={0}||statusEnum=={1}||statusEnum=={2}", (int)RequestState.Open, (int)RequestState.Pending, (int)RequestState.Change_Requested);
                string requestCondition = string.Format("({0})||({1})",openString,closedString);

                using (var db = new SNAPDatabaseDataContext())
                {
                    switch (view)
                    {
                        case ViewIndex.my_approvals:
                            string groupNames = "";
                            foreach(string group in SnapSession.CurrentUser.MemberOf)
                            {
                                if (group != ""){groupNames += "userId==\"" + group + "\"||";}
                            }
                            groupNames = groupNames.Substring(0, groupNames.Length - 2);
                            string actorCondition = string.Format("(userId==\"{0}\"||({1}&&isGroup==true))",userId,groupNames);
                            List<int> workflowIds = (from r in db.SNAP_Requests.Where(requestCondition)
                                                     join w in db.SNAP_Workflows on r.pkId equals w.requestId
                                                     join ws in db.SNAP_Workflow_States on w.pkId equals ws.workflowId
                                                     join a in db.SNAP_Actors.Where(actorCondition) on w.actorId equals a.pkId
                                                     group ws by new { ws.workflowId } into grp
                                                     select grp.Max(s => s.pkId)).ToList();
                            var approvals = from r in db.SNAP_Requests
                                            join w in db.SNAP_Workflows on r.pkId equals w.requestId
                                            join ws in db.SNAP_Workflow_States on w.pkId equals ws.workflowId
                                            where workflowIds.Contains(ws.pkId)
                                            orderby r.lastModifiedDate descending
                                            select new
                                            {
                                                DisplayName = r.userDisplayName,
                                                RequestStatus = r.statusEnum,
                                                LastModified = r.lastModifiedDate,
                                                RequestId = r.pkId,
                                                WorkflowStatus = ws.workflowStatusEnum
                                            };
                            if (approvals != null)
                            {
                                foreach (var request in approvals)
                                {
                                    UI.RequestBlade newRequest = new UI.RequestBlade();
                                    newRequest.DisplayName = request.DisplayName.StripTitleFromUserName();
                                    newRequest.RequestStatus = Convert.ToString((RequestState)Enum.Parse(typeof(RequestState), request.RequestStatus.ToString())).StripUnderscore();
                                    newRequest.WorkflowStatus = request.WorkflowStatus.ToString();
                                    newRequest.LastModified = WebUtilities.TestAndConvertDate(request.LastModified.ToString());
                                    newRequest.RequestId = request.RequestId.ToString();
                                    requestList.Add(newRequest);
                                }
                            }
                            break;
                        case ViewIndex.my_requests:
                            condition = string.Format("(userId==\"{0}\"||submittedBy==\"{0}\")", userId);
                            var requests = db.SNAP_Requests.
                                Where(condition + "&&(" + requestCondition + ")").
                                OrderByDescending(r => r.lastModifiedDate).ToList();

                            if (requests != null)
                            {
                                foreach (var request in requests)
                                {
                                    UI.RequestBlade my_requests = new UI.RequestBlade();
                                    my_requests.DisplayName = request.userDisplayName.StripTitleFromUserName();
                                    my_requests.RequestStatus = Convert.ToString((RequestState)Enum.Parse(typeof(RequestState), request.statusEnum.ToString())).StripUnderscore();
                                    my_requests.WorkflowStatus = String.Empty;
                                    my_requests.LastModified = WebUtilities.TestAndConvertDate(request.lastModifiedDate.ToString());
                                    my_requests.RequestId = request.pkId.ToString();
                                    requestList.Add(my_requests);
                                }
                            }
                            break;
                        case ViewIndex.access_team:
                            var accessRequests = db.SNAP_Requests.
                                Where(requestCondition).
                                OrderByDescending(r => r.lastModifiedDate).ToList();

                            if (accessRequests != null)
                            {
                                foreach (var request in accessRequests)
                                {
                                    UI.RequestBlade access_team = new UI.RequestBlade();
                                    access_team.DisplayName = request.userDisplayName.StripTitleFromUserName();
                                    access_team.RequestStatus = Convert.ToString((RequestState)Enum.Parse(typeof(RequestState), request.statusEnum.ToString())).StripUnderscore();
                                    access_team.WorkflowStatus = String.Empty;
                                    access_team.LastModified = WebUtilities.TestAndConvertDate(request.lastModifiedDate.ToString());
                                    access_team.RequestId = request.pkId.ToString();
                                    requestList.Add(access_team);
                                }
                            }
                        break;
                        case ViewIndex.search:
                            string primaryCondition = "";
                            string rangeCondition = "";
                            List<object> values = new List<object>();
                            int valCount = 0;
                            if (search.Primary != string.Empty) 
                            { 
                                primaryCondition = "(pkId.ToString()==@0 or userId==@0 or userDisplayName==@0 or submittedBy==@0)";
                                values.Add((object)search.Primary);
                                condition = primaryCondition;
                            }
                            if (search.RangeStart != string.Empty)
                            {
                                DateTime rangeStart = Convert.ToDateTime(search.RangeStart);
                                if (search.RangeEnd != string.Empty) 
                                {
                                    valCount = values.Count();
                                    DateTime rangeEnd = Convert.ToDateTime(search.RangeEnd);
                                    rangeCondition = "(createdDate>=@" + valCount.ToString() + " and createdDate<@" + (valCount + 1).ToString() + ")";
                                    values.Add((object)rangeStart);
                                    values.Add((object)rangeEnd.AddDays(1));
                                }
                                else 
                                {
                                    valCount = values.Count();
                                    rangeCondition = "(createdDate>=@" + valCount.ToString() + ")";
                                    values.Add((object)rangeStart);
                                }
                                if (condition != string.Empty) { condition += " and " + rangeCondition; }
                                else { condition = rangeCondition; }
                            }
                            
                            if (condition == string.Empty)
                            {
                                condition = "pkid>@0";
                                values.Add(0);
                            }
                            object[] objs = values.ToArray();
                            var searchResults = db.SNAP_Requests.
                                    Where(condition, objs).
                                    OrderByDescending(r => r.lastModifiedDate).ToList();
                            
                            if (searchResults != null)
                            {
                                foreach (var request in searchResults)
                                {
                                    if (RequestContainsContents(search.Contents,request.pkId))
                                    {
                                        UI.RequestBlade search_request = new UI.RequestBlade();
                                        search_request.DisplayName = request.userDisplayName.StripTitleFromUserName();
                                        search_request.RequestStatus = Convert.ToString((RequestState)Enum.Parse(typeof(RequestState), request.statusEnum.ToString())).StripUnderscore();
                                        search_request.WorkflowStatus = String.Empty;
                                        search_request.LastModified = WebUtilities.TestAndConvertDate(request.lastModifiedDate.ToString());
                                        search_request.RequestId = request.pkId.ToString();
                                        requestList.Add(search_request);
                                    }
                                }
                            }  
                        break;
                    }
                    return requestList;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetRequests \r\nMessage: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                return null;
            }
        }

        #endregion

        #region Request Form

        public static DataTable GetChangeComments(int requestId)
		{
			DataTable table = new DataTable();
			table.Columns.Add("ActorDisplayName", typeof(string));
			table.Columns.Add("Comment", typeof(string));
			table.Columns.Add("CommentDate", typeof(string));

			using (var db = new SNAPDatabaseDataContext())
			{
				var changeComments = (from sr in db.SNAP_Requests
									  join sw in db.SNAP_Workflows on sr.pkId equals sw.requestId
									  join swc in db.SNAP_Workflow_Comments on sw.pkId equals swc.workflowId
									  join sa in db.SNAP_Actors on sw.actorId equals sa.pkId
									  where sr.pkId == requestId
									  && swc.commentTypeEnum == 3
									  select new { sa.displayName, swc.commentText, swc.createdDate });
				
				foreach (var comment in changeComments)
				{
					table.Rows.Add(comment.displayName, comment.commentText, comment.createdDate);
				}
			}

			return table;
		}

		#endregion

		#region Tracking Blades

		public static DataTable BuildEmptyTrackingBladeTable()
		{
			DataTable table = new DataTable();
			table.Columns.Add("workflow_actor_name", typeof(string));
			table.Columns.Add("workflow_status", typeof(int));
			table.Columns.Add("workflow_due_date", typeof(string));
			table.Columns.Add("workflow_completed_date", typeof(DateTime));
			table.Columns.Add("workflow_pkid", typeof(int));
			table.Columns.Add("actor_group_type", typeof(int));

			return table;
		}

		public static DataTable BuildEmptyTrackingCommentsTable()
		{
			DataTable table = new DataTable();
			table.Columns.Add("comment_type", typeof(int));
			table.Columns.Add("workflow_actor", typeof(string));
			table.Columns.Add("comment_date", typeof(string));
			table.Columns.Add("comment", typeof(string));
			table.Columns.Add("is_alert", typeof(bool));
			table.Columns.Add("request_id", typeof(string));
			table.Columns.Add("submitted_by", typeof(string));
			table.Columns.Add("affected_end_user", typeof(string));
			table.Columns.Add("request_status_enum", typeof(int));

			return table;
		}

		public static DataTable GetAllTrackingData(string requestId)
		{
			DataTable table = BuildEmptyTrackingBladeTable();

			using (var db = new SNAPDatabaseDataContext())
			{
				var tracking = from wf in db.SNAP_Workflows
							   where wf.requestId == Convert.ToInt32(requestId)
							   join ws in db.SNAP_Workflow_States on wf.pkId equals ws.workflowId
							   join a in db.SNAP_Actors on wf.actorId equals a.pkId
							   join ag in db.SNAP_Actor_Groups on a.actor_groupId equals ag.pkId
							   orderby ws.workflowId ascending, ws.pkId descending
							   select new
							   {
								   display_name = a.displayName,
								   workflow_status = ws.workflowStatusEnum,
								   workflow_due_date = ws.dueDate,
								   workflow_completed_date = ws.completedDate,
								   workflow_pkid = ws.workflowId,
								   actor_group_type = ag.actorGroupType
							   };

				foreach (var trackingRow in tracking)
				{
					table.Rows.Add
						(
							trackingRow.display_name
							, trackingRow.workflow_status
							, trackingRow.workflow_due_date
							, trackingRow.workflow_completed_date
							, trackingRow.workflow_pkid
							, trackingRow.actor_group_type
						);
				}
			}

			return table;
			// datatable > rows > Non-Public members > list > Results View
		}

		public static DataTable GetWorkflowComments(int workflowId)
		{
			using (var db = new SNAPDatabaseDataContext())
			{
				var workflowComments = from w in db.SNAP_Workflows
									   join a in db.SNAP_Actors on w.actorId equals a.pkId
									   join wc in db.SNAP_Workflow_Comments on w.pkId equals wc.workflowId
									   join r in db.SNAP_Requests on w.requestId equals r.pkId  // NOTE: need submittedBy, AEU and statusEnum for Change_Requested status comments
									   where w.pkId == workflowId
									   orderby wc.pkId ascending // NOTE: dataset must be ordered from first to last
									   select new
									   {
										   commentTypeEnum = wc.commentTypeEnum,
										   actorName = a.displayName,
										   createdDate = wc.createdDate,
										   commentText = wc.commentText,
										   requestId = w.requestId,
										   submittedBy = r.submittedBy,
										   affectedEndUser = r.userId,
										   requestStatusEnum = r.statusEnum
									   };

				if (workflowComments != null)
				{
					DataTable table = BuildEmptyTrackingCommentsTable();
					foreach (var comment in workflowComments)
					{
						table.Rows.Add(
							comment.commentTypeEnum
							, (comment.actorName == "Access & Identity Management") ? "AIM" : comment.actorName // TODO: move this to trackingBlades.cs?
							, WebUtilities.TestAndConvertDate(comment.createdDate.ToString())
							, comment.commentText
							, (comment.commentTypeEnum == (int)CommentsType.Requested_Change || comment.commentTypeEnum == (int)CommentsType.Email_Reminder) ? true : false
							, comment.requestId
							, comment.submittedBy
							, comment.affectedEndUser
							, comment.requestStatusEnum
							);
					}
					return table;
				}

				return null;
			}
		}

		#endregion

		#region Generic Request Info

        public static bool IsPendingApproval(string requestId, string approvingManagerId)
        {
            using (var db = new SNAPDatabaseDataContext())
            {
				int[] requestStatusEnums = new int[] { (int)RequestState.Open, (int)RequestState.Pending };

				var workflowStatusEnum = (from r in db.SNAP_Requests
										   join w in db.SNAP_Workflows on r.pkId equals w.requestId
										   join ws in db.SNAP_Workflow_States on w.pkId equals ws.workflowId
										   join a in db.SNAP_Actors on w.actorId equals a.pkId
										   where r.pkId == Convert.ToInt32(requestId)
										   where a.userId == approvingManagerId
										   where requestStatusEnums.Contains(r.statusEnum)
										   orderby ws.pkId descending
										   select ws.workflowStatusEnum).ToList();

				if ((workflowStatusEnum.Count() > 0) 
					&& (Convert.ToInt32(workflowStatusEnum[0]) == (int)WorkflowState.Pending_Approval))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool RequestContainsContents(string contents, int requestId)
        {
            try
            {
                bool found = false;
                using (var db = new SNAPDatabaseDataContext())
                {
                    if (contents == string.Empty) { return true; }
                    else
                    {
                        List<int> latestIds = (from aut in db.SNAP_Access_User_Texts.Where("requestId==@0", requestId)
                                               join adf in db.SNAP_Access_Details_Forms.Where("isActive==@0", true)
                                               on aut.access_details_formId equals adf.pkId
                                               group aut by new { aut.access_details_formId } into grp
                                               select grp.Max(m => m.pkId)).ToList();
                        var formData = from aut in db.SNAP_Access_User_Texts
                                       where latestIds.Contains(aut.pkId)
                                       select aut.userText;
                                       
                        foreach (var field in formData)
                        {
                            if (field.Contains(contents)) { found = true; }
                        }
                        return found;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RequestContainsContents \r\nMessage: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                return false;
            }
            
        }

		#endregion
	}
}
