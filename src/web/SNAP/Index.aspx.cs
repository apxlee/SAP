using System;
using System.Configuration;
using System.Web.UI;

using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

// TODO: grab requests based on role + view and pass to next page?
// TODO: set pending counters (header) based on role + view + requests
// TODO: set roles on login page
// TODO: when login page is active, need to allow user to view support page
// TODO: if user follows link from email, must bounce to login page first (unless already authenticated)
// TODO: build footer links based on role
// TODO: make 404 page and/or error page and/or error panels on page?
// TODO: masterblade reads requestId from index and uses to make that request the expanded view
// TODO: clean up commented code and logic bombs in index/request form
// TODO: masterblade.js loads for every blade

namespace Apollo.AIM.SNAP.Web
{
	public partial class Index : SnapPage
	{
		//public string SelectedRequestId { get; set; }
		//public ViewIndex RequestedViewIndex { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)	
		{
			// NOTE: Allows user to view support page without logging in.
			//
			if (!SnapSession.IsUserCreated && (WebUtilities.CurrentViewIndex != ViewIndex.support))
			{
				if (string.IsNullOrEmpty(Request.QueryString[QueryStringConstants.REQUESTED_VIEW_INDEX]))
				{
					WebUtilities.SetActiveView((int)ViewIndex.login);
				}
				else 
				{
					if (Convert.ToInt32(Request.QueryString[QueryStringConstants.REQUESTED_VIEW_INDEX]) == (int)ViewIndex.support)
					{
						WebUtilities.SetActiveView((int)ViewIndex.support);
					}
				}
				
				return;
			} 
				
			if (!IsPostBack && SnapSession.IsUserCreated)
			{
				if (string.IsNullOrEmpty(Request.QueryString[QueryStringConstants.REQUEST_ID])
					&& string.IsNullOrEmpty(Request.QueryString[QueryStringConstants.REQUESTED_VIEW_INDEX]))
				{
					WebUtilities.SetActiveView((int)SnapSession.CurrentUser.DefaultView);
				}
				else
				{
					SetViewFromQueryString();
				}
			}
			
			if (IsPostBack && !SnapSession.IsUserCreated)
			{
				// this SHOULD never fire because postback requires logged in user
				// NOTE: fires when going from Support to Login for unauth'd user.
				bool IsThisStupid = true;
			}
		}
		
		private void SetViewFromQueryString()
		{
			// TODO: test to make sure this isn't null assignment?
			SnapSession.SelectedRequestId = Request.QueryString[QueryStringConstants.REQUEST_ID];
			ViewIndex requestedViewIndex = (ViewIndex)Enum.Parse(typeof(ViewIndex), Request.QueryString[QueryStringConstants.REQUESTED_VIEW_INDEX]);
			Role currentRole = SnapSession.CurrentUser.CurrentRole;

			switch (requestedViewIndex)
			{
				case ViewIndex.request_form:
				case ViewIndex.my_requests:
				case ViewIndex.search:
				case ViewIndex.support:
					WebUtilities.SetActiveView((int)requestedViewIndex);
					break;
			
				case ViewIndex.my_approvals:
					if (currentRole == Role.ApprovingManager || currentRole == Role.SuperUser)
					{
						WebUtilities.SetActiveView((int)ViewIndex.my_approvals);
					}
					else { goto default; }
					break;

				case ViewIndex.access_team:
					if (currentRole == Role.AccessTeam || currentRole == Role.SuperUser)
					{
						WebUtilities.SetActiveView((int)ViewIndex.access_team);
					}
					else { goto default; }
					break;

				default:
					WebUtilities.Redirect("AppError.aspx?errorReason=wrongRole", true);
					break;
			}
		}
	}
}
