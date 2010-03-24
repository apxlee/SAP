using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;

using System.Text;

namespace Apollo.AIM.SNAP.Model
{
    public class AccessRequest
    {
        private int _id;
        private static int AccessTeamActorId = 1;

        public AccessRequest(int i)
        {
            _id = i;

        }

        #region public methods

        public bool Ack()
        {
            bool result = false;
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
                    result = reqStateTransition(req, RequestState.Open, RequestState.Pending,
                                                accessTeamWF, WorkflowState.Pending_Acknowlegement,
                                                WorkflowState.Pending_Workflow);

                    if (result)
                    {
                        db.SubmitChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool NoAccess(WorkflowAction action, string comment)
        {
            bool result = false;

            WorkflowState wfState = WorkflowState.Not_Active;
            CommentsType commentType = CommentsType.Cancelled;
            switch (action)
            {
                case WorkflowAction.Denied:
                    wfState = WorkflowState.Closed_Denied;
                    commentType = CommentsType.Denied;
                    break;
                case WorkflowAction.Cancel:
                    wfState = WorkflowState.Closed_Cancelled;
                    commentType = CommentsType.Cancelled;
                    break;

            }

            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
                    result = reqStateTransition(req, RequestState.Pending, RequestState.Closed,
                                                accessTeamWF, WorkflowState.Pending_Workflow,
                                                wfState);

                    if (result)
                    {
                        addAccessTeamComment(accessTeamWF, comment, commentType);
                        db.SubmitChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public bool RequestToChange(string comment)
        {
            bool result = false;

            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
                    result = reqStateTransition(req, RequestState.Pending, RequestState.Change_Requested,
                                                accessTeamWF, WorkflowState.Pending_Workflow,
                                                WorkflowState.Change_Requested);

                    if (result)
                    {
                        addAccessTeamComment(accessTeamWF, comment, CommentsType.Requested_Change);
                        db.SubmitChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool RequestChanged()
        {
            var result = false;

            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
                    result = reqStateTransition(req, RequestState.Change_Requested, RequestState.Open,
                                                accessTeamWF, WorkflowState.Change_Requested,
                                                WorkflowState.Pending_Acknowlegement);

                    if (result)
                    {
                        db.SubmitChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

            return result;
        }

        public bool CreateWorkflow(List<int> actorIds)
        {
            var result = false;
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
                    result = reqStateTransition(req, RequestState.Pending, RequestState.Pending,
                                                accessTeamWF, WorkflowState.Pending_Workflow,
                                                WorkflowState.Pending_Approval);

                    if (result)
                    {
                        createrApprovalWorkFlow(db, actorIds);

                        db.SubmitChanges();
                    }

                }
            }
            catch (Exception ex) {
                throw ex; 
            }
            return result;
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


        public bool WorkflowAck(int wid, WorkflowAction action)
        {
            return WorkflowAck(wid, action, string.Empty);
        }

        public bool WorkflowAck(int wid, WorkflowAction action, string comment)
        {
            var result = false;
            int approvalType;


            using (var db = new SNAPDatabaseDataContext())
            {
               var req = db.SNAP_Requests.Single(r => r.pkId == _id);
               if (req.statusEnum != (byte)RequestState.Pending)
                   return result;
                    
                System.Data.Common.DbTransaction trans = null;
                try
                {
                    db.Connection.Open();
                    trans = db.Connection.BeginTransaction();
                    db.Transaction = trans;

                    approvalType = workflowApprovalType(db, wid);
                    var wf = db.SNAP_Workflows.Single(w => w.pkId == wid);
                    var currentState = wf.SNAP_Workflow_States.Single(s => s.completedDate == null);

                    WorkflowState newState = WorkflowState.Not_Active;
                    switch ((ActorApprovalType)approvalType)
                    {
                        case ActorApprovalType.Manager:
                        case ActorApprovalType.Team_Approver:
                            handleApproval(action, (ActorApprovalType)approvalType, ref newState);
                            break;

                        case ActorApprovalType.Technical_Approver:
                            handleApproval(action, (ActorApprovalType)approvalType, ref newState);
                            break;

                    }

                    if (currentState.workflowStatusEnum == (byte)WorkflowState.Pending_Approval)
                    {
                        stateTransition((ActorApprovalType) approvalType, wf, WorkflowState.Pending_Approval, newState);
                        db.SubmitChanges();

                        //throw new Exception("Bad!");

                        if (approvalType == (byte) ActorApprovalType.Technical_Approver &&
                            action == WorkflowAction.Approved)
                            completeRequestApprovalCheck(db);

                        if (action == WorkflowAction.Denied)
                            deny(db, (ActorApprovalType) approvalType, comment);

                        if (action == WorkflowAction.Change)
                            approvalRequestToChange(db, wf, comment);

                        db.SubmitChanges();
                        trans.Commit();
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    //Logger.Error(ex);
                    if (trans != null)
                        trans.Rollback();
                    throw ex;
                }
                finally
                {
                    //db.SubmitChanges();
                    
                    if (db.Connection.State == ConnectionState.Open) 
                        db.Connection.Close();

                }
                return result; 
            }
        }



        public bool CreateServiceDeskTicket()
        {
            var result = false;
            try 
            {
                using (var db = new SNAPDatabaseDataContext())
                {

                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId); 
                    result = reqStateTransition(req, RequestState.Pending, RequestState.Pending,
                                                accessTeamWF, WorkflowState.Approved,
                                                WorkflowState.Pending_Provisioning);

                    if (result)
                    {
                        // TODO - create SD here, save ticket in the request table

                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool FinalizeRequest()
        {
            var result = false;
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
                    result = reqStateTransition(req, RequestState.Pending, RequestState.Closed,
                                                accessTeamWF, WorkflowState.Pending_Provisioning,
                                                WorkflowState.Closed_Completed);

                    if (result)
                    {
                        // TODO - Info requester it is done

                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

#endregion

        #region private methods

        private bool reqStateTransition(SNAP_Request req, RequestState reqFr, RequestState reqTo, SNAP_Workflow accessTeamWF,  WorkflowState wfFr, WorkflowState wfTo)
        {
            var result = false;
            if (req.statusEnum == (byte)reqFr)
            {
                //var accessTeamWF = FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin)[0];
                var accessTeamWFState = accessTeamWF.SNAP_Workflow_States.Single(s => s.completedDate == null);
                if (accessTeamWFState.workflowStatusEnum == (byte)wfFr)
                {
                    stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF,wfFr,wfTo);

                    if (reqFr != reqTo)
                    {
                        req.statusEnum = (byte) reqTo;
                        req.lastModifiedDate = DateTime.Now;

                    }
                    result = true;
                }
            }
            return result;
        }

        private void addAccessTeamComment(SNAP_Workflow accessTeamWF, string comment, CommentsType type)
        {
            accessTeamWF.SNAP_Workflow_Comments.Add(new SNAP_Workflow_Comment()
            {
                commentText = comment,
                commentTypeEnum = (byte)type,
                createdDate = DateTime.Now
            });

        }

        private void approvalRequestToChange(SNAPDatabaseDataContext db, SNAP_Workflow wf, string comment)
        {
            var accessTeamWF = FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin)[0];
            stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF, WorkflowState.Pending_Approval, WorkflowState.Change_Requested);
            db.SubmitChanges();

            var req = db.SNAP_Requests.Single(w => w.pkId == _id);
            req.statusEnum = (byte)RequestState.Change_Requested;

            // complete date all exitsting wf state
            foreach (var w in req.SNAP_Workflows)
            {
                if (w.actorId != 1) // all non accessteam wfs are completed!
                    foreach (var s in w.SNAP_Workflow_States)
                    {
                        if (s.completedDate == null)
                            s.completedDate = DateTime.Now;
                    }
            }

            wf.SNAP_Workflow_Comments.Add(new SNAP_Workflow_Comment()
            {
                commentText = comment,
                commentTypeEnum = (byte)CommentsType.Requested_Change,
                createdDate = DateTime.Now
            });
            // TODO - info submiter or requester
        }

        private void handleApproval(WorkflowAction action, ActorApprovalType approvalType, ref WorkflowState newState)
        {
            switch (action)
            {
                case WorkflowAction.Approved:
                    newState = WorkflowState.Approved;
                    if (approvalType == ActorApprovalType.Team_Approver || approvalType == ActorApprovalType.Manager)
                        InformApproverForAction();
                    break;
                case WorkflowAction.Change:
                    newState = WorkflowState.Change_Requested;
                    break;
                case WorkflowAction.Denied:
                    // TODO - close denied the who request!
                    newState = WorkflowState.Closed_Denied;
                    break;
                default:
                    newState = WorkflowState.Not_Active;
                    break;
            }
        }

        private void deny(SNAPDatabaseDataContext db, ActorApprovalType type, string comment)
        {
            //using (var db = new SNAPDatabaseDataContext())
            //{
                var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                var wfs = FindApprovalTypeWF(db, (byte)type);
                foreach (var wf in wfs)
                {
                    if (wf.SNAP_Workflow_States.Count(s => s.workflowStatusEnum == (byte)WorkflowState.Closed_Denied) == 1)
                    {
                        wf.SNAP_Workflow_Comments.Add(new SNAP_Workflow_Comment()
                        {
                            commentText = comment,
                            createdDate = DateTime.Now,
                            commentTypeEnum = (byte)CommentsType.Denied
                        });
                        // set accessTeam WF and request to close-denied
                        var accessTeamWF = FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin)[0];
                        stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF, WorkflowState.Pending_Approval, WorkflowState.Closed_Denied);
                        req.statusEnum = (byte)RequestState.Closed;

                        break;
                    }

                }
                //db.SubmitChanges();
            //}
        }

        private void createrApprovalWorkFlow(SNAPDatabaseDataContext db, List<int> actorIds)
        {
            var req = db.SNAP_Requests.Single(x => x.pkId == _id);

            foreach (var actId in actorIds)
            {

                SNAP_Workflow wf;
                if (req.SNAP_Workflows.Count(w => w.actorId == actId) == 0) // 
                {
                    wf  = new SNAP_Workflow() {actorId = actId };
                    req.SNAP_Workflows.Add(wf);
                }
                else
                {
                    wf = req.SNAP_Workflows.Single(w => w.actorId == actId); // wf already exists due to change request
                }

                var actor = db.SNAP_Actors.Single(a => a.pkId == actId);
                    var agt = actor.SNAP_Actor_Group.actorGroupType ?? 3; // default to accessteam if null

                    ActorApprovalType t = (ActorApprovalType) agt;

                    stateTransition(t, wf, WorkflowState.Not_Active, WorkflowState.Pending_Approval);
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
                            // TODO - need to update state.dueDate 
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

        private void completeRequestApprovalCheck(SNAPDatabaseDataContext db)
        {
            var accessWF = FindApprovalTypeWF(db, (byte) ActorApprovalType.Workflow_Admin);
            var wfs = FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
            var totalApproved = 0;
            var state = accessWF[0].SNAP_Workflow_States.Single(s=>s.workflowStatusEnum==(byte)WorkflowState.Pending_Approval 
                && s.completedDate == null); // get lastest 'pending approval' for the workflowadmin state

            foreach (var wf in wfs)
            {
                var cnt = wf.SNAP_Workflow_States.Count(
                    s => s.workflowStatusEnum == (byte)WorkflowState.Approved
                    && s.completedDate != null
                    && s.pkId >= state.pkId); // only check approval for the latest iteration, ignore previously approved interateion

                totalApproved += cnt;
            }

            if (totalApproved == wfs.Count)
            {
                //var accessTeamWF = FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin)[0];
                stateTransition(ActorApprovalType.Workflow_Admin, accessWF[0], WorkflowState.Pending_Approval, WorkflowState.Approved);
            }

        }

        private void stateTransition(ActorApprovalType approvalType, SNAP_Workflow wf, WorkflowState from, WorkflowState to)
        {
            //
            // !!! if the state completion date is null, it is the WF current state !!!
            //

            // a brand new WF has no coming from state, such as when creating a new manager, team and techical approval workflow
            if (from != WorkflowState.Not_Active)
            {
                // complete current/prev state

                var prevWFState = wf.SNAP_Workflow_States.Single(
                    s => s.workflowStatusEnum == (byte)from
                    && s.completedDate == null); // To prevent looping in old state, null date is the latest state

                if (prevWFState != null)
                    prevWFState.completedDate = DateTime.Now;
                
            }

            // create new state
            var newState = new SNAP_Workflow_State()
            {
                completedDate = null,
                notifyDate = DateTime.Now,
                dueDate = getDueDate(approvalType, from, to), //DateTime.Now.AddDays(1),
                workflowStatusEnum = (byte)to
            };

            // for end/close states set the completion date
            checkToCloseWorkflowAdimStates(approvalType, to, newState);

            // workflowstate.approved is end state for manger, team approval and techical aproval but not for workflow adim
            checkToCloseMangerOrTeamOrTechnicalWorkflowStates(approvalType, to, newState);

            // go to new state
            wf.SNAP_Workflow_States.Add(newState);
        }

        private void checkToCloseMangerOrTeamOrTechnicalWorkflowStates(ActorApprovalType approvalType, WorkflowState to, SNAP_Workflow_State newState)
        {
            if (approvalType != ActorApprovalType.Workflow_Admin)
            {
                if (to == WorkflowState.Approved || to == WorkflowState.Closed_Denied)
                {
                    newState.completedDate = newState.dueDate = DateTime.Now;
                }
            }
        }

        private void checkToCloseWorkflowAdimStates(ActorApprovalType approvalType, WorkflowState to, SNAP_Workflow_State newState)
        {
            if (approvalType == ActorApprovalType.Workflow_Admin)
            {
                if (to == WorkflowState.Closed_Cancelled || to == WorkflowState.Closed_Completed ||
                    to == WorkflowState.Closed_Denied)
                {
                    newState.completedDate = newState.dueDate = DateTime.Now;
                }
            }
        }

        private DateTime getDueDate(ActorApprovalType approvalType, WorkflowState fr, WorkflowState to)
        {
            return DateTime.Now.AddDays(1);
        }

        #endregion
    }

}
