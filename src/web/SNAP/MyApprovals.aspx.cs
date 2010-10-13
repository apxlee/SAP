using System;
using Apollo.AIM.SNAP.Web.Common;
using Apollo.AIM.SNAP.Model;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Web
{
	public partial class MyApprovals : SnapPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WebUtilities.RoleCheck(WebUtilities.GetPageName(Page));

			if (!string.IsNullOrEmpty(SnapSession.SelectedRequestId))
			{
				try
				{
					if (!Database.IsPendingApproval(SnapSession.SelectedRequestId, SnapSession.CurrentUser.LoginId))
					{
						_badStatusRequestId.Text = SnapSession.SelectedRequestId;
						_statusChangedMessage.Visible = true;
						SnapSession.ClearSelectedRequestId();
					}
				}
				catch (Exception ex)
				{
					Logger.Error("MyApprovals.aspx\r\nMessage: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
					WebUtilities.Redirect(string.Format("{0}.aspx?{1}=badRequestId", PageNames.APP_ERROR, QueryStringConstants.ERROR_REASON), true );
				}
			}
		}
	}
}
