using System;
using System.Threading;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Apollo.CA.Logging;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Common
{
    public class WebUtilities
    {
		public static string ClientScriptPath
		{
			get { return VirtualPathUtility.ToAbsolute( ConfigurationManager.AppSettings["OOSPAScriptsPath"] ); }
		}
		
		public static string AppVersion
		{
			get { return ConfigurationManager.AppSettings["OOSPAVersion"]; }
		}
		
		public static string CurrentServer
		{
			get { return HttpContext.Current.Server.MachineName; }
		}
		
		public static string GetPageName(Page currentPage)
		{
			return currentPage.GetType().Name.StripUnderscoreAndExtension().ToLower();
		}
		
		public static bool IsSuperUser(string networkId)
		{
			string[] superUsers = ConfigurationManager.AppSettings["SNAPSuperUsers"].ToString().Split(',');
			foreach (string user in superUsers)
			{
				if (user == networkId) { return true; }
			}
			return false;
		}

        public static Control FindControlRecursive(Control controlRoot, string controlId)
        {
            if (controlId == string.Empty) { return null; }
            if (controlRoot.ID == controlId) { return controlRoot; }
            foreach (Control childControl in controlRoot.Controls)
            {
                Control target = FindControlRecursive(childControl, controlId);
                if (target != null) { return target; }
            }
            return null;
        }

		public static void SetRibbonContainerClass(string className)
		{
			Page currentPage = HttpContext.Current.Handler as Page;
			Panel ribbonContainer = (Panel)WebUtilities.FindControlRecursive(currentPage, "_ribbonContainerOuter");
			ribbonContainer.CssClass = className;
		}

		public static PlaceHolder WrapControl(WebControl innerControl, string outerElementName, string outerElementID)
		{
			PlaceHolder placeHolder = new PlaceHolder();

			Literal openElement = new Literal();
			openElement.Text = string.Format("<{0} id='{1}'>", outerElementName, outerElementID.ToLower());

			Literal closeElement = new Literal();
			closeElement.Text = "</" + outerElementName + ">";

			placeHolder.Controls.Add(openElement);
			placeHolder.Controls.Add(innerControl);
			placeHolder.Controls.Add(closeElement);

			return placeHolder;
		}		
		
		public static void RoleCheck(string pageName)
		{
			Role currentRole = SnapSession.CurrentUser.CurrentRole;
			Logger.Info(WebUtilities.GetTimestamp() + "WebUtilities > RoleCheck (85):" + currentRole.ToString() + "\r\n"); //TODO REMOVE
			bool isRedirect = true;

			if (currentRole == Role.SuperUser) {isRedirect = false;}
			else
			{
				if (pageName.ToLower() == PageNames.ACCESS_TEAM.ToLower())
				{
					isRedirect = (currentRole == Role.AccessTeam) ? false : true;
				}
				else if (pageName.ToLower() == PageNames.APPROVING_MANAGER.ToLower())
				{
					isRedirect = (currentRole == Role.ApprovingManager) ? false : true;
				}
			}

			Logger.Info(WebUtilities.GetTimestamp() + "WebUtilities > isRedirect (102):" + isRedirect + "\r\n"); //TODO REMOVE
			if (isRedirect) { WebUtilities.Redirect("AppError.aspx?errorReason=wrongRole", true); }
		}
         
        public static void Redirect(string redirectUrl, bool endResponse)
        {
			try
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				currentPage.Response.Redirect(redirectUrl, endResponse);
				//currentPage.Response.Redirect(redirectUrl, false);
			}
			catch (ThreadAbortException)
			{
				// No need to log this silly ThreadAbortException, can't get around this
			}
			
			//HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        
        public static void Redirect(string pageConstant)
        {
			Redirect(pageConstant + ".aspx", true);
        }

		public static string GetTimestamp()
		{
			return "[" + DateTime.Now.ToString("HH:mm:ss.ffff") + "] ";
		}
     }
        
    public static class ExtensionMethods
    {
		public static string StripTitleFromUserName(this String value)
		{
			try
			{
				return value.Remove( value.ToString().IndexOf("(") );
			}
			catch
			{
				return value;
			}
		}
		
		public static string StripUnderscore(this String value)
		{
			return value.Replace("_", " ");
		}
		
		public static string StripUrlDelimitersAndExtension(this String value)
		{
			try
			{
				string s = value.TrimStart('~', '/');
				return s.Remove(s.ToString().IndexOf("."));
			}
			catch
			{
				return value;
			}
		}
		
		public static string StripUnderscoreAndExtension(this String value)
		{
			try
			{
				return value.Remove( value.ToString().IndexOf("_") );
			}
			catch
			{
				return value;
			}
		}
    }
}
