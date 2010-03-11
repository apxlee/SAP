using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
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
			// TODO
			get { return "AWACPXS01"; }
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
					// TODO: reenable logger, doesn't work on localhost (jds)
					//
                    //Logger.Error("WebUtilities - DetermineRole failed", ex);
                    return Role.NotAuthenticated;
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
