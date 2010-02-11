using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class AccessTeamView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            LoadWorkFlowBuilder();
		}

        private void LoadWorkFlowBuilder()
        {
            WorkflowBuilderPanel workflowBuilderPanel;
            workflowBuilderPanel = LoadControl(@"/Controls/WorkflowBuilderPanel.ascx") as WorkflowBuilderPanel;

            this._workflowBuilderPanel.Controls.Add(workflowBuilderPanel);
        }
	}
}