using System;
using System.Web.UI;

namespace Apollo.AIM.SNAP.Web
{
    
    public class CapPage: Page
    {
        private CapSession _cSession;
        
        protected override void OnInitComplete(System.EventArgs e)
        {
            base.OnInitComplete(e);
            _cSession = new CapSession(Session);
        }

        internal CapSession CurrentSession
        {
            get { return _cSession; }
            set { _cSession = value; }
        }

        // redirect to login page (index.aspx) if user has not login yet
        protected override void OnLoad(EventArgs e)
        {
            var returnPage = Request.Url.PathAndQuery.Substring(1); //remove the first '/'

            if (LoginValidator.CurrentLogin(Session) == null)
            {
                Response.Redirect("Index.aspx?" + QueryStringConstants.RefKey + "=" + returnPage, true);
            }

            // Be sure to call the base class's OnLoad method!
            base.OnLoad(e);
        }

    }
}
