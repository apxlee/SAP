using System;
using System.Threading;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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

		public static void SetActiveView(int viewIndex)
		{
			Page currentPage = HttpContext.Current.Handler as Page;
			MultiView multiView = (MultiView)WebUtilities.FindControlRecursive(currentPage, "_masterMultiView");
			Panel ribbonContainer = (Panel)WebUtilities.FindControlRecursive(currentPage, "_ribbonContainerOuter");

			try
			{
				multiView.ActiveViewIndex = viewIndex;
				ribbonContainer.CssClass = Convert.ToString((ViewIndex)Enum.Parse(typeof(ViewIndex), viewIndex.ToString()));
			}
			catch
			{
				multiView.ActiveViewIndex = -1;
			}
		}
		
		public static ViewIndex CurrentViewIndex
		{
			get 
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				MultiView multiView = (MultiView)WebUtilities.FindControlRecursive(currentPage, "_masterMultiView");
			
				return (ViewIndex)multiView.ActiveViewIndex;

				//MultiView requestedView;
				//Int32 requestedViewIndex;
				//requestedView = (MultiView)WebUtilities.FindControlRecursive(Page, "_masterMultiView");
				//requestedViewIndex = requestedView.ActiveViewIndex;				
			}
		}

		// TODO: refactor into SNAPUser
		public static string CurrentLoginUserId
		{ get; set; }
		//    get
		//    {
		//        Page currentPage = HttpContext.Current.Handler as Page;
		//        SnapSession currentSession = new SnapSession(currentPage.Session);
		//        SNAPUser currentUser;
		//        currentUser = currentSession.CurrentUser;
		//        if (currentUser != null)
		//        {
		//            return currentUser.LoginId;				
		//        }
		//        else
		//        {
		//            return null;
		//        }

		//        // To-do: Should use CAP login user object here
		//        //var x = currentPage.Request.ServerVariables["AUTH_USER"].Split('\\')[1]; // remove domain name

		//        //return "clschwim";
		//        //return x;
		//    }
		//}
         
        public static void Redirect(string redirectUrl, bool endResponse)
        {
			try
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				currentPage.Response.Redirect(redirectUrl, endResponse);
			}
			catch (ThreadAbortException)
			{
				// No need to log this silly ThreadAbortException, can't get around this
			}
			
			HttpContext.Current.ApplicationInstance.CompleteRequest();
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
    }
}
