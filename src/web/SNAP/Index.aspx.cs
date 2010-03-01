using System;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Apollo.CA.Logging;
using Apollo.Ultimus.CAP;
using Apollo.Ultimus.CAP.Model;
using Apollo.Ultimus.Web;

// need to alias namespace because CAP had 'Role' class also
//
using WebUtilities = Apollo.AIM.SNAP.Web.Common.WebUtilities;
using OOSPARole = Apollo.AIM.SNAP.Web.Common.Role;
using SNAPControls = Apollo.AIM.SNAP.Web.Controls;

namespace Apollo.AIM.SNAP.Web
{
	public partial class Index : Page
	{
		protected void Page_Load(object sender, EventArgs e)	
		{
			// TODO: make sure user is in access team AD group and then allow role change
			//
			if (!string.IsNullOrEmpty(Request.QueryString["role"]))
			{
				WebUtilities.CurrentRole = (OOSPARole)Enum.Parse(typeof(OOSPARole), Request.QueryString["role"]);
			}
			else
			{
				// TODO: utility to find AD group and/or look into DB for existence of pending approvals to determine role
			}
			
			Panel ribbonContainer = (Panel)WebUtilities.FindControlRecursive(Page, "_ribbonContainerOuter");

			switch (WebUtilities.CurrentRole)
			{
				case OOSPARole.ApprovingManager:
					WebUtilities.SetActiveView("_approvingManagerView");
					ribbonContainer.CssClass = "my_approvals";
					// TODO: get from db
					_headerControl.PendingApprovals = 20;
					break;

				case OOSPARole.AccessTeam:
					WebUtilities.SetActiveView("_accessTeamView");
					ribbonContainer.CssClass = "access_team";
					// TODO: get from db
					_headerControl.PendingAccessTeam = 5;
					break;

				case OOSPARole.SuperUser:
					WebUtilities.SetActiveView("_userView");
					ribbonContainer.CssClass = "my_requests";
					// TODO: get from db
					_headerControl.PendingApprovals = 20;
					_headerControl.PendingAccessTeam = 5;
					break;

				case OOSPARole.Requestor:
					WebUtilities.SetActiveView("_userView");
					ribbonContainer.CssClass = "my_requests";
					break;

				case OOSPARole.NotAuthenticated:
				default :
					WebUtilities.SetActiveView("_loginView");
					ribbonContainer.CssClass = "login";
					break;
			}			
		}
	}
}
