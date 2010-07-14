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
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class RequestTrackingPanel : System.Web.UI.UserControl
	{
		public string RequestId { get; set; }
        public RequestState RequestState { get; set; }
        private DataTable _unfilteredTrackingData;
		
		protected void Page_Load(object sender, EventArgs e)
		{
			_unfilteredTrackingData = GetAllTrackingData();
			
			if (_unfilteredTrackingData.Rows.Count > 0)
			{
				BuildAIMTracking();
				BuildManagerTracking();
				BuildTeamApprovers();
				BuildTechnicalApprovers();
			}
			else
			{
				_nullDataMessage_NoWorkflows.Visible = true;
				Logger.Error("RequestTrackingPanel > Page_Load: No Tracking Data Exists.");
			}
		}
		
		#region Tracking Groups
		
		private void BuildAIMTracking()
		{
			try
			{
				DataTable filteredTrackingData = BuildEmptyTrackingBladeTable();
				DataRow selectedRow;

				selectedRow = (
					from bladeRow in _unfilteredTrackingData.AsEnumerable()
					where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Workflow_Admin
                    select bladeRow).Last();

				if ((int)selectedRow["workflow_status"] == (int)WorkflowState.Workflow_Created)
				{
					selectedRow.SetField("workflow_status", WorkflowState.In_Workflow);
					selectedRow.SetField("workflow_due_date", string.Empty);
				}
				else if ((int)selectedRow["workflow_status"] == (int)WorkflowState.Approved)
				{
					selectedRow.SetField("workflow_status", WorkflowState.Pending_Provisioning);
				}
				
				filteredTrackingData.ImportRow(selectedRow);
				
				RenderSectionHeading("Access & Identity Management");
				RenderWorkflowBlade(filteredTrackingData);
			}
            catch
            {
				// TODO: Linq returns exception if query is null.  Need to refactor?
            }
		}

		private void BuildManagerTracking()
		{
			try
			{
				DataTable filteredTrackingData = BuildEmptyTrackingBladeTable();
				DataRow selectedRow;

				selectedRow = (
					from bladeRow in _unfilteredTrackingData.AsEnumerable()
					where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Manager
					select bladeRow).Last();
				filteredTrackingData.ImportRow(selectedRow);

				RenderSectionHeading("Affected End User's Manager");
				RenderWorkflowBlade(filteredTrackingData);
			}
            catch
            {
                // TODO: Linq returns exception if query is null.  Need to refactor?
            }
		}

		private void BuildTeamApprovers()
		{
			try
			{
				DataRow selectedRow;

				var distinctTeamApprovers = (
					from bladeRow in _unfilteredTrackingData.AsEnumerable()
					where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Team_Approver
					select bladeRow["workflow_actor_name"]).Distinct();

				if (distinctTeamApprovers.Count() > 0)
				{
					RenderSectionHeading("Team Approvers");
				}					

				foreach (string teamApprover in distinctTeamApprovers)
				{
					selectedRow = (
						from bladeRow in _unfilteredTrackingData.AsEnumerable()
						where (string)bladeRow["workflow_actor_name"] == teamApprover
						where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Team_Approver
						select bladeRow).Last();

					DataTable filteredTrackingData = BuildEmptyTrackingBladeTable();
					filteredTrackingData.ImportRow(selectedRow);

					RenderWorkflowBlade(filteredTrackingData);
				}
			}
            catch
            {
				// TODO: Linq returns exception if query is null.  Need to refactor?
            }
		}

		private void BuildTechnicalApprovers()
		{
			try
			{
				DataRow selectedRow;

				var distinctTechnicalApprovers = (
					from bladeRow in _unfilteredTrackingData.AsEnumerable()
					where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Technical_Approver
					select bladeRow["workflow_actor_name"]).Distinct();

				if (distinctTechnicalApprovers.Count() > 0)
				{
					RenderSectionHeading("Technical Approvers");
				}

				foreach (string technicalApprover in distinctTechnicalApprovers)
				{
					selectedRow = (
						from bladeRow in _unfilteredTrackingData.AsEnumerable()
						where (string)bladeRow["workflow_actor_name"] == technicalApprover
						where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Technical_Approver
						select bladeRow).Last();

					DataTable filteredTrackingData = BuildEmptyTrackingBladeTable();
					filteredTrackingData.ImportRow(selectedRow);

					RenderWorkflowBlade(filteredTrackingData);
				}
			}
            catch
            {
				// TODO: Linq returns exception if query is null.  Need to refactor?
            }
		}
		
		#endregion

		private void RenderSectionHeading(string headingLabel)
		{
			WorkflowBladeSectionHeading sectionHeading;
			sectionHeading = LoadControl("~/Controls/WorkflowBladeSectionHeading.ascx") as WorkflowBladeSectionHeading;
			sectionHeading.HeadingLabel = headingLabel;
			this._workflowBladeContainer.Controls.Add(sectionHeading);
		}
				
		private void RenderWorkflowBlade(DataTable filteredTrackingData)
		{
			foreach (DataRow workflowRow in filteredTrackingData.Rows)
			{
				WorkflowBlade workflowBlade;
				workflowBlade = LoadControl("~/Controls/WorkflowBlade.ascx") as WorkflowBlade;

				if ((int)workflowRow["actor_group_type"] == (int)ActorGroupType.Workflow_Admin)
				{
					workflowBlade.AlternatingCss = " csm_alternating_bg";
				}

				#region Blade Labels
				workflowBlade.ActorName = workflowRow["workflow_actor_name"].ToString();
				workflowBlade.Status = Convert.ToString((WorkflowState)Enum.Parse(typeof(WorkflowState), workflowRow["workflow_status"].ToString())).StripUnderscore();
				workflowBlade.DueDate = TestAndConvertDate(workflowRow["workflow_due_date"].ToString());
				workflowBlade.CompletedDate = TestAndConvertDate(workflowRow["workflow_completed_date"].ToString());
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
						, (comment["action"].ToString() == CommentsType.Email_Reminder.ToString()) ? "AIM" : comment["workflow_actor"].ToString()
						, Convert.ToDateTime(comment["comment_date"]).ToString("MMM d, yyyy")
                        , comment["comment"].ToString().Replace("\n", "<br />"));
				}

				Literal comments = new Literal();
				comments.Text = workflowComments.ToString();

				workflowBladeCommentsContainer.Controls.Add(comments);
				workflowBladeCommentsContainer.Visible = true;
			}
		}
		
		private string TestAndConvertDate(string date)
		{
			if (!string.IsNullOrEmpty(date.ToString())) { return Convert.ToDateTime(date).ToString("MMM d, yyyy"); }
			else { return "-"; }
		}
		
		private DataTable GetWorkflowComments(int WorkflowId, string actorName)
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
					, (list.commentTypeEnum == (int)CommentsType.Requested_Change || list.commentTypeEnum == (int)CommentsType.Email_Reminder) ? true : false);
            }

			return table;
		}

		private DataTable GetAllTrackingData()
		{
			DataTable unfilteredTrackingData = BuildEmptyTrackingBladeTable();

            var wfDetails = Common.Request.WfDetails(RequestState);
            var details = wfDetails.Where(x => x.requestId.ToString() == RequestId)
                          .OrderByDescending(o => o.workflowStateId).Reverse();

            foreach (usp_open_my_request_workflow_detailsResult list in details)
            {
				unfilteredTrackingData.Rows.Add(list.displayName, Convert.ToInt32(list.workflowStatusEnum), list.dueDate, list.completedDate, list.workflowId, list.actorGroupType);
                // why is workflowStatusEnum byte instead of int?
            }

			return unfilteredTrackingData;
		}

		private DataTable BuildEmptyTrackingBladeTable()
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
	}
}