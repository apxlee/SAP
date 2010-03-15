using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
    public class AccessRequest
    {
        private int _id;
        public AccessRequest(int i)
        {
            _id = i;
        }

        public void Ack()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var wf = db.SNAP_Workflows.Single(x => x.requestId == _id && x.actorId == 1);

                // complete previous state
                var state = db.SNAP_Workflow_States.Single(x => x.workflowId == wf.pkId && x.workflowStatusEnum == (byte) WorkflowState.Pending_Acknowlegement);
                state.completedDate = DateTime.Now;

                var newState = new SNAP_Workflow_State()
                                   {
                                       //workflowId = wf.pkId,
                                       completedDate = null,
                                       dueDate = DateTime.Now.AddDays(1),
                                       notifyDate = DateTime.Now,
                                       workflowStatusEnum = (byte) WorkflowState.Pending_Workflow
                                };
                //db.SNAP_Workflow_States.InsertOnSubmit(newState);

                // enter new state
                wf.SNAP_Workflow_States.Add(newState);

                db.SubmitChanges();
            }

        }

        public void CreateWorkflow(List<int> actorIds)
        {
            using (var db = new SNAPDatabaseDataContext())
            {

                createrApprovalWorkFlow(db, _id, actorIds);

                // complete current state
                var wf = db.SNAP_Workflows.Single(x => x.requestId == _id && x.actorId == 1);
                var state = db.SNAP_Workflow_States.Single(x => x.workflowId == wf.pkId && x.workflowStatusEnum == (byte)WorkflowState.Pending_Workflow);
                state.completedDate = DateTime.Now;


                // add new state
                var newState = new SNAP_Workflow_State()
                                   {
                                       completedDate = null,
                                       dueDate = DateTime.Now.AddDays(1),
                                       notifyDate = DateTime.Now,
                                       workflowStatusEnum = (byte) WorkflowState.Pending_Approval
                                   };


                wf = db.SNAP_Workflows.Single(x => x.requestId == _id && x.actorId == 1);
                wf.SNAP_Workflow_States.Add(newState);

                db.SubmitChanges();
            }

            orderRequestWFAction();
        }

        private void createrApprovalWorkFlow(SNAPDatabaseDataContext db, int reqId, List<int> actorIds)
        {
            var req = db.SNAP_Requests.Single(x => x.pkId == reqId);

            foreach (var actId in actorIds)
            {
                var newWorkFlow = new SNAP_Workflow()
                                      {
                                          actorId = actId,
                                      };

                req.SNAP_Workflows.Add(newWorkFlow);

                var newState = new SNAP_Workflow_State()
                                   {
                                       completedDate = null,
                                       dueDate = DateTime.Now.AddDays(1),
                                       notifyDate = null,
                                       workflowStatusEnum = (byte) WorkflowState.Pending_Approval
                                   };

                newWorkFlow.SNAP_Workflow_States.Add(newState);
            }
            // TODO - send out email to the manager approval
        }

        private void orderRequestWFAction()
        {
            if (needManagerApproval)
            {
                var x = 1;
            }
            else if (needTeamApproval)
            {
                
            }
            else // teanical approval
            {
                
            }
        }

        private bool needManagerApproval
        {
            get
            {
                
                using (var db = new SNAPDatabaseDataContext())
                {
                    var wfs = db.SNAP_Workflows.Where(w => w.requestId == _id);
                    foreach (var wf in wfs)
                    {
                        var actGroup = db.SNAP_Actors.Single(a => a.pkId == wf.actorId);
                        var actGroupType = db.SNAP_Actor_Groups.Single(g => g.pkId == actGroup.actor_groupId);
                        if (actGroupType.pkId == 2) 
                            return true;
                    }

                }
                return false;
            }
        }

        private bool needTeamApproval
        {
            get { return true;}
        }
    }

}
