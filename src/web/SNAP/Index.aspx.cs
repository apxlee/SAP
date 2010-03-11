using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.CA.Logging;
using Apollo.Ultimus.CAP;
using Apollo.Ultimus.CAP.Model;
using Apollo.Ultimus.Web;

// need to alias namespace because CAP had 'Role' class also
//
using OOSPARole = Apollo.AIM.SNAP.Web.Common.Role;
using SNAPControls = Apollo.AIM.SNAP.Web.Controls;
using Apollo.AIM.SNAP.Web.Common;

// TODO: grab requests based on role + view and pass to next page?
// TODO: set pending counters (header) based on role + view + requests
// TODO: set roles on login page
// TODO: when login page is active, need to allow user to view support page
// TODO: if user follows link from email, must bounce to login page first (unless already authenticated)
// TODO: build footer links based on role
// TODO: make 404 page and/or error page and/or error panels on page?
// TODO: masterblade reads requestId from index and uses to make that request the expanded view

namespace Apollo.AIM.SNAP.Web
{
	public partial class Index : Page
	{
		public string RequestId { get; set; }
		public ViewIndex RequestedViewIndex { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)	
		{
			// TODO: REMOVE THIS SECTION AFTER DEMO!  or allow based on special AD group?  
			//
			if (!string.IsNullOrEmpty(Request.QueryString["role"]))
			{
				WebUtilities.CurrentRole = (OOSPARole)Enum.Parse(typeof(OOSPARole), Request.QueryString["role"]);
			}
				
			if (!IsPostBack)
			{
				SetViewFromQueryString();
			}
			else
			{
				// If user isn't following email link (with requestId and viewIndex) then drop them based on their role
				//
				SetViewFromRole();
			}
		}
		
		private void SetViewFromRole()
		{
			switch (WebUtilities.CurrentRole)
			{
				case OOSPARole.ApprovingManager:
					WebUtilities.SetActiveView((int)ViewIndex.my_approvals);
					break;

				case OOSPARole.AccessTeam:
					WebUtilities.SetActiveView((int)ViewIndex.access_team);
					break;

				case OOSPARole.SuperUser:
					WebUtilities.SetActiveView((int)ViewIndex.my_requests);
					break;

				case OOSPARole.Requestor:
					WebUtilities.SetActiveView((int)ViewIndex.my_requests);
					break;

				case OOSPARole.NotAuthenticated:
				default:
					WebUtilities.SetActiveView((int)ViewIndex.support);
					break;
			}		
		}
		
		private void SetViewFromQueryString()
		{
			// requestId and viewIndex must BOTH be present in query string
			//
			if (string.IsNullOrEmpty(Request.QueryString["requestId"]) && string.IsNullOrEmpty(Request.QueryString["viewIndex"]))
			{
				SetViewFromRole();
			}
			else
			{
				RequestId = Request.QueryString["requestId"];
				RequestedViewIndex = (ViewIndex)Enum.Parse(typeof(ViewIndex), Request.QueryString["viewIndex"]);

				switch (RequestedViewIndex)
				{
					case ViewIndex.my_requests:
						WebUtilities.SetActiveView((int)ViewIndex.my_requests);
						break;

					case ViewIndex.my_approvals:
						if (WebUtilities.CurrentRole == OOSPARole.ApprovingManager || WebUtilities.CurrentRole == OOSPARole.SuperUser)
						{
							WebUtilities.SetActiveView((int)ViewIndex.my_approvals);
						}
						else { goto default; }
						break;

					case ViewIndex.access_team:
						if (WebUtilities.CurrentRole == OOSPARole.AccessTeam || WebUtilities.CurrentRole == OOSPARole.SuperUser)
						{
							WebUtilities.SetActiveView((int)ViewIndex.access_team);
						}
						else { goto default; }
						break;

					case ViewIndex.request_form:
						// TODO: request must be in "change_requested" state for this view
						// TODO: current role will be set at login page (header links don't work right now)
						WebUtilities.SetActiveView((int)ViewIndex.request_form);
						break;

					default:
						// TODO: make 404 and direct there?
						WebUtilities.SetActiveView((int)ViewIndex.support);
						break;
				}
			}
		}
	}
}
