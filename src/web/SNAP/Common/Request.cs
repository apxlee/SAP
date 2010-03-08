using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Common
{
    public class Request
    {
        public static List<usp_open_my_request_detailsResult> Details
        {
            get
            {
                if (requests.ContainsKey("reqDetails"))
                    return (List<usp_open_my_request_detailsResult>) requests["reqDetails"];

                return new List<usp_open_my_request_detailsResult>();
            }
        }

        public static List<SNAP_Access_User_Text> UserTexts
        {
            get
            {
                if (requests.ContainsKey("reqText"))
                    return (List<SNAP_Access_User_Text>)requests["reqText"];

                return new List<SNAP_Access_User_Text>();
            }
        }

        public static List<usp_open_my_request_commentsResult> Comments
        {
            get
            {
                if (requests.ContainsKey("reqComments"))
                    return (List<usp_open_my_request_commentsResult>) requests["reqComments"];

                return new List<usp_open_my_request_commentsResult>();
            }
        }

        public static List<usp_open_my_request_workflow_detailsResult> WfDetails
        {
            get
            {
                if (requests.ContainsKey("wfDetails"))
                    return (List<usp_open_my_request_workflow_detailsResult>) requests["wfDetails"];

                return new List<usp_open_my_request_workflow_detailsResult>();
            }
        }

        public static List<usp_open_my_request_workflow_commentsResult> WfComments
        {
            get
            {
                if (requests.ContainsKey("wfComments"))
                    return (List<usp_open_my_request_workflow_commentsResult>) requests["wfComments"];

                return new List<usp_open_my_request_workflow_commentsResult>();
            }
        }
        static bool exist
        {
            get
            {
                return HttpContext.Current.Items.Contains(RequestKey);
            }
        }

        static Dictionary<string, object> requests
        {
            get
            {
                if (exist)
                    return (Dictionary<string, object>) HttpContext.Current.Items[RequestKey];

                return new Dictionary<string, object>();
            }
        }

        public static string RequestKey
        {
            get
            {
                return WebUtilities.CurrentLoginUserId + "-" + "Requests";
            }
        }

    }
}
