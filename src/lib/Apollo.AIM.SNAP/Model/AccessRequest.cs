﻿using System;
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

        #region public methods

        public void Ack()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var wf = db.SNAP_Workflows.Single(x => x.requestId == _id && x.actorId == 1);

                // complete previous state
                //var state = db.SNAP_Workflow_States.Single(x => x.workflowId == wf.pkId && x.workflowStatusEnum == (byte)WorkflowState.Pending_Acknowlegement);
                var state = wf.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte) WorkflowState.Pending_Acknowlegement);
                stateTransition(ActorApprovalType.Workflow_Admin,  wf, state, WorkflowState.Pending_Acknowlegement, WorkflowState.Pending_Workflow);

                wf.SNAP_Request.statusEnum = (byte) RequestState.Pending;
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
                //var state = db.SNAP_Workflow_States.Single(x => x.workflowId == wf.pkId && x.workflowStatusEnum == (byte)WorkflowState.Pending_Workflow);
                var state = wf.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Workflow);
                stateTransition(ActorApprovalType.Workflow_Admin, wf, state, WorkflowState.Pending_Workflow, WorkflowState.Pending_Approval);
                db.SubmitChanges();
            }

        }

        public List<SNAP_Workflow> FindApprovalTypeWF(SNAPDatabaseDataContext db, int wfType)
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

        public void InformApproverForAction()
        {
            bool done = false;
            using (var db = new SNAPDatabaseDataContext())
            {
                // manager approval
                var wfs = FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                // TODO - if request submitter is the manager, it should be auto approved and no need for this activity
                done = emailApproverForAction(wfs);

                // team approval
                if (!done)
                {
                    wfs = FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);

                    done = emailApproverForAction(wfs);
                }

                // technical approvals
                if (!done)
                {
                    wfs = FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                    done = emailApproverForAction(wfs);

                }

                db.SubmitChanges();
            }
        }


        public void WorkflowAck(int wid, WorkflowAction action)
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var approvalType = workflowApprovalType(db, wid);
                var wf = db.SNAP_Workflows.Single(w => w.pkId == wid);
                var state = wf.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval);
                //state.completedDate = DateTime.Now;
                switch (approvalType)
                {
                    case (byte)ActorApprovalType.Manager:
                    case (byte)ActorApprovalType.Team_Approver:
                        switch (action)
                        {
                            case WorkflowAction.Approved:
                                InformApproverForAction();
                                break;
                            case WorkflowAction.Change:
                                break;
                            case WorkflowAction.Denied:
                                // TODO - close denied the who request!
                                break;
                        }
                        stateTransition((ActorApprovalType)approvalType, wf, state, WorkflowState.Pending_Approval, (WorkflowState)action);
                        break;

                    case (byte)ActorApprovalType.Technical_Approver:
                        switch (action)
                        {
                            case WorkflowAction.Approved:
                                stateTransition((ActorApprovalType)approvalType, wf, state, WorkflowState.Pending_Approval, (WorkflowState)action);
                                completeRequestApprovalCheck(db);
                                break;
                            case WorkflowAction.Change:
                                break;
                            case WorkflowAction.Denied:
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


        public void CreateServiceDeskTicket()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var accessTeamWF = FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin)[0];
                var accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Approved);
                stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF, accessTeamState, WorkflowState.Approved, WorkflowState.Pending_Provisioning);


                // TODO - create SD here, save ticket in the request table


                db.SubmitChanges();
            }
        }

        public void FinalizeRequest()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var accessTeamWF = FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin)[0];
                var accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Provisioning);
                stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF, accessTeamState, WorkflowState.Pending_Provisioning, WorkflowState.Closed_Completed);


                // TODO - Info requester it is done

                accessTeamWF.SNAP_Request.statusEnum = (byte)RequestState.Closed;
                db.SubmitChanges();
            }

        }

#endregion

        #region private methods

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

                var actor = db.SNAP_Actors.Single(a => a.pkId == actId);
                var agt = actor.SNAP_Actor_Group.actorGroupType ?? 3; // default to accessteam if null

                ActorApprovalType t = (ActorApprovalType) agt; 

                stateTransition(t, newWorkFlow, null, WorkflowState.Not_Active, WorkflowState.Pending_Approval);
            }
            // TODO - send out email to the manager approval
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
                            // TODO - send email
                            state.notifyDate = DateTime.Now;
                            done = true;
                        }
                    }
                }
            }

            return done;
        }

        private int workflowApprovalType(SNAPDatabaseDataContext db, int wid)
        {

            var wf = db.SNAP_Workflows.Single(w => w.pkId == wid);

            if (wf.SNAP_Actor.actor_groupId == 0)
                return (byte)ActorApprovalType.Workflow_Admin;

            return wf.SNAP_Actor.SNAP_Actor_Group.actorGroupType ?? -1;

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

        private SNAP_Workflow_State createState(DateTime complete, DateTime due, DateTime notify, WorkflowState s)
        {
            var state = new SNAP_Workflow_State()
            {
                completedDate = complete,
                dueDate = due,
                notifyDate = notify,
                workflowStatusEnum = (byte)s
            };
            return state;
            
        }
        private void completeRequestApprovalCheck(SNAPDatabaseDataContext db)
        {
            db.SubmitChanges(); // !!! Need to commit the db first, before checking final approver status

            var wfs = FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
            var totalApproved = 0;
            foreach (var wf in wfs)
            {
                totalApproved += wf.SNAP_Workflow_States.Count(s => s.workflowStatusEnum == (byte)WorkflowState.Approved);
            }

            if (totalApproved == wfs.Count)
            {
                //search for pending and not complete wf

                //var req = db.SNAP_Requests.Single(x => x.pkId == _id);
                //var accessTeamWF = req.SNAP_Workflows.Single(w => w.actorId == 1); // actid = 1 => accessTeam
                var accessTeamWF = FindApprovalTypeWF(db, (byte) ActorApprovalType.Workflow_Admin)[0]; // req.SNAP_Workflows.Single(w => w.actorId == 1)); // actid = 1 => accessTeam
                var accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval);

                stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF, accessTeamState, WorkflowState.Pending_Approval, WorkflowState.Approved);
                //req.statusEnum = (byte) WorkflowState.Approved;
            }

        }



        private void stateTransition(ActorApprovalType approvalType,  SNAP_Workflow wf, SNAP_Workflow_State prevWFState, WorkflowState from, WorkflowState to)
        {
            if (prevWFState != null)
                prevWFState.completedDate = DateTime.Now;

            var state = new SNAP_Workflow_State()
                            {
                                completedDate = null,
                                notifyDate = DateTime.Now,
                                dueDate = getDueDate(approvalType, from, to), //DateTime.Now.AddDays(1),
                                workflowStatusEnum = (byte)to
                            };
            // for end/close states set the completion date
            if (to == WorkflowState.Closed_Cancelled || to == WorkflowState.Closed_Completed || to == WorkflowState.Closed_Denied ) //||
            {
                state.completedDate = state.dueDate = DateTime.Now;
            }

            // workflowstate.approved is end state for manger, team approval and techical aproval but not for workflow adim
            if (to == WorkflowState.Approved && approvalType != ActorApprovalType.Workflow_Admin)
            {
                state.completedDate = state.dueDate = DateTime.Now;
            }

            wf.SNAP_Workflow_States.Add(state);
        }

        private DateTime getDueDate(ActorApprovalType approvalType, WorkflowState fr, WorkflowState to)
        {
            return DateTime.Now.AddDays(1);
        }

        #endregion
    }

}
