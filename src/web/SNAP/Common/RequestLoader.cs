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

                    Dictionary<string, object> openRequests = null;
                    Dictionary<string, object> closeRequests = null;

                    loadData(db, ref openRequests, ref closeRequests);
                    addToContext(Common.Request.OpenRequestKey, openRequests);
                    addToContext(Common.Request.CloseRequestKey, closeRequests);
                    
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SNAP - Index: loadMyRequests failed", ex);
            }

        }

        private void addToContext(string key, Dictionary<string, object> data)
        {
            if (HttpContext.Current.Items.Contains(key))
                HttpContext.Current.Items.Remove(key);

            HttpContext.Current.Items.Add(key, data);
            
        }

        protected abstract void loadData(SNAPDatabaseDataContext db, ref Dictionary<string, object> open, ref Dictionary<string, object> close);

    }

    public class MyRequestLooder : RequestLoader
    {

        protected override void loadData(SNAPDatabaseDataContext db, ref Dictionary<string, object> open, ref Dictionary<string, object> close)
        {
            db.GetAllRequests(SnapSession.CurrentUser.LoginId, "my");
            open = db.OpenRquests;
            close = db.CloseRquests;
        }
    }

    public class MyApprovalLoader : RequestLoader
    {

        protected override void loadData(SNAPDatabaseDataContext db, ref Dictionary<string, object> open, ref Dictionary<string, object> close)
        {
            db.GetAllRequests(SnapSession.CurrentUser.LoginId, "approval");
            open = db.OpenRquests;
            close = db.CloseRquests;
        }
    }

    public class AccessTeamRequestLooder : RequestLoader
    {

        protected override void loadData(SNAPDatabaseDataContext db, ref Dictionary<string, object> open, ref Dictionary<string, object> close)
        {
            db.GetAllRequests(SnapSession.CurrentUser.LoginId, "accessteam");
            open = db.OpenRquests;
            close = db.CloseRquests;
        }
    }

}
