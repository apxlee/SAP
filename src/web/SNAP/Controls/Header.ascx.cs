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
			//PendingApprovals = 10;
			//PendingAccessTeam = 20;
			
			BuildRibbon();
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
		
		private void BuildRibbon()
		{
			//_ribbonContainerOuter.CssClass = CurrentPageName;
			
			// TODO: build from xml?  make List of objects (link buttons?) that have url pre-populated?
			//
			
			List<string> linkButtons = new List<string>{};
			string linksContainerWidth = "763";
			
			switch ( WebUtilities.CurrentRole )
			{
				case Role.ApprovingManager :
					linkButtons.AddRange(new List<string> { "request_form", "my_requests", "my_approvals", "search", "support" });
					linksContainerWidth = "763";
					break;
					
				case Role.AccessTeam :
					linkButtons.AddRange(new List<string> { "request_form", "my_requests", "access_team", "search", "support" });
					linksContainerWidth = "763";
					break;
					
				case Role.SuperUser :
					linkButtons.AddRange(new List<string> { "request_form", "my_requests", "my_approvals", "access_team", "search", "support" });
					linksContainerWidth = "763";
					break;

				case Role.Requestor :
					linkButtons.AddRange(new List<string> { "request_form", "my_requests", "search", "support" });
					linksContainerWidth = "763";
					break;
				
				case Role.NotAuthenticated :
				default :
					linkButtons.AddRange(new List<string>{ "login" });
					break;
			}

			Literal openElements = new Literal();
			openElements.Text = string.Format("<div id='{0}' style='width:{1}px;'><ul id='{2}'>"
				, "csm_ribbon_container_outer", linksContainerWidth, "csm_ribbon_container_inner");
			_ribbonContainerOuter.Controls.Add(openElements);

			foreach (string link_name in linkButtons)
			{
				LinkButton link = new LinkButton();
				link.CommandName = link_name; // + "_button";
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

				_ribbonContainerOuter.Controls.Add(WrapControl(link, "li", link.CommandName));
				//_ribbonContainerOuter.CssClass = link_name;
			}

			Literal closeElements = new Literal();
			closeElements.Text = "</ul></div>";
			_ribbonContainerOuter.Controls.Add(closeElements);		
		}
		
        protected void RibbonLink_Click(object sender, CommandEventArgs e)
        {
			//int viewIndex = (int)Enum.Parse(typeof(ViewIndex), e.CommandName);
			WebUtilities.SetActiveView( (int)Enum.Parse(typeof(ViewIndex), e.CommandName) );
			_ribbonContainerOuter.CssClass = e.CommandName;
        
			//MultiView viewControl;
			//viewControl = (MultiView)WebUtilities.FindControlRecursive(Page, "_masterMultiView");
			
			// TODO: make the view index an enum or object and eliminate need for silly switch
			// or use setactiveview method in webutils
			//
			//switch (e.CommandName)
			//{
			//    case "request_form":
			//        WebUtilities.SetActiveView(1);
			//        //viewControl.ActiveViewIndex = 1;
			//        _ribbonContainerOuter.CssClass = "request_form";
			//        break;

			//    case "my_requests":
			//        viewControl.ActiveViewIndex = 2;
			//        _ribbonContainerOuter.CssClass = "my_requests";
			//        break;

			//    case "my_approvals":
			//        viewControl.ActiveViewIndex = 3;
			//        _ribbonContainerOuter.CssClass = "my_approvals";
			//        break;

			//    case "access_team":
			//        viewControl.ActiveViewIndex = 4;
			//        _ribbonContainerOuter.CssClass = "access_team";
			//        break;

			//    case "search":
			//        viewControl.ActiveViewIndex = 5;
			//        _ribbonContainerOuter.CssClass = "search";
			//        break;

			//    case "support":
			//        viewControl.ActiveViewIndex = 6;
			//        _ribbonContainerOuter.CssClass = "support";
			//        break;
					
			//    default :
			//        viewControl.ActiveViewIndex = 0;
			//        _ribbonContainerOuter.CssClass = "";
			//        break;
			//}
        }
	}
}