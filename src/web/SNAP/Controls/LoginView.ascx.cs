using System;

using Apollo.Ultimus.Web;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class LoginView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e) 
		{
			// TODO: REMOVE THIS AFTER DEV COMPLETE!!!
			_password.Text = "Password1";
		}
		
//        private void DisplayMessage(string message, bool isError)
//        {
//            _loginMessage.Text = message;
//            _loginMessageContainer.Visible = true;
			
//            if (isError) _loginMessage.CssClass += " csm_error_text";
//        }
		
//        protected void _submitLogin_Click(object sender, EventArgs e)
//        {
//            if (!IsAuthenticatedUser(_networkId.Text, _password.Text))
//            {
//                DisplayMessage(@"Unable to Login.  Please re-enter your Network Login and Password. [Authentication Failure]", true);
//            }
//            else
//            {
//                Session.Clear();
				
//                // TODO: make sure networkid is validated client-side so no nulls here
//                string networkId = _networkId.Text.ToLower().Trim();

//                SnapUser currentUser = new SnapUser(networkId);
//                SnapSession.CurrentUser = currentUser;
				
//                string redirectUrl = "index.aspx";
				
//                if (!string.IsNullOrEmpty(Request.QueryString.ToString()))
//                {
//                    redirectUrl += "?" + Request.QueryString.ToString();
//                }
//                else
//                {
//                    if (_loginPathSelection.Value == "request_form" || _loginPathSelection.Value == "proxy_request")
//                    {
//                        redirectUrl += "?" + QueryStringConstants.REQUESTED_VIEW_INDEX + "=" + (int)ViewIndex.request_form;
//                    }

//                    if (_loginPathSelection.Value == "request_form") 
//                    {	
//                        SnapSession.IsRequestPrePopulated = true;
//                    }
//                }
				
//                WebUtilities.Redirect(redirectUrl , false);
//            }
//        }
        
//        private bool IsAuthenticatedUser(string networkId, string password)
//        {
//#if DEBUG
//    return true;
//#endif			
//            try
//            {
//                LdapAuthentication authentication = new LdapAuthentication();
				
//                if (authentication.IsAuthenticated(networkId.ToLower().Trim(), password.Trim()))
//                {
//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//                // TODO: Logger.Error("LoginView > AuthenticateUser", ex);
//            }
			
//            return false;
//        }
	}
}