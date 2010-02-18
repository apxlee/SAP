using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class RequestTrackingPanel : System.Web.UI.UserControl
	{
		public string RequestId { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)
		{
			DataTable workflowBladeTestTable = GetWorkflowBlade();
			
			// TODO: if there are no blades (which would be odd), then build 'no data' message
			
			foreach (DataRow workflowRow in workflowBladeTestTable.Rows)
			{
				WorkflowBlade workflowBlade;
				workflowBlade = LoadControl(@"/Controls/WorkflowBlade.ascx") as WorkflowBlade;

				// TODO: test for access team and append csm_alternating_bg
				//
				Panel workflowBladeData;
				workflowBladeData = (Panel)WebUtilities.FindControlRecursive(workflowBlade, "_workflowBladeData");
				workflowBladeData.CssClass = workflowBladeData.CssClass + " csm_alternating_bg";

				Label workflowActorName = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowActorName");
				workflowActorName.Text = workflowRow["workflow_actor_name"].ToString();
				
				Label workflowStatus = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowStatus");
				workflowStatus.Text = workflowRow["workflow_status"].ToString();
				
				Label workflowDueDate = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowDueDate");
				workflowDueDate.Text = workflowRow["workflow_due_date"].ToString();

				Label workflowCompletedDate = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowCompletedDate");
				workflowCompletedDate.Text = workflowRow["workflow_completed_date"].ToString();

				BuildBladeComments( workflowBlade, (int)workflowRow["workflow_pkid"] );
				
				this._workflowBladeContainer.Controls.Add(workflowBlade);
			}
		}
		
		private void BuildBladeComments(Control CurrentBlade, int WorkflowId)
		{
			// if no comments then hide comments container
			//
			DataTable workflowCommentsTable = GetWorkflowComments(WorkflowId);
			Panel workflowBladeCommentsContainer = (Panel)WebUtilities.FindControlRecursive(CurrentBlade, "_workflowBladeCommentsContainer");
			StringBuilder workflowComments = new StringBuilder();

			foreach (DataRow comment in workflowCommentsTable.Rows)
			{
				// TODO: move string to config file?
				workflowComments.AppendFormat("<p{0}><u>{1} by {2} on {3}</u><br />{4}</p>"
					, (bool)comment["is_new"] ? " class=csm_error_text" : string.Empty
					, comment["action"].ToString()
					, comment["workflow_actor"].ToString()
					, comment["comment_date"].ToString()
					, comment["comment"].ToString());
			}

			Literal comments = new Literal();
			comments.Text = workflowComments.ToString();

			workflowBladeCommentsContainer.Controls.Add(comments);
			workflowBladeCommentsContainer.Visible = true;
		}
		
		static DataTable GetWorkflowComments(int WorkflowId)
		{
			// NOTE: dataset must be ordered from first to last
			//
			DataTable table = new DataTable();
			table.Columns.Add("action", typeof(string));
			table.Columns.Add("workflow_actor", typeof(string));
			table.Columns.Add("comment_date", typeof(string));
			table.Columns.Add("comment", typeof(string));
			table.Columns.Add("is_new", typeof(bool));
			
			table.Rows.Add("Acknowledged", "AIM", "Jan. 24, 2010", "Due Date: Jan. 20, 2010", false);
			table.Rows.Add("Change Requested", "AIM", "Jan. 24, 2010", "The 'justification' section on the form was not completed.", false);
			table.Rows.Add("Acknowledged", "AIM", "Jan. 25, 2010", "Due Date: Jan. 25, 2010", false);
			table.Rows.Add("Change Requested", "AIM", "Jan. 26, 2010", "Please complete the form as requested.", true);
			
			return table;
		}

		static DataTable GetWorkflowBlade()
		{
			// TODO: where RequestId = this.RequestId
			//
			DataTable table = new DataTable();
			table.Columns.Add("workflow_actor_name", typeof(string));
			table.Columns.Add("workflow_status", typeof(string));
			table.Columns.Add("workflow_due_date", typeof(string));
			table.Columns.Add("workflow_completed_date", typeof(string));
			table.Columns.Add("workflow_pkid", typeof(int));

			table.Rows.Add("Actor One", "Status One", "Feb. 12, 2010", "Feb. 12, 2010", 1);
			table.Rows.Add("Actor Two", "Status Two", "Feb. 13, 2010", "Feb. 12, 2010", 2);
			table.Rows.Add("Actor Three", "Status Three", "Feb. 14, 2010", "Feb. 12, 2010", 3);
			return table;
		}				
	}
}