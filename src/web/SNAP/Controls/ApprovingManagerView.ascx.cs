using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Apollo.AIM.SNAP.Web.Common;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class ApprovingManagerView : System.Web.UI.UserControl
	{
		//public string SelectedRequestId { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                PopulateSections();
            }
		}
		
		private void PopulateSections()
		{

            ViewBaseUtilities.SetGroupMembership();


            if (SnapSession.CurrentUser.DistributionGroup != null)
            {
                var groupRequestLoader = new Common.GroupApprovalLoader();
                groupRequestLoader.Load();
                ViewBaseUtilities.BuildApproverRequests(this.Page, RequestState.Open, _pendingApprovalsContainer);
                ViewBaseUtilities.BuildRequests(this.Page, RequestState.Closed, _closedRequestsContainer, _nullDataMessage_ClosedRequests);
            }
            else
            {
                var requestLoader = new Common.MyApprovalLoader();
                requestLoader.Load();
                ViewBaseUtilities.BuildApproverRequests(this.Page, RequestState.Open, _pendingApprovalsContainer);
                ViewBaseUtilities.BuildRequests(this.Page, RequestState.Closed, _closedRequestsContainer, _nullDataMessage_ClosedRequests);
           
            }
           
            //only use it once then clear requestId
            SnapSession.SelectedRequestId = "";
        } 
	}
}