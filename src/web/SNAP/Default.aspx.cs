using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.Ultimus.Web;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web
{
	public partial class Default : SnapPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			_password.Text = "Password1"; // TODO: REMOVE THIS AFTER DEV COMPLETE!!!
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
				SnapSession.ClearCurrentUser();
				SnapUser currentUser = new SnapUser(_networkId.Text.ToLower().Trim());  // TODO: make sure networkid is validated client-side so no nulls here
				SnapSession.CurrentUser = currentUser;

				string redirectConstant = SnapSession.CurrentUser.DefaultPage;

				if (!string.IsNullOrEmpty(SnapSession.RequestedPage))
				{
					redirectConstant = SnapSession.RequestedPage;
				}
				else
				{
					if (_loginPathSelection.Value == "request_form" || _loginPathSelection.Value == "proxy_request")
					{
						redirectConstant = PageNames.REQUEST_FORM;
					}

					if (_loginPathSelection.Value == "request_form")
					{
						SnapSession.IsRequestPrePopulated = true;
					}
				}

				WebUtilities.Redirect(redirectConstant);
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
