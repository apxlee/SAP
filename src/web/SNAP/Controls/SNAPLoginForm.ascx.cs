using System;
using System.Security.Principal;
using System.Configuration;
using System.Web.UI.WebControls;
using Apollo.CA.Logging;
using Apollo.Ultimus.CAP;
using Apollo.Ultimus.CAP.Model;
using Apollo.Ultimus.Web;
using System.Threading;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class SNAPLoginForm : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			performChecks();

#if DEBUG
			if (!IsPostBack)
			{
				prepopulateForm();
			}
#endif
		}

		private void performChecks()
		{
			string msg = displayMessage();
			checkMaintenance(msg);
			checkBrowser();
		}

		private string displayMessage()
		{
			string msg = Request[QueryStringConstants.MessageKey];
			if (msg != null)
			{
				if (msg.Equals("MAINT"))
				{
					showError("The Computer Access Process site is currently unavailable as we perform maintenance on it.<br/>We appreciate your patience and apologize for any inconvenience. ");
				}
				else
				{
					showError(msg);
				}
			}

			return msg;
		}

		private void checkMaintenance(string message)
		{
			if (ConfigurationManager.AppSettings["MaintenanceOn"].ToString() == "true" &&
		(message == null || (message != null && message != "MAINT")))
			{
				Server.Transfer("maintenance.html");
			}
		}

		/// <summary>
		/// Checks browser for javascript capability
		/// </summary>
		private void checkBrowser()
		{
			//A browser with a major version greater than or
			//equal to 1 should have javascript capabilities
			if (Request.Browser.EcmaScriptVersion.Major < 1)
			{
				Server.Transfer("BrowserMessage.html");
			}
		}

		private void prepopulateForm()
		{
			txtLogin.Text = this.Page.User.Identity.Name.Replace("APOLLOGROUP\\", "");
			txtPassword.TextMode = TextBoxMode.SingleLine;
			txtPassword.Text = "Password1";
			showError(
			  @"This app was compiled in DEBUG mode, and as such,<br/>
autoinserts your loginid and the default password for your ease of use.<br/>
The password field is also made clearText to facilitate auto-assigment.<br/>
You may need to disable Anonymous Authentication on your web app to get your id to show.");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnLogin.Click += new EventHandler(this.btnLogin_Click);
		}
		#endregion

		private void SetupSession()
		{
			Session.Clear();
			//DbDataStore eafStore;
		}

		private void btnLogin_Click(object sender, EventArgs e)
		{
			LdapAuthentication adAuth = new LdapAuthentication();

			try
			{
				txtLogin.Text = txtLogin.Text.ToLower().Trim();
				if (!adAuth.IsAuthenticated(txtLogin.Text.Trim(), txtPassword.Text.Trim()))
				{
					showError("Authentication failed, check username and password.");
					return;
				}

				//string thing = adAuth.GetGroups();

				SetupSession();

				Person LoginPerson = PersonTools.GetPersonFromUserId(NeoDbManager.IdmReadonlyContext, txtLogin.Text);
				// This wasn't actually being used anywhere, so I commented it out. -Kevin
				// ApplicationList alist = LoginPerson.Access.Find(Apollo.Ultimus.CAP.Model.Application.AppRestrictNeoWhereClause(false));

				if (LoginPerson == null)
				{
					showError("Login Failed. Could not find user account in the Apollo Identity Database.");
					return;
				}

				LoginValidator.SetLoginPerson(Session, LoginPerson);

				if (!String.IsNullOrEmpty(Request[QueryStringConstants.RefKey]) && Request[QueryStringConstants.RefKey].ToLower().IndexOf("addmodifyuser.aspx") < 0)
				{
					Response.Redirect(Request[QueryStringConstants.RefKey], false);
				}
				else
				{
					Response.Redirect("Default.aspx", false);
				}
				Context.ApplicationInstance.CompleteRequest();

			}
			catch (ThreadAbortException)
			{
			}
			catch (Exception ex)
			{
#if DEBUG
				showError(string.Format("Error authenticating. {0}<br/><pre>{1}</pre>", ex.Message, ex.StackTrace));

				Logger.Error("UI.Index.aspx.btnLogin_Click()", ex);
#else
				showError("Error authenticating. " + ex.Message);
#endif
			}
		}

		private void showError(string msg)
		{
			ErrorMsg.InnerHtml = msg;
			ErrorMsg.Visible = true;
		}
	}
}