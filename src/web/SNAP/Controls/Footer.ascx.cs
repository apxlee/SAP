using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.WebControls;

using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
    public partial class Footer : System.Web.UI.UserControl
    {
		private Role CurrentRole
		{
			// NOTE: Footer renders before Session created and CurrentUser instantiated
			get { return (SnapSession.IsUserCreated) ? SnapSession.CurrentUser.CurrentRole : Role.NotAuthorized; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			BuildFooterLinks();
		}

		private void BuildFooterLinks()
		{
			List<string> linkButtons = new List<string> { };

			if (Convert.ToBoolean(ConfigurationManager.AppSettings["SNAPMaintenanceOn"].ToString()))
			{
				linkButtons.AddRange(new List<string> { "Support", "Reports" });
			}
			else
			{
				switch (this.CurrentRole)
				{
					case Role.ApprovingManager:
                        linkButtons.AddRange(new List<string> { "Request Form", "My Requests", "My Approvals", "Search", "Support", "Reports" });
						break;

					case Role.AccessTeam:
                        linkButtons.AddRange(new List<string> { "Request Form", "My Requests", "Access Team", "Search", "Support", "Reports" });
						break;

					case Role.SuperUser:
                        linkButtons.AddRange(new List<string> { "Request Form", "My Requests", "My Approvals", "AccessTeam", "Search", "Support", "Reports" });
						break;

					case Role.Requestor:
                        linkButtons.AddRange(new List<string> { "Request Form", "My Requests", "Search", "Support", "Reports" });
						break;

					case Role.NotAuthorized:
					default:
                        linkButtons.AddRange(new List<string> { "Support", "Reports" });
						break;
				}
			}

			Literal openElements = new Literal();
			openElements.Text = "<ul>";

			_applicationLinksContainer.Controls.Add(openElements);

			foreach (string link_name in linkButtons)
			{
				LinkButton link = new LinkButton();
				link.CausesValidation = false;
				link.CommandName = link_name.Replace(" ", "");
				link.Command += new CommandEventHandler(FooterLink_Click);
				link.Text = link_name;

				_applicationLinksContainer.Controls.Add(WebUtilities.WrapControl(link, "li", string.Empty));
			}

			Literal closeElements = new Literal();
			closeElements.Text = "</ul>";
			_applicationLinksContainer.Controls.Add(closeElements);
		}

		protected void FooterLink_Click(object sender, CommandEventArgs e)
		{
			WebUtilities.Redirect(e.CommandName + ".aspx", true);
		}
	}
}