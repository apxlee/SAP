using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
    public partial class SNAPDatabaseDataContext
    {

        private Dictionary<string, object> _openRquests = new Dictionary<string, object>();
        private Dictionary<string, object> _closeRquests = new Dictionary<string, object>();

        public Dictionary<string, object> OpenRquests
        {
            get { return _openRquests; }

        }

        public Dictionary<string, object> CloseRquests
        {
            get { return _closeRquests; }

        }

        public IEnumerable<SNAP_Access_User_Text> RetrieveRequestUserText(int requestId)
        {
            //using (var db = new SNAPDatabaseDataContext())
            //{
                var data = from userText in SNAP_Access_User_Texts
                           where (userText.requestId == requestId)
                           group userText by userText.access_details_formId into g
                           let latest = g.Max(t => t.modifiedDate)
                           select new { newUserText = g.Where(t => t.modifiedDate == latest) };


                foreach (var x in data)
                {
                    foreach (var y in x.newUserText)
                    {
                        yield return y;
                    }
                }
            //}

        }
        


        /*
         * !!! These are customed SP that require special anotation !!!
         * 
         * Don't use the LinqToSql tool to drap these SP to autognerate code
         * Hand code these SP
         */

        [Function(Name = "dbo.usp_requests")]
        [ResultType(typeof(usp_open_my_request_detailsResult))]
        [ResultType(typeof(SNAP_Access_User_Text))]
        [ResultType(typeof(usp_open_my_request_commentsResult))]
        [ResultType(typeof(usp_open_my_request_workflow_detailsResult))]
        [ResultType(typeof(usp_open_my_request_workflow_commentsResult))]
        public IMultipleResults usp_requests([Parameter(DbType = "NVarChar(10)")] string userId, [Parameter(DbType = "NVarChar(10)")] string role)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userId, role);
            return ((IMultipleResults)(result.ReturnValue));
        }


        // this is for the app to call, so request data (open, close) are available
        public void GetAllRequests(string userId, string role)
        {
            IMultipleResults result = usp_requests(userId, role);
            populateAllRequests(result, _openRquests, _closeRquests);
        }


        static void populateAllRequests(IMultipleResults result, Dictionary<string, object> myOpenRequests, Dictionary<string, object> myCloseRequests)
        {
            myOpenRequests.Clear();
            myCloseRequests.Clear();

            if (result.ReturnValue.ToString() == "0")
            {
                myOpenRequests.Add("reqDetails", result.GetResult<usp_open_my_request_detailsResult>().ToList());
                myOpenRequests.Add("reqText", result.GetResult<SNAP_Access_User_Text>().ToList());
                myOpenRequests.Add("reqComments", result.GetResult<usp_open_my_request_commentsResult>().ToList());
                myOpenRequests.Add("wfDetails", result.GetResult<usp_open_my_request_workflow_detailsResult>().ToList());
                myOpenRequests.Add("wfComments", result.GetResult<usp_open_my_request_workflow_commentsResult>().ToList());

                myCloseRequests.Add("reqDetails", result.GetResult<usp_open_my_request_detailsResult>().ToList());
                myCloseRequests.Add("reqText", result.GetResult<SNAP_Access_User_Text>().ToList());
                myCloseRequests.Add("reqComments", result.GetResult<usp_open_my_request_commentsResult>().ToList());
                myCloseRequests.Add("wfDetails", result.GetResult<usp_open_my_request_workflow_detailsResult>().ToList());
                myCloseRequests.Add("wfComments", result.GetResult<usp_open_my_request_workflow_commentsResult>().ToList());

            }

        }

    }
}
