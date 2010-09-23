using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;
//using MyRequest = Apollo.AIM.SNAP.Web.Common.Request;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class AccessTeamView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            //if (!IsPostBack)
            //{
            //    BuildAccessTeamView();
            //}
		}
		
        //private void BuildAccessTeamView()
        //{
        //    SnapSession.CurrentUser.DistributionGroup = null;
        //    var requestLoader = new Common.AccessTeamRequestLoader();
        //    requestLoader.Load();

        //    ViewBaseUtilities.BuildRequests(this.Page, RequestState.Open, _openRequestsContainer, _nullDataMessage_OpenRequests);
        //    ViewBaseUtilities.BuildRequests(this.Page, RequestState.Closed, _closedRequestsContainer, _nullDataMessage_ClosedRequests);

        //    //only use it once then clear requestId
        //    SnapSession.SelectedRequestId = "";
        //}

        protected void _submitForm_Click(object sender, EventArgs e)
        {
            WebUtilities.Redirect("AccessTeam.aspx", true);
        }
	}
}