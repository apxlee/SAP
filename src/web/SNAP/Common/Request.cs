using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Common
{
	// TODO: refactor this into a data model?  Rename 'request' since it's a protected word?
	//
    public class Request
    {
		//public static List<usp_open_my_request_detailsResult> Details(RequestState state)
		//{
		//    if (requests(state).ContainsKey("reqDetails"))
		//        return (List<usp_open_my_request_detailsResult>) requests(state)["reqDetails"];

		//    return new List<usp_open_my_request_detailsResult>();
		//}

		//public static List<SNAP_Access_User_Text> UserTexts(RequestState state)
		//{
		//        if (requests(state).ContainsKey("reqText"))
		//            return (List<SNAP_Access_User_Text>)requests(state)["reqText"];

		//        return new List<SNAP_Access_User_Text>();
		//}

		//public static List<usp_open_my_request_commentsResult> Comments(RequestState state)
		//{
		//        if (requests(state).ContainsKey("reqComments"))
		//            return (List<usp_open_my_request_commentsResult>) requests(state)["reqComments"];

		//        return new List<usp_open_my_request_commentsResult>();
		//}

		//public static List<usp_open_my_request_workflow_detailsResult> WfDetails(RequestState state)
		//{
		//        if (requests(state).ContainsKey("wfDetails"))
		//            return (List<usp_open_my_request_workflow_detailsResult>)requests(state)["wfDetails"];

		//        return new List<usp_open_my_request_workflow_detailsResult>();
		//}

		//public static List<usp_open_my_request_workflow_commentsResult> WfComments(RequestState state)
		//{
		//        if (requests(state).ContainsKey("wfComments"))
		//            return (List<usp_open_my_request_workflow_commentsResult>) requests(state)["wfComments"];

		//        return new List<usp_open_my_request_workflow_commentsResult>();
		//}
        
		//static bool exist(RequestState state)
		//{
		//    return HttpContext.Current.Items.Contains(GetKey(state));
		//}

		//static Dictionary<string, object> requests(RequestState state)
		//{
		//     if (exist(state))
		//        return (Dictionary<string, object>)HttpContext.Current.Items[GetKey(state)];

		//    return new Dictionary<string, object>();
		//}

		//private static string GetKey(RequestState state)
		//{
		//    var key = "";
		//    switch (state)
		//    {
		//        case RequestState.Open:
		//            key = OpenRequestKey;
		//            break;
		//        case RequestState.Closed:
		//            key = CloseRequestKey;
		//            break;
		//        case RequestState.Search:
		//            key = SearchRequestKey;
		//            break;
		//    }
		//    return key;
		//}

		//public static string OpenRequestKey
		//{
		//    get
		//    {
		//        return SnapSession.CurrentUser.LoginId + "-" + "OpenRequests";
		//    }
		//}

		//public static string CloseRequestKey
		//{
		//    get
		//    {
		//        return SnapSession.CurrentUser.LoginId + "-" + "CloseRequests";
		//    }
		//}

		//public static string SearchRequestKey
		//{
		//    get
		//    {
		//        return SnapSession.CurrentUser.LoginId + "-" + "SearchRequests";
		//    }
		//}

		//public static DataTable GetChangeComments(int requestId)
		//{
		//    DataTable table = new DataTable();
		//    table.Columns.Add("ActorDisplayName", typeof(string));
		//    table.Columns.Add("Comment", typeof(string));
		//    table.Columns.Add("CommentDate", typeof(string));

		//    using(var db = new SNAPDatabaseDataContext())
		//    {
		//        var changeComments = (from sr in db.SNAP_Requests
		//                              join sw in db.SNAP_Workflows on sr.pkId equals sw.requestId
		//                              join swc in db.SNAP_Workflow_Comments on sw.pkId equals swc.workflowId
		//                              join sa in db.SNAP_Actors on sw.actorId equals sa.pkId
		//                              where sr.pkId == requestId
		//                              && swc.commentTypeEnum == 3
		//                              select new { sa.displayName, swc.commentText, swc.createdDate });
		//        foreach (var comment in changeComments)
		//        {
		//            table.Rows.Add(comment.displayName, comment.commentText, comment.createdDate);
		//        }
		//    }
           
		//    return table;
		//}

		//public static int AccessTeamCount()
		//{
		//    using (var db = new SNAPDatabaseDataContext())
		//    {
		//        int result = db.SNAP_Requests
		//                     .Where(o => o.statusEnum == 0 || o.statusEnum == 1 || o.statusEnum == 2)
		//                     .Select(s => s.statusEnum)
		//                     .Count();

		//        return result;
		//    } 
		//}

		//public static int ApprovalCount(string[] userIds)
		//{
		//    using (var db = new SNAPDatabaseDataContext())
		//    {
		//        int result = (from sr in db.SNAP_Requests
		//                    join sw in db.SNAP_Workflows on sr.pkId equals sw.requestId
		//                    join sws in db.SNAP_Workflow_States on sw.pkId equals sws.workflowId
		//                    join sa in db.SNAP_Actors on sw.actorId equals sa.pkId
		//                    where userIds.Contains(sa.userId)
		//                    && sws.workflowStatusEnum == 7
		//                    && sws.completedDate == null
		//                    && sr.statusEnum == 2
		//                    select new {sr.pkId}).GroupBy(p=> p.pkId).Count();

		//        return result;
		//    }
		//}

        /* TODO method for Access Team Filter Counts
        public static List<int> AccessFilterCount()
        {
            List<int> filterList = new List<int>();

            using (var db = new SNAPDatabaseDataContext())
            {
                var pendingAck = from sr in db.SNAP_Requests
                                 join sw in db.SNAP_Workflows on sr.pkId equals sw.requestId
                                 join sws in db.SNAP_Workflow_States on sw.pkId equals sws.workflowId
                                 join sa in db.SNAP_Actors on sw.actorId equals sa.pkId
                                 where sws.workflowStatusEnum == 6 && 
                                 sr.statusEnum == 0 || 
                                 sr.statusEnum == 1 || 
                                 sr.statusEnum == 2
                                 select new { sr.pkId };

                var pendingWor = from sr in db.SNAP_Requests
                                 join sw in db.SNAP_Workflows on sr.pkId equals sw.requestId
                                 join sws in db.SNAP_Workflow_States on sw.pkId equals sws.workflowId
                                 join sa in db.SNAP_Actors on sw.actorId equals sa.pkId
                                 where sws.workflowStatusEnum == 9 &&
                                 sr.statusEnum == 0 ||
                                 sr.statusEnum == 1 ||
                                 sr.statusEnum == 2
                                 select new { sr.pkId };

                var pendingPro = from sr in db.SNAP_Requests
                                 join sw in db.SNAP_Workflows on sr.pkId equals sw.requestId
                                 join sws in db.SNAP_Workflow_States on sw.pkId equals sws.workflowId
                                 join sa in db.SNAP_Actors on sw.actorId equals sa.pkId
                                 where sws.workflowStatusEnum == 8 &&
                                 sr.statusEnum == 0 ||
                                 sr.statusEnum == 1 ||
                                 sr.statusEnum == 2
                                 select new { sr.pkId };

                filterList.Add((int)pendingAck.Count());
                filterList.Add((int)pendingWor.Count());
                filterList.Add((int)pendingPro.Count());
            }

            return filterList;
        }

        */



    }
}
