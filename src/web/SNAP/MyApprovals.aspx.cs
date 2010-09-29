using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web
{
	public partial class MyApprovals : SnapPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WebUtilities.RoleCheck(WebUtilities.GetPageName(Page));

            //if (!Database.IsPendingApproval(SnapSession.SelectedRequestId, SnapSession.CurrentUser.LoginId))
            //{
            //    _badStatusRequestId.Text = SnapSession.SelectedRequestId;
            //    _statusChangedMessage.Visible = true;
            //    SnapSession.SelectedRequestId = string.Empty;
            //}
		}
	}
}
