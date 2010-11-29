using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Web.Common;
using System.Configuration;

namespace Apollo.AIM.SNAP.Web.ajax
{
    public partial class AjaxUtilities : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        [WebMethod]
        public static List<Utilities.ReportListItem> GetReportListItems()
        {
            string SitePath = ConfigurationManager.AppSettings["sharepoint.sitepath"];
            string Domain = ConfigurationManager.AppSettings["sharepoint.domain"];
            string UserName = ConfigurationManager.AppSettings["sharepoint.username"];
            string Password = ConfigurationManager.AppSettings["sharepoint.password"];
            return Utilities.GetBottleneckApproversListItems(SitePath, UserName, Password, Domain);
        }
    }
}
