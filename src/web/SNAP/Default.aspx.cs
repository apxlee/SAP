using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Apollo.Ultimus.Web;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web
{
	public partial class Default : SnapPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			_password.Text = "Password1"; // TODO: REMOVE THIS AFTER DEV COMPLETE!!!

			//WebUtilities.SetRibbonContainerClass("login");
			//string blah = Page.GetType().Name;
		}

		private void DisplayMessage(string message, bool isError)
		{
			_loginMessage.Text = message;
			_loginMessageContainer.Visible = true;

			if (isError) _loginMessage.CssClass += " csm_error_text";
		}

		protected void _submitLogin_Click(object sender, EventArgs e)
		{
			if (!IsAuthenticatedUser(_networkId.Text, _password.Text))
			{
				DisplayMessage(@"Unable to Login.  Please re-enter your Network Login and Password. [Authentication Failure]", true);
			}
			else
			{
				Session.Clear();

				// TODO: make sure networkid is validated client-side so no nulls here
				string networkId = _networkId.Text.ToLower().Trim();
				string redirectUrl = "Default.aspx";

				SnapUser currentUser = new SnapUser(networkId);
				SnapSession.CurrentUser = currentUser;

				if (!string.IsNullOrEmpty(Request.QueryString.ToString()))
				{
					// TODO: strip out page name and append extra query string values to it
					redirectUrl += "?" + Request.QueryString.ToString();
				}
				else
				{
					if (_loginPathSelection.Value == "request_form" || _loginPathSelection.Value == "proxy_request")
					{
						//redirectUrl += "?" + QueryStringConstants.REQUESTED_VIEW_INDEX + "=" + (int)ViewIndex.request_form;
						redirectUrl = "MyRequests.aspx";
					}

					if (_loginPathSelection.Value == "request_form")
					{
						SnapSession.IsRequestPrePopulated = true;
					}
				}
				
				// TODO: logic to direct to page based on role

				WebUtilities.Redirect(redirectUrl, false);
			}
		}

		private bool IsAuthenticatedUser(string networkId, string password)
		{
#if DEBUG
			return true;
#endif
			try
			{
				LdapAuthentication authentication = new LdapAuthentication();

				if (authentication.IsAuthenticated(networkId.ToLower().Trim(), password.Trim()))
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				// TODO: Logger.Error("LoginView > AuthenticateUser", ex);
			}

			return false;
		}
	}
}
