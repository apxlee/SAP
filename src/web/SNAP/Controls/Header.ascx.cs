using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class Header : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			BuildRibbon("my_requests");

		}

		private PlaceHolder WrapControl(WebControl control, string element, string elementID)
		{
			PlaceHolder placeHolder = new PlaceHolder();

			Literal openElement = new Literal();
			openElement.Text = string.Format("<{0} id='{1}'>", element, elementID);

			Literal closeElement = new Literal();
			closeElement.Text = "</" + element + ">";

			placeHolder.Controls.Add(openElement);
			placeHolder.Controls.Add(control);
			placeHolder.Controls.Add(closeElement);

			return placeHolder;
		}		
		
		private void BuildRibbon(string CurrentPageName)
		{
			Panel ribbonPanel = new Panel();
			ribbonPanel.CssClass = CurrentPageName;
			
			List<string> linkButtons = new List<string>
			{
				"request_form", "my_requests", "my_approvals", "access_team", "search", "support"
			};

			Literal openElements = new Literal();
			openElements.Text = string.Format("<div id='{0}' class=''><ul id='{1}'>"
				, "csm_ribbon_container_outer"
				, "csm_ribbon_container_inner");

			Literal closeElements = new Literal();
			closeElements.Text = "</ul></div>";

			ribbonPanel.Controls.Add(openElements);

			foreach (string link_name in linkButtons)
			{
				LinkButton link = new LinkButton();
				link.CommandName = link_name + "_button";
				link.Command += new CommandEventHandler(RibbonLink_Click);
				link.Text = link.CommandName;
				ribbonPanel.Controls.Add(WrapControl(link, "li", link.CommandName));
			}

			ribbonPanel.Controls.Add(closeElements);			
			
			_ribbonContainer.Controls.Add(ribbonPanel);
		}
		
        protected void RibbonLink_Click(object sender, CommandEventArgs e)
        {
        }
	}
}