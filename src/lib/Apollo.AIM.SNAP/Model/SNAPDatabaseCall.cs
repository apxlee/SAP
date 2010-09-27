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

    }
}
