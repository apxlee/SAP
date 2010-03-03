﻿using System;
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

namespace Apollo.AIM.SNAP.Web.Common
{
    public class WebUtilities
    {
			//public static string ApplicationTitle
			//{
			//    get { return ConfigurationManager.AppSettings["CsmApplicationTitle"]; }
			//}

			//public static string UserControlPath
			//{
			//    get { return ConfigurationManager.AppSettings["CsmUserControlPath"]; }
			//}
            
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
						currentPage.Session["OOSPAUserRole"] = Role.NotAuthenticated;
						return Role.NotAuthenticated;
					}
				}

				set 
				{
					Page currentPage = HttpContext.Current.Handler as Page;
					currentPage.Session["OOSPAUserRole"] = value;
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

            public static void SetActiveView(string viewId)
            {
				Page currentPage = HttpContext.Current.Handler as Page;
				MultiView multiView = (MultiView)WebUtilities.FindControlRecursive(currentPage, "_masterMultiView");

                try
                {
                    View selectedView = (View)WebUtilities.FindControlRecursive(multiView, viewId);
                    multiView.SetActiveView(selectedView);
                }
                catch
                {
                    multiView.ActiveViewIndex = -1;
                }
            }

			public static void SetActiveView(int viewIndex)
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				MultiView multiView = (MultiView)WebUtilities.FindControlRecursive(currentPage, "_masterMultiView");

				try
				{
					multiView.ActiveViewIndex = viewIndex;
				}
				catch
				{
					multiView.ActiveViewIndex = -1;
				}
			}

            private static string ScriptsRoot = "~/scripts/";

            public static string ScriptUrl(string scriptFile)
            {
                return VirtualPathUtility.ToAbsolute(ScriptsRoot + scriptFile);
            }
        }
}
