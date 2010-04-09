using System;
using System.Configuration;
using System.Web.UI;

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
			base.OnLoad(e);
		}

		private void PerformChecks()
		{
			// TODO: (Phase 2) Allow selected users (superUsers) to work on app in maintenance mode
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
