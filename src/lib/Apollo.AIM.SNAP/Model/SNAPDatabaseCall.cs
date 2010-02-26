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
        
        [Function(Name = "dbo.usp_open_user_view_tab")]
        [ResultType(typeof(usp_open_user_view_statusResult))]
        [ResultType(typeof(usp_open_user_view_detailsResult))]
        public IMultipleResults usp_open_user_view_tab([Parameter(DbType = "NVarChar(10)")] string userId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userId);
            return (IMultipleResults)(result.ReturnValue);
        }
         

    }
}
