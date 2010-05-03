﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.WebControls;

using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;
using MyRequest = Apollo.AIM.SNAP.Web.Common.Request;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class HeaderStandalone : System.Web.UI.UserControl
	{
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
			openElement.Text = string.Format("<{0} id='{1}'>", element, elementID.ToLower());

			Literal closeElement = new Literal();
			closeElement.Text = "</" + element + ">";

			placeHolder.Controls.Add(openElement);
			placeHolder.Controls.Add(control);
			placeHolder.Controls.Add(closeElement);

			return placeHolder;
		}

		private void BuildRibbon()
		{
			// NOTE: linkButtons List<> must match the case of the Css names (Css is case-sensitive with Xhtml doctypes)
			//
			List<string> linkButtons = new List<string> { };
			string linksContainerWidth = "763";

			if (Convert.ToBoolean(ConfigurationManager.AppSettings["SNAPMaintenanceOn"].ToString()))
			{
				linkButtons.AddRange(new List<string> { "Support" });
			}
			else
			{
				switch (this.CurrentRole)
				{
					case Role.ApprovingManager:
						linkButtons.AddRange(new List<string> { "RequestForm", "MyRequests", "MyApprovals", "Search", "Support" });
						linksContainerWidth = "600";
						PendingApprovals = MyRequest.ApprovalCount(SnapSession.CurrentUser.LoginId);
						break;

					case Role.AccessTeam:
						linkButtons.AddRange(new List<string> { "RequestForm", "MyRequests", "AccessTeam", "Search", "Support" });
						linksContainerWidth = "600";
						PendingAccessTeam = MyRequest.AccessTeamCount();
						break;

					case Role.SuperUser:
						linkButtons.AddRange(new List<string> { "RequestForm", "MyRequests", "MyApprovals", "AccessTeam", "Search", "Support" });
						linksContainerWidth = "763";
						PendingApprovals = MyRequest.ApprovalCount(SnapSession.CurrentUser.LoginId);
						PendingAccessTeam = MyRequest.AccessTeamCount();
						break;

					case Role.Requestor:
						linkButtons.AddRange(new List<string> { "RequestForm", "MyRequests", "Search", "Support" });
						linksContainerWidth = "430";
						break;

					case Role.NotAuthorized:
					default:
						linkButtons.AddRange(new List<string> { "Default", "Support" });
						break;
				}
			}

			// NOTE: Header links hierarchy is: DIV > UL > LI > A > SPAN (for 'pending' counts)
			//
			Literal openElements = new Literal();
			openElements.Text = string.Format("<div id='{0}' style='width:{1}px;'><ul id='{2}'>"
				, "csm_ribbon_container_outer"
				, linksContainerWidth
				, "csm_ribbon_container_inner");
			
			_ribbonContainerOuter.Controls.Add(openElements);

			foreach (string link_name in linkButtons)
			{
				// NOTE: LinkButton controls are wrapped in LI elements
				//
				LinkButton link = new LinkButton();
				link.CausesValidation = false;
				link.CommandName = link_name;
				link.Command += new CommandEventHandler(RibbonLink_Click);

				switch (link_name)
				{
					case "MyApprovals":
						link.Text = "<span>" + PendingApprovals.ToString() + "</span>";
						link.CssClass = (PendingApprovals < 10) ? "snap_approvals_text_one_digit" : "snap_approvals_text_two_digits";
						break;

					case "AccessTeam":
						link.Text = "<span>" + PendingAccessTeam.ToString() + "</span>";
						link.CssClass = (PendingAccessTeam < 10) ? "snap_aim_text_one_digit" : "snap_aim_text_two_digits";
						break;

					default:
						link.Text = link.CommandName;
						break;
				}

				// NOTE: Careful to add the LI & A wrapped controls before the loop ends
				//
				_ribbonContainerOuter.Controls.Add(WrapControl(link, "li", link.CommandName));
			}

			// NOTE: Now close the UL and DIV and add the whole blob to the PlaceHolder
			//
			Literal closeElements = new Literal();
			closeElements.Text = "</ul></div>";
			_ribbonContainerOuter.Controls.Add(closeElements);
		}

		protected void RibbonLink_Click(object sender, CommandEventArgs e)
		{
			WebUtilities.Redirect(e.CommandName + ".aspx", true);
		}
	}
}