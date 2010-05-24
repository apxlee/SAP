using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;
using MyRequest = Apollo.AIM.SNAP.Web.Common.Request;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class AccessTeamView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                BuildAccessTeamView();
                BuildAccessTeamFilter();
            }
		}
		
		private void BuildAccessTeamView()
		{
            SnapSession.CurrentUser.DistributionGroup = null;
            var requestLoader = new Common.AccessTeamRequestLoader();
            requestLoader.Load();

            ViewBaseUtilities.BuildRequests(this.Page, RequestState.Open, _openRequestsContainer, _nullDataMessage_OpenRequests);
            ViewBaseUtilities.BuildRequests(this.Page, RequestState.Closed, _closedRequestsContainer, _nullDataMessage_ClosedRequests);

            //only use it once then clear requestId
            SnapSession.SelectedRequestId = "";
        }

        private void BuildAccessTeamFilter()
        {
            //List<int> filterList = MyRequest.AccessFilterCount();
            //int total = filterList[0] + filterList[1] + filterList[2];
            _pendingacknowledgementFilter.Text = "Pending Acknowledgement"; //(" + filterList[0].ToString() + ")";
            _pendingworkflowFilter.Text = "Pending Workflow"; //(" + filterList[1].ToString() + ")";
            _inWorkflowFilter.Text = "In Workflow";
            _pendingprovisioningFilter.Text = "Pending Provisioning"; //(" + filterList[2].ToString() + ")";
            _showallFilter.Text = "Show All"; //(" + total.ToString() + ")";
        }

        protected void _submitForm_Click(object sender, EventArgs e)
        {
            WebUtilities.Redirect("AccessTeam.aspx", true);
        }
	}
}