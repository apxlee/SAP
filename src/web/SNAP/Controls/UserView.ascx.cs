using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class UserView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
            {
                BuildUserView();
            }
		}
		
		private void BuildUserView()
		{
            SnapSession.CurrentUser.DistributionGroup = null;
            var requesterLoader = new Common.MyRequestLoader();
		    requesterLoader.Load();

			// TODO: refactor buildrequests... don't need to pass in all these parms
			//
            ViewBaseUtilities.BuildRequests(this.Page, RequestState.Open, _openRequestsContainer, _nullDataMessage_OpenRequests);
            ViewBaseUtilities.BuildRequests(this.Page, RequestState.Closed, _closedRequestsContainer, _nullDataMessage_ClosedRequests);
            
            //only use it once then clear requestId
            SnapSession.SelectedRequestId = "";
        }
		
	}

}