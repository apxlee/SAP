using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class RequestTrackingView : System.Web.UI.UserControl
	{
		public string RequestId { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)
		{
			DataTable workflowBladeTestTable = GetWorkflowBlade();
			
			foreach (DataRow workflowRow in workflowBladeTestTable.Rows)
			{
				WorkflowBlade workflowBlade;
				workflowBlade = LoadControl(@"/Controls/WorkflowBlade.ascx") as WorkflowBlade;

				Label workflowActorName = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowActorName");
				workflowActorName.Text = workflowRow["workflow_actor_name"].ToString();
				
				Label workflowStatus = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowStatus");
				workflowStatus.Text = workflowRow["workflow_status"].ToString();
				
				Label workflowDueDate = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowDueDate");
				workflowDueDate.Text = workflowRow["workflow_due_date"].ToString();

				Label workflowCompletedDate = (Label)WebUtilities.FindControlRecursive(workflowBlade, "_workflowCompletedDate");
				workflowCompletedDate.Text = workflowRow["workflow_completed_date"].ToString();

				// if no comments then hide comments container
				// or loop thru comments and add in correct format
				//
				Panel workflowBladeCommentsContainer = (Panel)WebUtilities.FindControlRecursive(workflowBlade, "_workflowBladeCommentsContainer");
				workflowBladeCommentsContainer.Visible = false;
				
				this.Controls.Add(workflowBlade);			
			}
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

			table.Rows.Add("Actor One", "Status One", "Feb. 12, 2010", "Feb. 12, 2010");
			table.Rows.Add("Actor Two", "Status Two", "Feb. 13, 2010", "Feb. 12, 2010");
			table.Rows.Add("Actor Three", "Status Three", "Feb. 14, 2010", "Feb. 12, 2010");
			return table;
		}				
	}
}