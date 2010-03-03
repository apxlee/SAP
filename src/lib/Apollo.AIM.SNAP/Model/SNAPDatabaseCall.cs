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
        public IEnumerable<SNAP_Access_User_Text> RetrieveRequest(int requestId)
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

        [Function(Name = "dbo.usp_open_my_request_text")]
        public ISingleResult<SNAP_Access_User_Text> usp_open_my_request_text([Parameter(DbType = "NVarChar(10)")] string userId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userId);
            return ((ISingleResult<SNAP_Access_User_Text>)(result.ReturnValue));
        }


        [Function(Name = "dbo.usp_open_my_request_tab")]
        [ResultType(typeof(usp_open_my_request_detailsResult))]
        [ResultType(typeof(SNAP_Access_User_Text))]
        [ResultType(typeof(usp_open_my_request_commentsResult))]
        [ResultType(typeof(usp_open_my_request_workflow_detailsResult))]
        [ResultType(typeof(usp_open_my_request_workflow_commentsResult))]
        public IMultipleResults usp_open_my_request_tab([Parameter(DbType = "NVarChar(10)")] string userId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userId);
            return ((IMultipleResults)(result.ReturnValue));
        }



        // this method just package my open request call results into dictionary type
        public Dictionary<string, object> MyOpenRequests(string userId)
        {

            var myRequests = new Dictionary<string, object>();
            IMultipleResults result = usp_open_my_request_tab(userId);
            if (result.ReturnValue.ToString() == "0")
            {
                myRequests.Add("reqDetails", result.GetResult<usp_open_my_request_detailsResult>().ToList());
                myRequests.Add("reqText", result.GetResult<SNAP_Access_User_Text>().ToList());
                myRequests.Add("reqComments", result.GetResult<usp_open_my_request_commentsResult>().ToList());
                myRequests.Add("wfDetails", result.GetResult<usp_open_my_request_workflow_detailsResult>().ToList());
                myRequests.Add("wfComments", result.GetResult<usp_open_my_request_workflow_commentsResult>().ToList());
            }

            return myRequests;
        }
    }
}
