using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Apollo.AIM.SNAP.Model;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Web.Common
{
	public class SnapPage : Page
	{
		protected override void OnInitComplete(System.EventArgs e)
		{
			base.OnInitComplete(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			PerformChecks();
			
			this.ErrorPage = string.Format("{0}.aspx?{1}=unhandled"
				, PageNames.APP_ERROR
				, QueryStringConstants.ERROR_REASON);
			
			string currentPageName = WebUtilities.GetPageName(Page);
			// TODO REMOVE Logger.Info(WebUtilities.GetTimestamp() + "SnapPage > currentPageName (27): " + currentPageName + "\r\n"); 

			if (Request.QueryString.Count > 0)
			{
				SnapSession.RequestedPage = currentPageName;
                if (!string.IsNullOrEmpty(Request.QueryString[QueryStringConstants.REQUEST_ID]))
                {
                    SnapSession.SelectedRequestId = Request.QueryString[QueryStringConstants.REQUEST_ID];
                    SnapSession.UseSelectedRequestId = true;
                }
                else { SnapSession.UseSelectedRequestId = false; }
			}
			
			if (!SnapSession.IsUserCreated)
			{
				if (currentPageName != PageNames.DEFAULT_LOGIN.ToLower() 
					&& currentPageName != PageNames.SUPPORT.ToLower()
					&& currentPageName != PageNames.APP_ERROR.ToLower() )
					{
						WebUtilities.Redirect(PageNames.DEFAULT_LOGIN);
						//TODO REMOVE Logger.Info(WebUtilities.GetTimestamp() + "SnapPage > redirecting to DEFAULT_LOGIN (43) from: " + currentPageName + "\r\n"); 
					}
			}

			if (SnapSession.IsUserCreated)
			{
				try
				{
					HtmlInputHidden hiddenUserId = (HtmlInputHidden)WebUtilities.FindControlRecursive(Page, "_hiddenCurrentUserId");
                    HtmlInputHidden hiddenUserFullName = (HtmlInputHidden)WebUtilities.FindControlRecursive(Page, "_hiddenCurrentUserFullName");
					HtmlInputHidden hiddenSelectedRequestId = (HtmlInputHidden)WebUtilities.FindControlRecursive(Page, "_hiddenSelectedRequestId");   
					hiddenUserId.Value = SnapSession.CurrentUser.LoginId;
                    hiddenUserFullName.Value = SnapSession.CurrentUser.FullName;

					if (SnapSession.UseSelectedRequestId) { 
                        hiddenSelectedRequestId.Value = SnapSession.SelectedRequestId;
                        SnapSession.UseSelectedRequestId = false;
                    }
				}
				catch (Exception ex) 
                {
                    Logger.Error("SnapPage - OnLoad, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                }
			}

			base.OnLoad(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			WebUtilities.SetRibbonContainerClass(Page.AppRelativeVirtualPath.StripUrlDelimitersAndExtension().ToLower());
	
			base.OnPreRender(e);
		}

		private void PerformChecks()
		{
			// TODO: (future iteration) Allow selected users (superUsers) to work on app in maintenance mode
			//
			if (Convert.ToBoolean(ConfigurationManager.AppSettings["SNAPMaintenanceOn"].ToString()))
			{
				Server.Transfer("AppMaintenance.aspx");
			}

			if (Request.Browser.EcmaScriptVersion.Major < 1)
			{
				Server.Transfer("AppBrowserMessage.html");
			}
		}				
	}
}


