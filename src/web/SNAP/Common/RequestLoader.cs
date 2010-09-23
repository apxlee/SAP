using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Apollo.AIM.SNAP.Model;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Web.Common
{
	//public abstract class RequestLoader
	//{
	//    public string searchText { get; set; }

	//    public void Load()
	//    {
	//        try
	//        {
	//            using (var db = new SNAPDatabaseDataContext())
	//            {

	//                Dictionary<string, object> openRequests = null;
	//                Dictionary<string, object> closeRequests = null;
	//                Dictionary<string, object> searchRequests = null;

	//                loadData(db, ref openRequests, ref closeRequests, ref searchRequests);
	//                addToContext(Common.Request.OpenRequestKey, openRequests);
	//                addToContext(Common.Request.CloseRequestKey, closeRequests);
	//                addToContext(Common.Request.SearchRequestKey, searchRequests);
                    
	//            }
	//        }
	//        catch (Exception ex)
	//        {
	//            Logger.Error("RequestLoader - Load, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
	//        }

	//    }

	//    private void addToContext(string key, Dictionary<string, object> data)
	//    {
	//        if (HttpContext.Current.Items.Contains(key))
	//            HttpContext.Current.Items.Remove(key);

	//        HttpContext.Current.Items.Add(key, data);
            
	//    }

	//    protected abstract void loadData(SNAPDatabaseDataContext db, ref Dictionary<string, object> open, ref Dictionary<string, object> close, ref Dictionary<string, object> search);

	//}

	//public class MyRequestLoader : RequestLoader
	//{

	//    protected override void loadData(SNAPDatabaseDataContext db, ref Dictionary<string, object> open, ref Dictionary<string, object> close, ref Dictionary<string, object> search)
	//    {
	//        db.GetAllRequests(SnapSession.CurrentUser.LoginId, "my");
	//        open = db.OpenRquests;
	//        close = db.CloseRquests;
	//    }
	//}

	//public class MyApprovalLoader : RequestLoader
	//{

	//    protected override void loadData(SNAPDatabaseDataContext db, ref Dictionary<string, object> open, ref Dictionary<string, object> close, ref Dictionary<string, object> search)
	//    {
	//        db.GetAllRequests(SnapSession.CurrentUser.LoginId, "approval");
	//        open = db.OpenRquests;
	//        close = db.CloseRquests;
	//    }
	//}

	//public class GroupApprovalLoader : RequestLoader
	//{

	//    protected override void loadData(SNAPDatabaseDataContext db, ref Dictionary<string, object> open, ref Dictionary<string, object> close, ref Dictionary<string, object> search)
	//    {
	//        db.GetAllRequests(SnapSession.CurrentUser.DistributionGroup, "approval");
	//        open = db.OpenRquests;
	//        close = db.CloseRquests;
	//    }
	//}

	//public class AccessTeamRequestLoader : RequestLoader
	//{

	//    protected override void loadData(SNAPDatabaseDataContext db, ref Dictionary<string, object> open, ref Dictionary<string, object> close, ref Dictionary<string, object> search)
	//    {
	//        db.GetAllRequests(SnapSession.CurrentUser.LoginId, "accessteam");
	//        open = db.OpenRquests;
	//        close = db.CloseRquests;
	//    }
	//}

	//public class SearchRequestLoader : RequestLoader
	//{
	//    protected override void loadData(SNAPDatabaseDataContext db, ref Dictionary<string, object> open, ref Dictionary<string, object> close, ref Dictionary<string, object> search)
	//    {
	//        db.GetSearchRequests(searchText);
	//        search = db.SearchRquests;
	//    }
	//}
}
