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

        public static List<string> GetRequests(string condition, string userId, ViewIndex view)
        {
            try
            {
                List<string> requestList = new List<string>();
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
                            var approvals = from r in (db.SNAP_Requests.Where(requestCondition))
                                           join w in db.SNAP_Workflows on r.pkId equals w.requestId
                                           join ws in (db.SNAP_Workflow_States.Where(condition)) on w.pkId equals ws.workflowId
                                           join a in (db.SNAP_Actors.Where(actorCondition)) on w.actorId equals a.pkId
                                           group r by new { r.pkId, r.userDisplayName, r.statusEnum, r.lastModifiedDate, ws.workflowStatusEnum } into grp
                                           orderby grp.Key.lastModifiedDate descending
                                           select new
                                           {
                                               DisplayName = grp.Key.userDisplayName,
                                               RequestStatus = grp.Key.statusEnum,
                                               LastModified = grp.Key.lastModifiedDate,
                                               RequestId = grp.Key.pkId,
                                               WorkflowStatus = grp.Key.workflowStatusEnum
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
                                    requestList.Add(newRequest.ToJSONString());
                                }
                            }
                            break;
                        case ViewIndex.my_requests:
                        case ViewIndex.access_team:
                            var requests = db.SNAP_Requests.
                                Where(condition + "&&(" + requestCondition + ")").
                                OrderByDescending(r => r.lastModifiedDate).ToList();

                            if (requests != null)
                            {
                                foreach (var request in requests)
                                {
                                    UI.RequestBlade newRequest = new UI.RequestBlade();
                                    newRequest.DisplayName = request.userDisplayName.StripTitleFromUserName();
                                    newRequest.RequestStatus = Convert.ToString((RequestState)Enum.Parse(typeof(RequestState), request.statusEnum.ToString())).StripUnderscore();
                                    newRequest.WorkflowStatus = String.Empty;
                                    newRequest.LastModified = WebUtilities.TestAndConvertDate(request.lastModifiedDate.ToString());
                                    newRequest.RequestId = request.pkId.ToString();
                                    requestList.Add(newRequest.ToJSONString());
                                }
                            }
                        break;
                        case ViewIndex.search:
                        var search = db.SNAP_Requests.Where(r => r.userId.Contains(condition) 
                            || r.userDisplayName.Contains(condition)
                            || r.pkId.ToString().Contains(condition)).OrderByDescending(l => l.lastModifiedDate);

                        if (search != null)
                        {
                            foreach (var request in search)
                            {
                                UI.RequestBlade newResult = new UI.RequestBlade();
                                newResult.DisplayName = request.userDisplayName.StripTitleFromUserName();
                                newResult.RequestStatus = Convert.ToString((RequestState)Enum.Parse(typeof(RequestState), request.statusEnum.ToString())).StripUnderscore();
                                newResult.WorkflowStatus = String.Empty;
                                newResult.LastModified = WebUtilities.TestAndConvertDate(request.lastModifiedDate.ToString());
                                newResult.RequestId = request.pkId.ToString();
                                requestList.Add(newResult.ToJSONString());
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
			table.Columns.Add("action", typeof(string));
			table.Columns.Add("workflow_actor", typeof(string));
			table.Columns.Add("comment_date", typeof(string));
			table.Columns.Add("comment", typeof(string));
			table.Columns.Add("is_new", typeof(bool));

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
									   where w.pkId == workflowId
									   orderby wc.pkId ascending // NOTE: dataset must be ordered from first to last
									   select new
									   {
										   commentTypeEnum = wc.commentTypeEnum,
										   actorName = a.displayName,
										   createdDate = wc.createdDate,
										   commentText = wc.commentText,
									   };

				if (workflowComments != null)
				{
					DataTable table = BuildEmptyTrackingCommentsTable();
					foreach (var comment in workflowComments)
					{
						table.Rows.Add(
							comment.commentTypeEnum.ToString()
							, (comment.actorName == "Access & Identity Management") ? "AIM" : comment.actorName
							, WebUtilities.TestAndConvertDate(comment.createdDate.ToString())
							, comment.commentText
							, (comment.commentTypeEnum == (int)CommentsType.Requested_Change || comment.commentTypeEnum == (int)CommentsType.Email_Reminder) ? true : false);
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

                var result = from r in db.SNAP_Requests
                             join w in db.SNAP_Workflows on r.pkId equals w.requestId
                             join ws in db.SNAP_Workflow_States on w.pkId equals ws.workflowId
                             join a in db.SNAP_Actors on w.actorId equals a.pkId
                             where r.pkId == Convert.ToInt32(requestId)
                             where a.userId == approvingManagerId
                             where ws.workflowStatusEnum == (int)WorkflowState.Pending_Approval
                             where requestStatusEnums.Contains(r.statusEnum)
                             select r;

                if (result.Count() > 0)
                {
                    return true;
                }
            }

            return false;
        }

		#endregion
	}
}
