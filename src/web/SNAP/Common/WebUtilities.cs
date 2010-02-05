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

namespace Apollo.AIM.SNAP.Web.Common
{
    public class WebUtilities
    {
            public static string ApplicationTitle
            {
                get { return ConfigurationManager.AppSettings["CsmApplicationTitle"]; }
            }

            public static string UserControlPath
            {
                get { return ConfigurationManager.AppSettings["CsmUserControlPath"]; }
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

            public static void SetActiveView(Page currentPage, string viewId)
            {
                MultiView multiView = (MultiView)WebUtilities.FindControlRecursive(currentPage, "_csmMultiView");
                Menu tabMenu = (Menu)WebUtilities.FindControlRecursive(currentPage, "_tabbedMenu");

                try { tabMenu.FindItem(viewId).Selected = true; }
                catch { tabMenu.Items[0].Selected = true; }

                try
                {
                    View selectedView = (View)WebUtilities.FindControlRecursive(multiView, viewId);
                    multiView.SetActiveView(selectedView);
                }
                catch
                {
                    Panel contentContainer = (Panel)WebUtilities.FindControlRecursive(currentPage, "_contentContainer");
                    UserControl pageNotFound = currentPage.LoadControl(WebUtilities.UserControlPath + "Csm404.ascx") as UserControl;
                    contentContainer.Controls.Add(pageNotFound);

                    multiView.ActiveViewIndex = -1;
                }
            }
        }
}
