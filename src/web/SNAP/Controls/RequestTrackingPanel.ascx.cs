using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class RequestTrackingPanel : System.Web.UI.UserControl
	{
		public string RequestId { get; set; }
        public RequestState RequestState { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)
		{
			DataTable workflowBladeTable = GetWorkflowBlade();
			
			// TODO: if there are no blades (which would be odd), then build 'no data' message

			DataTable filteredTable = new DataTable();
			filteredTable.Columns.Add("workflow_actor_name", typeof(string));
			filteredTable.Columns.Add("workflow_status", typeof(string));
			filteredTable.Columns.Add("workflow_due_date", typeof(string));
			filteredTable.Columns.Add("workflow_completed_date", typeof(DateTime));
			filteredTable.Columns.Add("workflow_pkid", typeof(int));
			filteredTable.Columns.Add("actor_group_type", typeof(int));

			DataRow selectedRow;

			// Build Top AIM blades
			//
			try
			{
			selectedRow = (
				from bladeRow in workflowBladeTable.AsEnumerable()
				where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Workflow_Admin
				select bladeRow).Last();

			filteredTable.ImportRow(selectedRow);
			}
			catch {}
			
			// Build Manager blades
			//
			try
			{
			selectedRow = (
				from bladeRow in workflowBladeTable.AsEnumerable()
				where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Manager
				select bladeRow).Last();

			filteredTable.ImportRow(selectedRow);
			}
			catch {}
			
			// Build Team Approver blades
			//
			try
			{
				var distinctTeamApprovers = (
					from bladeRow in workflowBladeTable.AsEnumerable()
					where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Team_Approver
					select bladeRow["workflow_actor_name"]).Distinct();
					
				foreach(string teamApprover in distinctTeamApprovers)
				{
					selectedRow = (
						from bladeRow in workflowBladeTable.AsEnumerable()
						where (string)bladeRow["workflow_actor_name"] == teamApprover
						where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Team_Approver
						select bladeRow).Last();

					filteredTable.ImportRow(selectedRow);
				}
			}
			catch { }
			
			// Build Technical Approver blades
			//
			try
			{
				var distinctTechnicalApprovers = (
					from bladeRow in workflowBladeTable.AsEnumerable()
					where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Technical_Approver
					select bladeRow["workflow_actor_name"]).Distinct();

				foreach (string technicalApprover in distinctTechnicalApprovers)
				{
					selectedRow = (
						from bladeRow in workflowBladeTable.AsEnumerable()
						where (string)bladeRow["workflow_actor_name"] == technicalApprover
						where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Technical_Approver
						select bladeRow).Last();

					filteredTable.ImportRow(selectedRow);
				}
			}
			catch { }			
			
			
			//foreach (DataRow workflowRow in workflowBladeTable.Rows)
			foreach (DataRow workflowRow in filteredTable.Rows)
			{
				WorkflowBlade workflowBlade;
				workflowBlade = LoadControl("~/Controls/WorkflowBlade.ascx") as WorkflowBlade;

				Panel workflowBladeData;
				workflowBladeData = (Panel)WebUtilities.FindControlRecursive(workflowBlade, "_workflowBladeData");

				if ((int)workflowRow["actor_group_type"] == (int)ActorGroupType.Workflow_Admin)
				{
					workflowBladeData.CssClass = workflowBladeData.CssClass + " csm_alternating_bg";
				}

				#region Blade Labels
				Label workflowActorName = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowActorName");
				workflowActorName.Text = workflowRow["workflow_actor_name"].ToString() + " [" + workflowRow["actor_group_type"].ToString() +"]";
				
				Label workflowStatus = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowStatus");
				workflowStatus.Text = Convert.ToString((WorkflowState)Enum.Parse(typeof(WorkflowState), workflowRow["workflow_status"].ToString())).StripUnderscore();

				Label workflowDueDate = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowDueDate");
				workflowDueDate.Text = TestAndConvertDate(workflowRow["workflow_due_date"].ToString());
				
				Label workflowCompletedDate = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowCompletedDate");
				workflowCompletedDate.Text = TestAndConvertDate(workflowRow["workflow_completed_date"].ToString());
				#endregion

                BuildBladeComments(workflowBlade, (int)workflowRow["workflow_pkid"], workflowRow["workflow_actor_name"].ToString());
				
				this._workflowBladeContainer.Controls.Add(workflowBlade);
			}
		}
		
		private void BuildBladeComments(Control CurrentBlade, int WorkflowId, string actorName)
		{
			DataTable workflowCommentsTable = GetWorkflowComments(WorkflowId, actorName);
			
			if (workflowCommentsTable.Rows.Count > 0)
			{
				Panel workflowBladeCommentsContainer = (Panel)WebUtilities.FindControlRecursive(CurrentBlade, "_workflowBladeCommentsContainer");
				StringBuilder workflowComments = new StringBuilder();

				foreach (DataRow comment in workflowCommentsTable.Rows)
				{
					// TODO: move string to config file?
					workflowComments.AppendFormat("<p{0}><u>{1} by {2} on {3}</u><br />{4}</p>"
						, (bool)comment["is_new"] ? " class=csm_error_text" : string.Empty
						, Convert.ToString((CommentsType)Enum.Parse(typeof(CommentsType), comment["action"].ToString())).StripUnderscore()
						, comment["workflow_actor"].ToString()
						, Convert.ToDateTime(comment["comment_date"]).ToString("MMM d, yyyy")
						, comment["comment"].ToString());
				}

				Literal comments = new Literal();
				comments.Text = workflowComments.ToString();

				workflowBladeCommentsContainer.Controls.Add(comments);
				workflowBladeCommentsContainer.Visible = true;
			}
		}
		
		private string TestAndConvertDate(string date)
		{
			if (!string.IsNullOrEmpty(date.ToString()))
			{
				return Convert.ToDateTime(date).ToString("MMM d, yyyy");
			}
			else
			{
				return "-";
			}
		}
		
		DataTable GetWorkflowComments(int WorkflowId, string actorName)
		{
			// NOTE: dataset must be ordered from first to last
			//
			DataTable table = new DataTable();
			table.Columns.Add("action", typeof(string));
			table.Columns.Add("workflow_actor", typeof(string));
			table.Columns.Add("comment_date", typeof(string));
			table.Columns.Add("comment", typeof(string));
			table.Columns.Add("is_new", typeof(bool));


		    var wfComments = Common.Request.WfComments(RequestState);
            var details = wfComments.Where(x => x.workflowId == WorkflowId);

            foreach (usp_open_my_request_workflow_commentsResult list in details)
            {
                table.Rows.Add(list.commentTypeEnum
					, (actorName == "Access & Identity Management") ? "AIM" : actorName
					, list.createdDate
					, list.commentText
					, (list.commentTypeEnum == (int)CommentsType.Requested_Change) ? true : false);
            }

			return table;
		}

		DataTable GetWorkflowBlade()
		{
			DataTable table = new DataTable();
			table.Columns.Add("workflow_actor_name", typeof(string));
			table.Columns.Add("workflow_status", typeof(int));
			table.Columns.Add("workflow_due_date", typeof(string));
			table.Columns.Add("workflow_completed_date", typeof(DateTime));
			table.Columns.Add("workflow_pkid", typeof(int));
			table.Columns.Add("actor_group_type", typeof(int));

            var wfDetails = Common.Request.WfDetails(RequestState);
            var details = wfDetails.Where(x => x.requestId.ToString() == RequestId)
                          .OrderByDescending(o => o.pkId).Reverse();

            foreach (usp_open_my_request_workflow_detailsResult list in details)
            {
                table.Rows.Add(list.displayName, Convert.ToInt32(list.workflowStatusEnum), list.dueDate, list.completedDate, list.workflowId, list.actorGroupType);
                // why is workflowStatusEnum byte instead of int?
            }

			return table;
		}				
	}
}