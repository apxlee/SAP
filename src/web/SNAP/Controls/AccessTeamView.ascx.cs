using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class AccessTeamView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (WebUtilities.CurrentViewIndex == ViewIndex.access_team)
			{
				BuildAccessTeamView();
			}
		}
		
		private void BuildAccessTeamView()
		{
            var requestLoader = new Common.AccessTeamRequestLooder();
            requestLoader.Load();

            ViewBaseUtilities.BuildRequests(this.Page, RequestState.Open, _openRequestsContainer, _nullDataMessage_ClosedRequests, false);
            //ViewBaseUtilities.BuildRequests(this.Page, RequestState.Closed, _closedRequestsContainer, _nullDataMessage_ClosedRequests, true);
            ViewBaseUtilities.BuildRequests(this.Page, RequestState.Closed, _closedRequestsContainer, _nullDataMessage_ClosedRequests, false);
		}
	}
}