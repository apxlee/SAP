using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Apollo.AIM.SNAP.Model;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Web.Common
{
    public abstract class RequestLoader
    {
        public void Load()
        {
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {

                    var requests = loadData(db);
                    if (HttpContext.Current.Items.Contains(Common.Request.RequestKey))
                        HttpContext.Current.Items.Remove(Common.Request.RequestKey);

                    HttpContext.Current.Items.Add(Common.Request.RequestKey, requests);

                }
            }
            catch (Exception ex)
            {
                Logger.Error("SNAP - Index: loadMyRequests failed", ex);
            }

        }

        protected abstract Dictionary<string, object> loadData(SNAPDatabaseDataContext db);

    }

    public class MyOpenRequestLooder : RequestLoader
    {

        protected override Dictionary<string, object> loadData(SNAPDatabaseDataContext db)
        {
            return db.MyOpenRequests(WebUtilities.CurrentLoginUserId);
        }
    }


    public class MyOpenApprovalRequestLooder : RequestLoader
    {

        protected override Dictionary<string, object> loadData(SNAPDatabaseDataContext db)
        {
            return db.MyOpenApprovalRequests(WebUtilities.CurrentLoginUserId);
        }
    }

    public class AccessTeamRequestLooder : RequestLoader
    {

        protected override Dictionary<string, object> loadData(SNAPDatabaseDataContext db)
        {
            return db.AccessTeamRequests();
        }
    }


}
