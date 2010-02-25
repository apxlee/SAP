using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class Header : System.Web.UI.UserControl
	{
		public int PendingApprovals { get; set; }
		public int PendingAccessTeam { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)
		{
			// TODO: for testing
			PendingApprovals = 10;
			PendingAccessTeam = 20;
			
			if (!IsPostBack)
			{
				BuildRibbon("login");
			}
			else
			{
				BuildRibbon("");
			}
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
			//Panel ribbonPanel = new Panel();
			_ribbonContainerOuter.CssClass = CurrentPageName;
			
			// TODO: based on role, build list of linkbuttons
			// TODO: build from xml?  make List of objects (link buttons?) that have url pre-populated?
			//
			List<string> linkButtons = new List<string>
			{
				"request_form", "my_requests", "my_approvals", "access_team", "search", "support"
			};

			Literal openElements = new Literal();
			openElements.Text = string.Format("<div id='{0}' style='width:{1}px;'><ul id='{2}'>"
				, "csm_ribbon_container_outer", "763", "csm_ribbon_container_inner");

			Literal closeElements = new Literal();
			closeElements.Text = "</ul></div>";

			//ribbonPanel.Controls.Add(openElements);
			_ribbonContainerOuter.Controls.Add(openElements);

			foreach (string link_name in linkButtons)
			{
				LinkButton link = new LinkButton();
				link.CommandName = link_name + "_button";
				link.Command += new CommandEventHandler(RibbonLink_Click);
				
				switch (link_name)
				{
					case "my_approvals" :
						link.Text = "<span>" + PendingApprovals.ToString() + "</span>";
						link.CssClass = (PendingApprovals < 10) ? "snap_approvals_text_one_digit" : "snap_approvals_text_two_digits";
						break;
						
					case "access_team" :
						link.Text = "<span>" + PendingAccessTeam.ToString() + "</span>";
						link.CssClass = (PendingAccessTeam < 10) ? "snap_aim_text_one_digit" : "snap_aim_text_two_digits";
						break;
						
					default :
						link.Text = link.CommandName;
						break;
				}

				//ribbonPanel.Controls.Add(WrapControl(link, "li", link.CommandName));
				_ribbonContainerOuter.Controls.Add(WrapControl(link, "li", link.CommandName));
			}

			//ribbonPanel.Controls.Add(closeElements);
			_ribbonContainerOuter.Controls.Add(closeElements);		
			
			//_ribbonContainer.Controls.Add(ribbonPanel);
		}
		
        protected void RibbonLink_Click(object sender, CommandEventArgs e)
        {
			MultiView viewControl;
			viewControl = (MultiView)WebUtilities.FindControlRecursive(Page, "_masterMultiView");
			
			// TODO: make the view index an enum or object and eliminate need for silly switch
			//
			switch (e.CommandName)
			{
				case "request_form_button":
					viewControl.ActiveViewIndex = 1;
					_ribbonContainerOuter.CssClass = "request_form";
					break;

				case "my_requests_button":
					viewControl.ActiveViewIndex = 2;
					_ribbonContainerOuter.CssClass = "my_requests";
					break;

				case "my_approvals_button":
					viewControl.ActiveViewIndex = 3;
					_ribbonContainerOuter.CssClass = "my_approvals";
					break;

				case "access_team_button":
					viewControl.ActiveViewIndex = 4;
					_ribbonContainerOuter.CssClass = "access_team";
					break;

				case "search_button":
					viewControl.ActiveViewIndex = 5;
					_ribbonContainerOuter.CssClass = "search";
					break;

				case "support_button":
					viewControl.ActiveViewIndex = 6;
					_ribbonContainerOuter.CssClass = "support";
					break;
					
				default :
					viewControl.ActiveViewIndex = 0;
					_ribbonContainerOuter.CssClass = "";
					break;
			}
        }
	}
}