using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.WebControls;

using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;
using MyRequest = Apollo.AIM.SNAP.Web.Common.Request;

// TODO: header links don't work on non-multiview pages

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class Header : System.Web.UI.UserControl
	{
		private bool _isStandalonePage = false;
		public bool IsStandalonePage {set { _isStandalonePage = value; }}
		
		public int PendingApprovals { get; set; }
		public int PendingAccessTeam { get; set; }
		
		private Role CurrentRole 
		{
			// NOTE: Header renders before Session created and CurrentUser instantiated
			get { return (SnapSession.IsUserCreated) ? SnapSession.CurrentUser.CurrentRole : Role.NotAuthorized; }
		}
		
		protected void Page_Load(object sender, EventArgs e)
		{
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
			// TODO: (Phase x) -- build from xml?  make List of objects (link buttons?) that have url pre-populated?
			//
			List<string> linkButtons = new List<string>{};
			string linksContainerWidth = "763";

			if (Convert.ToBoolean(ConfigurationManager.AppSettings["SNAPMaintenanceOn"].ToString()))
			{
				linkButtons.AddRange(new List<string> { "support" });
			}
			else
			{
				switch (this.CurrentRole)
				{
					case Role.ApprovingManager :
						linkButtons.AddRange(new List<string> { "request_form", "my_requests", "my_approvals", "search", "support" });
						linksContainerWidth = "600";
						PendingApprovals = MyRequest.ApprovalCount(SnapSession.CurrentUser.LoginId);
						break;
						
					case Role.AccessTeam :
						linkButtons.AddRange(new List<string> { "request_form", "my_requests", "access_team", "search", "support" });
						linksContainerWidth = "600";
						PendingAccessTeam = MyRequest.AccessTeamCount();
						break;
						
					case Role.SuperUser :
						linkButtons.AddRange(new List<string> { "request_form", "my_requests", "my_approvals", "access_team", "search", "support" });
						linksContainerWidth = "763";
						PendingApprovals = MyRequest.ApprovalCount(SnapSession.CurrentUser.LoginId);
						PendingAccessTeam = MyRequest.AccessTeamCount();
						break;

					case Role.Requestor :
						linkButtons.AddRange(new List<string> { "request_form", "my_requests", "search", "support" });
						linksContainerWidth = "430";
						break;
					
					case Role.NotAuthorized :
					default :
						linkButtons.AddRange(new List<string>{ "login", "support" });
						break;
				}
			}

			Literal openElements = new Literal();
			openElements.Text = string.Format("<div id='{0}' style='width:{1}px;'><ul id='{2}'>"
				, "csm_ribbon_container_outer", linksContainerWidth, "csm_ribbon_container_inner");
			_ribbonContainerOuter.Controls.Add(openElements);

			foreach (string link_name in linkButtons)
			{
				LinkButton link = new LinkButton();
				link.CommandName = link_name;
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
			}

			Literal closeElements = new Literal();
			closeElements.Text = "</ul></div>";
			_ribbonContainerOuter.Controls.Add(closeElements);		
		}
		
        protected void RibbonLink_Click(object sender, CommandEventArgs e)
        {
			if (!_isStandalonePage)
			{
				WebUtilities.SetActiveView( (int)Enum.Parse(typeof(ViewIndex), e.CommandName) );
				_ribbonContainerOuter.CssClass = e.CommandName;
			}
			else
			{
				WebUtilities.Redirect(string.Format("index.aspx?{0}={1}"
					, QueryStringConstants.REQUESTED_VIEW_INDEX
					, (int)Enum.Parse(typeof(ViewIndex), e.CommandName)), true);
			}
        }
	}
}