﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.ajax
{

    public partial class AjaxUI : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region WebMethods
        [WebMethod]
        public static List<UI.RequestBlade> GetRequests(ViewIndex view, UI.Search search)
        {
            return UI.GetRequestBlades(view, search);
        }

        [WebMethod]
        public static string GetAccessTeamFilter()
        {
            return UI.GetAccessTeamFilter();
        }

        [WebMethod]
        public static string GetDetails(int requestId)
        {
            return UI.GetDetails(requestId);
        }

        [WebMethod]
        public static string GetBuilder(int requestId)
        {
            return UI.GetBuilder(requestId);
        }

        [WebMethod]
        public static List<string> GetAllTrackingData(string requestId)
        {
            return UI.GetTrackingBlades(requestId);
        }
        #endregion  
    }
}
