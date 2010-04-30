using System;
using System.Configuration;
using System.Web.UI;

using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

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
                _currentUserDisplayName.Value = SnapSession.CurrentUser.FullName;
                _currentUserId.Value = SnapSession.CurrentUser.LoginId;

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
			
			if (IsPostBack && !string.IsNullOrEmpty(Request[QueryStringConstants.REQUESTED_VIEW_INDEX]))
			{
				SetViewFromQueryString();
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

            // PONG!!! TODO - ask jds????
            ViewIndex requestedViewIndex = ViewIndex.request_form;
            try
            {
                requestedViewIndex =(ViewIndex)Enum.Parse(typeof(ViewIndex), Request.QueryString[QueryStringConstants.REQUESTED_VIEW_INDEX]);
            }
            catch (Exception ex)
            {
                
            }

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
