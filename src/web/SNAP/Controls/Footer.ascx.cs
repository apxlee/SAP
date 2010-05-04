using System;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
    public partial class Footer : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

		protected void logout_Click(object sender, EventArgs e)
		{
			Session.Clear();
			WebUtilities.Redirect(PageNames.DEFAULT_LOGIN);
		}
    }
}