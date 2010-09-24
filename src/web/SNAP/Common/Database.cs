using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
	}
}
