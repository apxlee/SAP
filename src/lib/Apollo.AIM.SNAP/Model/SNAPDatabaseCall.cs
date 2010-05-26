using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Model
{
    public partial class SNAPDatabaseDataContext
    {

        private Dictionary<string, object> _openRquests = new Dictionary<string, object>();
        private Dictionary<string, object> _closeRquests = new Dictionary<string, object>();
        private Dictionary<string, object> _searchRquests = new Dictionary<string, object>();

        public Dictionary<string, object> OpenRquests
        {
            get { return _openRquests; }

        }

        public Dictionary<string, object> CloseRquests
        {
            get { return _closeRquests; }

        }

        public Dictionary<string, object> SearchRquests
        {
            get { return _searchRquests; }

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
        

        public int GetActiveWorkflowId(long requestId, string userId)
        {
            var wfId = 0;

            var req = SNAP_Requests.Single(r => r.pkId == requestId);
            try
            {
                foreach (SNAP_Workflow w in req.SNAP_Workflows)
                {
                    if (w.SNAP_Actor.userId == userId && w.SNAP_Actor.isActive)
                    {
                        /*
                        var state =
                            w.SNAP_Workflow_States.Single(
                                s => s.workflowStatusEnum == (byte) WorkflowState.Pending_Approval);
                        if (state.completedDate == null && state.notifyDate != null)
                        {
                            actorId = w.SNAP_Actor.pkId;
                            break;
                        }
                         */

                        var state = w.SNAP_Workflow_States.Where(s => s.completedDate == null && s.notifyDate != null).ToList();
                        if (state.Count == 1 && state[0].workflowStatusEnum == (byte)WorkflowState.Pending_Approval)
                        {
                            wfId = w.pkId;
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("SNAPDatabaseCall - GetActiveWorkflowId, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace); 
            }
            return wfId;
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

        public IMultipleResults usp_requests([Parameter(DbType = "NVarChar(128)")] string userId, [Parameter(DbType = "NVarChar(10)")] string role)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userId, role);
            return ((IMultipleResults)(result.ReturnValue));
        }

        [Function(Name = "dbo.usp_search_requests")]
        [ResultType(typeof(usp_open_my_request_detailsResult))]
        [ResultType(typeof(SNAP_Access_User_Text))]
        [ResultType(typeof(usp_open_my_request_commentsResult))]
        [ResultType(typeof(usp_open_my_request_workflow_detailsResult))]
        [ResultType(typeof(usp_open_my_request_workflow_commentsResult))]

        public IMultipleResults usp_search_requests([Parameter(DbType = "NVarChar(100)")] string search)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), search);
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

        public void GetSearchRequests(string search)
        {
            IMultipleResults result = usp_search_requests(search);
            populateSearchRequests(result, _searchRquests);
        }

        static void populateSearchRequests(IMultipleResults result, Dictionary<string, object> mySearchRequests)
        {
            mySearchRequests.Clear();

            if (result.ReturnValue.ToString() == "0")
            {
                mySearchRequests.Add("reqDetails", result.GetResult<usp_open_my_request_detailsResult>().ToList());
                mySearchRequests.Add("reqText", result.GetResult<SNAP_Access_User_Text>().ToList());
                mySearchRequests.Add("reqComments", result.GetResult<usp_open_my_request_commentsResult>().ToList());
                mySearchRequests.Add("wfDetails", result.GetResult<usp_open_my_request_workflow_detailsResult>().ToList());
                mySearchRequests.Add("wfComments", result.GetResult<usp_open_my_request_workflow_commentsResult>().ToList());
            }

        }

    }
}
