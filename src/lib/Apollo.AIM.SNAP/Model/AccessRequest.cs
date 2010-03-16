using System;
using System.Collections.Generic;
using System.Data.Linq;
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
                var state = db.SNAP_Workflow_States.Single(x => x.workflowId == wf.pkId && x.workflowStatusEnum == (byte)WorkflowState.Pending_Acknowlegement);
                state.completedDate = DateTime.Now;

                var newState = new SNAP_Workflow_State()
                {
                    //workflowId = wf.pkId,
                    completedDate = null,
                    dueDate = DateTime.Now.AddDays(1),
                    notifyDate = DateTime.Now,
                    workflowStatusEnum = (byte)WorkflowState.Pending_Workflow
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

                createrApprovalWorkFlow(db, actorIds);

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
                    workflowStatusEnum = (byte)WorkflowState.Pending_Approval
                };


                wf = db.SNAP_Workflows.Single(x => x.requestId == _id && x.actorId == 1);
                wf.SNAP_Workflow_States.Add(newState);

                db.SubmitChanges();
            }

        }

        private void createrApprovalWorkFlow(SNAPDatabaseDataContext db, List<int> actorIds)
        {
            var req = db.SNAP_Requests.Single(x => x.pkId == _id);

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
                    workflowStatusEnum = (byte)WorkflowState.Pending_Approval
                };

                newWorkFlow.SNAP_Workflow_States.Add(newState);
            }
            // TODO - send out email to the manager approval
        }


        public void InformApprovalForAction()
        {
            bool done = false;
            using (var db = new SNAPDatabaseDataContext())
            {
                // manager approval
                var wfs = findApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                // TODO - if request submitter is the manager, it should be auto approved and no need for this activity
                done = emailApproverForAction(wfs);

                // team approval
                if (!done)
                {
                    wfs = findApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);

                    done = emailApproverForAction(wfs);
                }

                // technical approvals
                if (!done)
                {
                    wfs = findApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                    done = emailApproverForAction(wfs);

                }

                db.SubmitChanges();
            }
        }

        private bool emailApproverForAction(List<SNAP_Workflow> wfs)
        {
            var done = false;

            if (wfs.Count > 0)
            {
                foreach (var wf in wfs)
                {
                    foreach (var state in wf.SNAP_Workflow_States)
                    {
                        if (state.notifyDate == null && state.workflowStatusEnum == (byte)WorkflowState.Pending_Approval)
                        {
                            // send email
                            state.notifyDate = DateTime.Now;
                            done = true;
                        }
                    }
                }
            }

            return done;
        }
        public List<SNAP_Workflow> findApprovalTypeWF(SNAPDatabaseDataContext db, int wfType)
        {
            List<SNAP_Workflow> wfList = new List<SNAP_Workflow>();
            var wfs = db.SNAP_Workflows.Where(w => w.requestId == _id);
            foreach (var wf in wfs)
            {
                var t = workflowApprovalType(db, wf.pkId);
                if (t != -1 && t == wfType)
                    wfList.Add(wf);
            }
            return wfList;
        }

        private int workflowApprovalType(SNAPDatabaseDataContext db, int wid)
        {

            var wf = db.SNAP_Workflows.Single(w => w.pkId == wid);

            if (wf.SNAP_Actor.actor_groupId == 0)
                return (byte)ActorApprovalType.Workflow_Admin;

            return wf.SNAP_Actor.SNAP_Actor_Group.actorGroupType ?? -1;

        }

        public void WorkflowAck(int wid, WorkflowAction action)
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var approvalType = workflowApprovalType(db, wid);
                var wf = db.SNAP_Workflows.Single(w => w.pkId == wid);
                var state = wf.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval);
                state.completedDate = DateTime.Now;
                switch (approvalType)
                {
                    case (byte)ActorApprovalType.Manager:
                    case (byte)ActorApprovalType.Team_Approver:
                        switch (action)
                        {
                            case WorkflowAction.Approved:
                                wf.SNAP_Workflow_States.Add(createState(WorkflowState.Approved));
                                InformApprovalForAction();
                                break;
                            case WorkflowAction.Change:
                                break;
                            case WorkflowAction.Denied:
                                wf.SNAP_Workflow_States.Add(createState(WorkflowState.Closed_Denied));
                                // TODO - close denied the who request!

                                break;
                        }
                        break;
/*
                    case (byte)ActorApprovalType.Team_Approver:
                        switch (action)
                        {
                            case WorkflowAction.Approved:
                                wf.SNAP_Workflow_States.Add(createState(WorkflowState.Approved));
                                InformApprovalForAction();
                                break;
                            case WorkflowAction.Change:
                                break;
                            case WorkflowAction.Denied:
                                wf.SNAP_Workflow_States.Add(createState(WorkflowState.Closed_Denied));
                                // TODO - close denied the who request!

                                break;
                        }
                        break;
*/
                    case (byte)ActorApprovalType.Technical_Approver:
                        switch (action)
                        {
                            case WorkflowAction.Approved:
                                wf.SNAP_Workflow_States.Add(createState(WorkflowState.Approved));
                                completeRequestApprovalCheck(db);
                                break;
                            case WorkflowAction.Change:
                                break;
                            case WorkflowAction.Denied:
                                wf.SNAP_Workflow_States.Add(createState(WorkflowState.Closed_Denied));
                                // TODO - close denied the who request!

                                break;
                        }

                        break;

                }
                // complete the date
                // manager ack
                // team ack
                // technical mgr ack
                // determine next state

                db.SubmitChanges();
            }
        }

        private SNAP_Workflow_State createState(WorkflowState s)
        {
            var state = new SNAP_Workflow_State()
            {
                completedDate = DateTime.Now,
                dueDate = DateTime.Now,
                notifyDate = DateTime.Now,
                workflowStatusEnum = (byte)s
            };
            return state;

        }
        private void completeRequestApprovalCheck(SNAPDatabaseDataContext db)
        {
            db.SubmitChanges(); // !!! Need to commit the db first, before checking final approver status

            var wfs = findApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
            var totalApproved = 0;
            foreach (var wf in wfs)
            {
                totalApproved += wf.SNAP_Workflow_States.Count(s => s.workflowStatusEnum == (byte)WorkflowState.Approved);
            }

            if (totalApproved == wfs.Count)
            {
                //search for pending and not complete wf

                var req = db.SNAP_Requests.Single(x => x.pkId == _id);
                var accessTeamWF = req.SNAP_Workflows.Single(w => w.actorId == 1); // actid = 1 => accessTeam
                var accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval);

                accessTeamState.completedDate = DateTime.Now;

                var state = new SNAP_Workflow_State()
                {
                    completedDate = DateTime.Now,
                    notifyDate = DateTime.Now,
                    dueDate = DateTime.Now,
                    workflowStatusEnum = (byte)WorkflowState.Approved
                };
                accessTeamWF.SNAP_Workflow_States.Add(state);
                //req.statusEnum = (byte) WorkflowState.Approved;
            }

        }

        public void CreateServiceDeskTicket()
        {

        }

        public void FinalizeRequest()
        {

        }
    }

}
