using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Common
{
    public class Request
    {
        public static List<usp_open_my_request_detailsResult> Details(RequestState state)
        {
            if (requests(state).ContainsKey("reqDetails"))
                return (List<usp_open_my_request_detailsResult>) requests(state)["reqDetails"];

            return new List<usp_open_my_request_detailsResult>();
        }

        public static List<SNAP_Access_User_Text> UserTexts(RequestState state)
        {
                if (requests(state).ContainsKey("reqText"))
                    return (List<SNAP_Access_User_Text>)requests(state)["reqText"];

                return new List<SNAP_Access_User_Text>();
        }

        public static List<usp_open_my_request_commentsResult> Comments(RequestState state)
        {
                if (requests(state).ContainsKey("reqComments"))
                    return (List<usp_open_my_request_commentsResult>) requests(state)["reqComments"];

                return new List<usp_open_my_request_commentsResult>();
        }

        public static List<usp_open_my_request_workflow_detailsResult> WfDetails(RequestState state)
        {
                if (requests(state).ContainsKey("wfDetails"))
                    return (List<usp_open_my_request_workflow_detailsResult>)requests(state)["wfDetails"];

                return new List<usp_open_my_request_workflow_detailsResult>();
        }

        public static List<usp_open_my_request_workflow_commentsResult> WfComments(RequestState state)
        {
                if (requests(state).ContainsKey("wfComments"))
                    return (List<usp_open_my_request_workflow_commentsResult>) requests(state)["wfComments"];

                return new List<usp_open_my_request_workflow_commentsResult>();
        }
        
        static bool exist(RequestState state)
        {
            var key = "";
            switch (state)
            {
                case RequestState.Open:
                    key = OpenRequestKey;
                    break;
                case RequestState.Closed:
                    key = CloseRequestKey;
                    break;
                case RequestState.Search:
                    key = SearchRequestKey;
                    break;
            }

            return HttpContext.Current.Items.Contains(key);
        }

        static Dictionary<string, object> requests(RequestState state)
        {
            var key = "";
            switch (state)
            {
                case RequestState.Open:
                    key = OpenRequestKey;
                    break;
                case RequestState.Closed:
                    key = CloseRequestKey;
                    break;
                case RequestState.Search:
                    key = SearchRequestKey;
                    break;
            }

            if (exist(state))
                return (Dictionary<string, object>) HttpContext.Current.Items[key];

            return new Dictionary<string, object>();
        }

        public static string OpenRequestKey
        {
            get
            {
                return SnapSession.CurrentUser.LoginId + "-" + "OpenRequests";
            }
        }

        public static string CloseRequestKey
        {
            get
            {
                return SnapSession.CurrentUser.LoginId + "-" + "CloseRequests";
            }
        }

        public static string SearchRequestKey
        {
            get
            {
                return SnapSession.CurrentUser.LoginId + "-" + "SearchRequests";
            }
        }

        public static int AccessTeamCount()
        {
            int num = 0;

            using (var db = new SNAPDatabaseDataContext())
            {
                var result = db.SNAP_Requests
                             .Where(o => o.statusEnum == 0 || o.statusEnum == 2)
                             .Select(s => s.statusEnum)
                             .Count();

                num = (int)result;
            }

            return num;
        }

        public static int ApprovalCount(string UserID)
        {
            int num = 0;

            using (var db = new SNAPDatabaseDataContext())
            {
                var result = from sr in db.SNAP_Requests
                            join sw in db.SNAP_Workflows on sr.pkId equals sw.requestId
                            join sws in db.SNAP_Workflow_States on sw.pkId equals sws.workflowId
                            join sa in db.SNAP_Actors on sw.actorId equals sa.pkId
                            where sa.userId == UserID && sws.workflowStatusEnum == 7
                            select new {sr.pkId};

                num = (int)result.Count();
            }

            return num;
        }





    }
}
