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
	}
}
