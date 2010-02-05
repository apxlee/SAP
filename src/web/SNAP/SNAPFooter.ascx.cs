using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web
{
    public partial class SNAPFooter : System.Web.UI.UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _footerLinksRepeater.DataSource = WebUtilities.FindControlRecursive(Page, "_tabMenuSource");

            // TODO: still having problems with cacheing of the added tabs.
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _footerLinkHeading.InnerText = WebUtilities.ApplicationTitle;
            _footerLinksRepeater.DataBind();
        }

        protected void _footerLinkItem_OnClick(object sender, CommandEventArgs e)
        {
            WebUtilities.SetActiveView(this.Page, e.CommandArgument.ToString());
        }
    }
}