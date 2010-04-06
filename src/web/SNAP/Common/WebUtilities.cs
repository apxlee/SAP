using System;
using System.Threading;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using OOSPARole = Apollo.AIM.SNAP.Web.Common.Role;
using Apollo.AIM.SNAP.CA;
using Apollo.AIM.SNAP.Model;
using Apollo.CA.Logging;

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
        
        public static Role CurrentRole
        {
			get 
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				try 
				{ 
					return (Role)currentPage.Session["OOSPAUserRole"]; 
				}
				catch 
				{
					return DetermineRole();
				}
			}

			set 
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				currentPage.Session["OOSPAUserRole"] = value;
			}
        }
        
        public static ViewIndex DefaultView
        {
			get
			{
				ViewIndex defaultViewIndex;
				
				switch (CurrentRole)
				{
					case OOSPARole.ApprovingManager:
						defaultViewIndex = ViewIndex.my_approvals;
						break;

					case OOSPARole.AccessTeam:
						defaultViewIndex = ViewIndex.access_team;
						break;

					case OOSPARole.SuperUser:
						defaultViewIndex = ViewIndex.my_requests;
						break;

					case OOSPARole.Requestor:
						defaultViewIndex = ViewIndex.my_requests;
						break;

					case OOSPARole.NotAuthenticated:
					default:
						defaultViewIndex = ViewIndex.login;
						break;
				}
				
				return defaultViewIndex;					
			}
        }

        private static Role DetermineRole()
        {
            ADUserDetail userDetail = CA.DirectoryServices.GetUserByLoginName(CurrentLoginUserId);
            using (var db = new SNAPDatabaseDataContext())
            {
                try
                {
                    // AD group is access team
                    if (userDetail.MemberOf.Contains("Access"))
                    {
                        // 1 - aim team. Is the user configured as AIM in the db?
                        var rolecheck = db.SNAP_Actors.Where(
                                a => a.actor_groupId == 1 && a.userId == CurrentLoginUserId && a.isActive == true);
						return rolecheck.Count() > 0 ? Role.SuperUser : Role.AccessTeam;
                    }
                    else
                    {
                        // Is the user configure as approval manager?
                        var rolecheck = db.SNAP_Actors.Where(
                                a => a.actor_groupId != 1 && a.userId == CurrentLoginUserId && a.isActive == true);
                        return rolecheck.Count() > 0 ? Role.ApprovingManager : Role.Requestor;
                    }
                }
                catch (Exception ex)
                {
                    #if DEBUG
                        return Role.NotAuthenticated;
                    #else
                        Logger.Error("WebUtilities - DetermineRole failed", ex);
                        return Role.NotAuthenticated;
                    #endif
                }
            }
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
		
        public static string CurrentLoginUserId
        {
            get
            {
                Page currentPage = HttpContext.Current.Handler as Page;
                // To-do: Should use CAP login user object here
                //var x = currentPage.Request.ServerVariables["AUTH_USER"].Split('\\')[1]; // remove domain name

                return "clschwim";
                //return x;
            }
         }
         
        public static void Redirect(string redirectUrl)
        {
			try
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				currentPage.Response.Redirect(redirectUrl, false);
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
