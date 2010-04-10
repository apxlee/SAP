using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;

using System.Text;
using System.Transactions;
using Apollo.AIM.SNAP.Model;
using System.Collections.Specialized;
using System.Configuration;
using Apollo.CA.Logging;


namespace Apollo.AIM.SNAP.Model
{

    #region AccessReqeust class

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
                Logger.Error("SNAP - AccessRequst: Ack failed", ex);
            }

            return result;
        }

        public bool AddComment(string comment, CommentsType type)
        {
            var result = false;
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    req.SNAP_Request_Comments.Add(new SNAP_Request_Comment()
                                                      {
                                                          commentText = comment,
                                                          commentTypeEnum = (byte)type,
                                                          createdDate = DateTime.Now
                                                       });
                    db.SubmitChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SNAP - AccessRequst: AddComment failed", ex);
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
                Logger.Error("SNAP - AccessRequst: No access failed", ex);
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
                        Email.UpdateRequesterStatus(req.submittedBy, req.userDisplayName, _id, WorkflowState.Change_Requested, comment);
                        db.SubmitChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error("SNAP - AccessRequst: Request To Change", ex);
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

                Logger.Error("SNAP - AccessRequst: Request Changed", ex);
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
                        result = mustHaveManagerInWorkflow(db, actorIds);
                        if (result)
                        {
                            createrApprovalWorkFlow(db, actorIds);
                            db.SubmitChanges();
                        }
                    }

                }

                InformApproverForAction();
            }
            catch (Exception ex) {
                Logger.Error("SNAP - AccessRequst: Create Workflow", ex);
                result = false;
            }
            return result;
        }

        private bool mustHaveManagerInWorkflow(SNAPDatabaseDataContext db,  List<int> actorIds)
        {
            var result = false;
            foreach (var i in actorIds)
            {
                if (db.SNAP_Actors.Single(a=>a.pkId == i).SNAP_Actor_Group.actorGroupType == (byte) ActorApprovalType.Manager)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public List<SNAP_Workflow> FindApprovalTypeWF(SNAPDatabaseDataContext db, int wfType)
        {
            List<SNAP_Workflow> wfList = new List<SNAP_Workflow>();
            var wfs = db.SNAP_Workflows.Where(w => w.requestId == _id);
            foreach (var wf in wfs)
            {
                var t = WorkflowApprovalType(db, wf.pkId);
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
            ApprovalWorkflow wf = ApprovalWorkflow.CreateApprovalWorkflow(wid);
            if (wf == null)
                return false;

            switch (action)
            {
                case WorkflowAction.Approved : return wf.Approve();
                case WorkflowAction.Change : return wf.RequestToChange(comment);
                case WorkflowAction.Denied : return wf.Deny(comment);
            }

            return false;
        }
        //public bool WorkflowAck(int wid, WorkflowAction action, string comment)
        //{
        //    var result = false;
        //    int approvalType;


        //    using (var db = new SNAPDatabaseDataContext())
        //    {
        //       var req = db.SNAP_Requests.Single(r => r.pkId == _id);
        //       if (req.statusEnum != (byte)RequestState.Pending)
        //           return result;
                    
        //        System.Data.Common.DbTransaction trans = null;
        //        try
        //        {
        //            db.Connection.Open();
        //            trans = db.Connection.BeginTransaction();
        //            db.Transaction = trans;

        //            approvalType = WorkflowApprovalType(db, wid);
        //            var wf = db.SNAP_Workflows.Single(w => w.pkId == wid);
        //            var currentState = wf.SNAP_Workflow_States.Single(s => s.completedDate == null);

        //            WorkflowState newState = WorkflowState.Not_Active;
        //            handleApproval(action, (ActorApprovalType)approvalType, ref newState);
        //            /*
        //            switch ((ActorApprovalType)approvalType)
        //            {
        //                case ActorApprovalType.Manager:
        //                case ActorApprovalType.Team_Approver:
        //                    handleApproval(action, (ActorApprovalType)approvalType, ref newState);
        //                    break;

        //                case ActorApprovalType.Technical_Approver:
        //                    handleApproval(action, (ActorApprovalType)approvalType, ref newState);
        //                    break;

        //            }
        //            */
        //            if (currentState.workflowStatusEnum == (byte)WorkflowState.Pending_Approval)
        //            {
        //                stateTransition((ActorApprovalType) approvalType, wf, WorkflowState.Pending_Approval, newState);
        //                db.SubmitChanges();

        //                //throw new Exception("Bad!");

        //                if (approvalType == (byte) ActorApprovalType.Technical_Approver &&
        //                    action == WorkflowAction.Approved)
        //                    completeRequestApprovalCheck(db);

        //                if (action == WorkflowAction.Denied)
        //                    deny(db, (ActorApprovalType) approvalType, comment);

        //                if (action == WorkflowAction.Change)
        //                    approvalRequestToChange(db, wf, comment);

        //                db.SubmitChanges();
        //                trans.Commit();
        //                result = true;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //Logger.Error(ex);
        //            if (trans != null)
        //                trans.Rollback();
        //            throw ex;
        //        }
        //        finally
        //        {
        //            //db.SubmitChanges();
                    
        //            if (db.Connection.State == ConnectionState.Open) 
        //                db.Connection.Close();

        //        }
        //        return result; 
        //    }
        //}

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
                        var changeRequest = new ServiceDesk.ChangeRequest(Apollo.ServiceDesk.SDConfig.Instance.Login, Apollo.ServiceDesk.SDConfig.Instance.Password);

                        changeRequest.CategoryName = "Server.Systems.Privileged Access";

                        changeRequest.Submitter.Get("svc_Cap");
                        changeRequest.AffectedUser.Get(req.userId);  // req.userId???

                        changeRequest.Attributes["description"] = requestDescription;

                        changeRequest.Create();


                        req.ticketNumber = changeRequest.Number;

                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SNAP - AccessRequst: Create Service Desk Ticket", ex); 
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
                        Email.UpdateRequesterStatus(req.submittedBy, req.userDisplayName, _id, WorkflowState.Closed_Completed, string.Empty);
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SNAP - AccessRequst: Finalize Request", ex); 
            }

            return result;
        }

#endregion

        #region private methods

        private string requestDescription
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                using (var db = new SNAPDatabaseDataContext())
                {
                    var requestTexts = db.RetrieveRequestUserText(_id);

                    foreach (var text in requestTexts)
                    {
                        sb.AppendLine(text.userText);
                        sb.AppendLine("");
                    }
                }

                return sb.ToString();
            }
        }

        private bool reqStateTransition(SNAP_Request req, RequestState reqFr, RequestState reqTo, SNAP_Workflow accessTeamWF,  WorkflowState wfFr, WorkflowState wfTo)
        {
            var result = false;
            if (req.statusEnum == (byte)reqFr)
            {
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

        /*
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

        */
        /*
        private void createrApprovalWorkFlow(SNAPDatabaseDataContext db, List<int> actorIds)
        {
            var req = db.SNAP_Requests.Single(x => x.pkId == _id);

            

            //if (req.SNAP_Workflows.Count == 1 && req.SNAP_Workflows[0].actorId == 1) // brand new wf
            //{
                foreach (var actId in actorIds)
                {

                    SNAP_Workflow wf;
                    if (req.SNAP_Workflows.Count(w => w.actorId == actId) == 0) // 
                    {
                        wf = new SNAP_Workflow() {actorId = actId};
                        req.SNAP_Workflows.Add(wf);
                    }
                    else
                    {
                        wf = req.SNAP_Workflows.Single(w => w.actorId == actId);
                            // wf already exists due to change request
                    }

                    var actor = db.SNAP_Actors.Single(a => a.pkId == actId);
                    var agt = actor.SNAP_Actor_Group.actorGroupType ?? 3; // default to accessteam if null

                    ActorApprovalType t = (ActorApprovalType) agt;

                    stateTransition(t, wf, WorkflowState.Not_Active, WorkflowState.Pending_Approval);

                }
            //}

        }
*/

        private void createrApprovalWorkFlow(SNAPDatabaseDataContext db, List<int> actorIds)
        {
            var req = db.SNAP_Requests.Single(x => x.pkId == _id);

            
            var orgActorList = new List<int>();
            var toDeleteActorList = new List<int>();
            List<SNAP_Workflow_State> toDeleteWFStates = new List<SNAP_Workflow_State>();
            if (req.SNAP_Workflows.Count > 1)
            {
                foreach(var wf in req.SNAP_Workflows)
                {
                    if (wf.actorId != 1)
                        orgActorList.Add(wf.actorId);
                }

                toDeleteActorList = orgActorList.Except(actorIds).ToList();
                foreach (var i in toDeleteActorList)
                {
                    var wf = req.SNAP_Workflows.Single(w => w.actorId == i);
                    toDeleteWFStates.AddRange(wf.SNAP_Workflow_States);
                    db.SNAP_Workflow_States.DeleteAllOnSubmit(toDeleteWFStates);
                    db.SNAP_Workflows.DeleteOnSubmit(wf);
                    db.SNAP_Workflow_Comments.DeleteAllOnSubmit(wf.SNAP_Workflow_Comments);
                    //req.SNAP_Workflows.Remove(wf);
                }

                
            }
             

            //if (req.SNAP_Workflows.Count == 1 && req.SNAP_Workflows[0].actorId == 1) // brand new wf
            //{
            foreach (var actId in actorIds)
            {

                SNAP_Workflow wf;
                if (req.SNAP_Workflows.Count(w => w.actorId == actId) == 0) // 
                {
                    wf = new SNAP_Workflow() { actorId = actId };
                    req.SNAP_Workflows.Add(wf);
                }
                else
                {
                    wf = req.SNAP_Workflows.Single(w => w.actorId == actId);
                    // wf already exists due to change request
                }

                var actor = db.SNAP_Actors.Single(a => a.pkId == actId);
                var agt = actor.SNAP_Actor_Group.actorGroupType ?? 3; // default to accessteam if null

                ActorApprovalType t = (ActorApprovalType)agt;

                stateTransition(t, wf, WorkflowState.Not_Active, WorkflowState.Pending_Approval);

            }
            //}

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
                            Email.TaskAssignToApprover(state.SNAP_Workflow.SNAP_Actor.emailAddress, state.SNAP_Workflow.SNAP_Actor.displayName, _id, state.SNAP_Workflow.SNAP_Request.userDisplayName);
                            state.notifyDate = DateTime.Now;
                            var actorType = (ActorApprovalType) (wf.SNAP_Actor.SNAP_Actor_Group.actorGroupType ?? 3); // default workflow admin
                            state.dueDate = getDueDate(actorType, WorkflowState.Pending_Approval, WorkflowState.Pending_Workflow);
                            done = true;
                        }
                    }
                }
            }

            return done;
        }

        internal static int WorkflowApprovalType(SNAPDatabaseDataContext db, int wid)
        {

            var wf = db.SNAP_Workflows.Single(w => w.pkId == wid);

            if (wf.SNAP_Actor.actor_groupId == 0)
                return (byte)ActorApprovalType.Workflow_Admin;

            return wf.SNAP_Actor.SNAP_Actor_Group.actorGroupType ?? -1;

        }

        /*
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
        */

        internal static void stateTransition(ActorApprovalType approvalType, SNAP_Workflow wf, WorkflowState from, WorkflowState to)
        {
            //
            // !!! if the state completion date is null, it is the WF current state !!!
            //

            // a brand new WF has no coming from state, such as when creating a new manager, team and techical approval workflow

            var prevDueDate = DateTime.Now;
            SNAP_Workflow_State prevWFState=null;
            if (from != WorkflowState.Not_Active)
            {
                // complete current/prev state

                prevWFState = wf.SNAP_Workflow_States.Single(
                    s => s.workflowStatusEnum == (byte)from
                    && s.completedDate == null); // To prevent looping in old state, null date is the latest state

                if (prevWFState != null)
                {
                    prevWFState.completedDate = DateTime.Now;
                    prevDueDate = prevWFState.dueDate ?? DateTime.Now;

                }

            }

            // create new state
            var newState = new SNAP_Workflow_State()
            {
                completedDate = null,
                //notifyDate = DateTime.Now,
                dueDate = getDueDate(approvalType, from, to), 
                workflowStatusEnum = (byte)to
            };

            // we don't need to notify workflow admin/access team. they have to monitor the open request constantly
            if (approvalType == ActorApprovalType.Workflow_Admin)
                newState.notifyDate = DateTime.Now;

            if (prevWFState != null)
            {
                newState.notifyDate = prevWFState.notifyDate; // just propergate the notify date to new state...it is primarily for approval manager
            }

            // for end/close states set the completion date
            checkToCloseWorkflowAdimStates(approvalType, to, newState, prevDueDate);

            // workflowstate.approved is end state for manger, team approval and techical aproval but not for workflow adim
            checkToCloseMangerOrTeamOrTechnicalWorkflowStates(approvalType, to, newState, prevDueDate);

            // go to new state
            wf.SNAP_Workflow_States.Add(newState);
        }

        private static void checkToCloseMangerOrTeamOrTechnicalWorkflowStates(ActorApprovalType approvalType, WorkflowState to, SNAP_Workflow_State newState, DateTime dueDate)
        {
            if (approvalType != ActorApprovalType.Workflow_Admin)
            {
                if (to == WorkflowState.Approved || to == WorkflowState.Closed_Denied)
                {
                    newState.completedDate = DateTime.Now;
                    newState.dueDate = dueDate;
                }
            }
        }

        private static void checkToCloseWorkflowAdimStates(ActorApprovalType approvalType, WorkflowState to, SNAP_Workflow_State newState, DateTime dueDate)
        {
            if (approvalType == ActorApprovalType.Workflow_Admin)
            {
                if (to == WorkflowState.Closed_Cancelled || to == WorkflowState.Closed_Completed ||
                    to == WorkflowState.Closed_Denied)
                {
                    newState.completedDate = DateTime.Now;
                    newState.dueDate = dueDate;
                }
            }
        }

        private static DateTime getDueDate(ActorApprovalType approvalType, WorkflowState fr, WorkflowState to)
        {
            int day = 1;
            DateTime due;
            SLAConfiguration slaCfg = new SLAConfiguration((NameValueCollection)ConfigurationManager.GetSection(
                "Apollo.AIM.SNAP/Workflow.SLA"));


            switch (approvalType)
                {
                    case ActorApprovalType.Manager:
                        day = System.Convert.ToInt16(slaCfg.ManagerApprovalInDays);
                        break;
                    case ActorApprovalType.Team_Approver:
                        day = System.Convert.ToInt16(slaCfg.TeamApprovalInDays);
                        break;
                    case ActorApprovalType.Technical_Approver:
                        day = System.Convert.ToInt16(slaCfg.TechnicalApprovalInDays);
                        break;
                    case ActorApprovalType.Workflow_Admin:
                        if (to == WorkflowState.Pending_Approval) // same as the due day for technical approval since workflow admin depend on it
                            day = System.Convert.ToInt16(slaCfg.TechnicalApprovalInDays);
                        break;
                }

                using (var db = new SNAPDatabaseDataContext())
                {
                    due = db.udf_get_next_business_day(DateTime.Now, day) ?? DateTime.Now.AddDays(-1);
                }


            return due;
        }

        #endregion
    }

    #endregion


    #region  ApprovalWorkflow class

    public abstract class  ApprovalWorkflow
    {
        public static ApprovalWorkflow CreateApprovalWorkflow(int wfId)
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var t = AccessRequest.WorkflowApprovalType(db, wfId);
                if (t != -1)
                {
                    switch ((ActorApprovalType) t)
                    {
                        case ActorApprovalType.Manager:
                            return new ManagerApprovalWorkflow(wfId);
                        case ActorApprovalType.Team_Approver:
                            return new TeamApprovalWorkflow(wfId);
                        case ActorApprovalType.Technical_Approver:
                            return new TechnicalApprovalWorkflow(wfId);
                    }
                }

                return null;
            }
        }

        public static List<AccessApprover> GetAvailableApprovers()
        {
            List<AccessApprover> approverList = new List<AccessApprover>();

            using (var db = new SNAPDatabaseDataContext())
            {
                var query = from sa in db.SNAP_Actors
                            join sag in db.SNAP_Actor_Groups on sa.actor_groupId equals sag.pkId
                            where sa.isActive == true && sag.isActive == true && sag.actorGroupType < 2
                            orderby sag.actorGroupType, sag.groupName ascending
                            select new
                            {
                                ActorId = sa.pkId,
                                UserId = sa.userId,
                                DisplayName = sa.displayName,
                                IsDefault = sa.isDefault,
                                GroupId = sag.pkId,
                                GroupName = sag.groupName,
                                Description = sag.description,
                                ActorGroupType = sag.actorGroupType
                            };
                foreach (var approver in query)
                {
                    AccessApprover newApprover = new AccessApprover();
                    newApprover.ActorId = approver.ActorId;
                    newApprover.UserId = approver.UserId;
                    newApprover.DisplayName = approver.DisplayName;
                    newApprover.IsDefault = approver.IsDefault;
                    newApprover.GroupId = approver.GroupId;
                    newApprover.GroupName = approver.GroupName;
                    newApprover.Description = approver.Description;
                    newApprover.ActorApprovalType = (ActorApprovalType)approver.ActorGroupType;
                    approverList.Add(newApprover);
                }

            }
            
            return approverList;
        }

        public static List<AccessApprover> GetRequestApprovers(int requestId)
        {
            List<AccessApprover> approverList = new List<AccessApprover>();

            using (var db = new SNAPDatabaseDataContext())
            {
                var query = from sw in db.SNAP_Workflows
                            join sws in db.SNAP_Workflow_States on sw.pkId equals sws.workflowId
                            where sw.requestId == requestId
                            group sw by new { sw.requestId, sw.actorId } into wfGroup
                            select new
                            {
                                ActorId = wfGroup.Key.actorId,
                                WorkflowId = wfGroup.Max(sw => sw.pkId) 
                            };

                foreach (var approver in query)
                {
                    AccessApprover newApprover = new AccessApprover();
                    newApprover.ActorId = approver.ActorId;
                    newApprover.WorkflowState = (WorkflowState)GetWorkflowState(approver.WorkflowId);
                    approverList.Add(newApprover);
                }
            }

            return approverList;
        }

        public static byte GetWorkflowState(int workflowId)
        {
            byte state = 0;

            using (var db = new SNAPDatabaseDataContext())
            {
                var result = db.SNAP_Workflow_States
                             .Where(u=>u.workflowId == workflowId)
                             .OrderByDescending(o=>o.pkId)
                             .Select(s=>s.workflowStatusEnum)
                             .First();

                state = (byte)result;
            }

            return state;
        }

        public int Id { get; set; }

        protected SNAPDatabaseDataContext db;
        protected SNAP_Request req;
        protected AccessRequest accessReq;
        protected SNAP_Workflow wf;
        protected SNAP_Workflow accessTeamWF;

        protected ApprovalWorkflow() {}
        protected ApprovalWorkflow(int id)
        {
            Id = id;
            db = new SNAPDatabaseDataContext();

            wf = db.SNAP_Workflows.Single(w => w.pkId == Id);
            req = wf.SNAP_Request;
            accessReq = new AccessRequest(req.pkId);
            accessTeamWF = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin)[0];
        }

        protected bool approveAndInformOhterSequentialManager()
        {
            if (req.statusEnum != (byte)RequestState.Pending)
                return false;

            //using (TransactionScope ts = new TransactionScope())
            //{
                if (wfStateChange(ActorApprovalType.Manager, WorkflowState.Approved))
                {
                    informApproverForAction();
                    //db.SubmitChanges();
                    //ts.Complete();
                    return true;
                }
            //}

            return false;

        }

        protected bool wfStateChange(ActorApprovalType approvalType, WorkflowState toState)
        {
            var wf = db.SNAP_Workflows.Single(w => w.pkId == Id);

            if (wf.SNAP_Request.statusEnum != (byte)RequestState.Pending)
                return false;

            var currentState = wf.SNAP_Workflow_States.Single(s => s.completedDate == null);

            AccessRequest.stateTransition(approvalType, wf, (WorkflowState)currentState.workflowStatusEnum, toState);

            // !!! because of this we are force to use TransactionScope
            // !!! so we can make sure every technical approver has approved which moves the request approved state
            db.SubmitChanges(); 

            return true;
        }

        protected void informApproverForAction()
        {
            accessReq.InformApproverForAction();
        }

        protected void completeRequestApprovalCheck()
        {
            var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
            var totalApproved = 0;
            var state = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval
                && s.completedDate == null); // get lastest 'pending approval' for the workflowadmin state

            foreach (var w in wfs)
            {
                var cnt = w.SNAP_Workflow_States.Count(
                    s => s.workflowStatusEnum == (byte)WorkflowState.Approved
                    && s.completedDate != null
                    && s.pkId >= state.pkId); // only check approval for the latest iteration, ignore previously approved interateion

                totalApproved += cnt;
            }

            if (totalApproved == wfs.Count)
            {
                AccessRequest.stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF, WorkflowState.Pending_Approval, WorkflowState.Approved);
            }

        }

        protected bool requestToChangeBy(ActorApprovalType approvalType, string comment)
        {

            if (req.statusEnum != (byte)RequestState.Pending)
                return false;

            using (TransactionScope ts = new TransactionScope())
            {
                if (wfStateChange(approvalType, WorkflowState.Change_Requested))
                {
                    approvalRequestToChange(comment);
                    db.SubmitChanges();
                    ts.Complete();
                    return true;
                }
            }
            return false;


        }


        protected void approvalRequestToChange(string comment)
        {
            AccessRequest.stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF, WorkflowState.Pending_Approval, WorkflowState.Change_Requested);
            //db.SubmitChanges();

            //var req = db.SNAP_Requests.Single(w => w.pkId == _id);
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
            Email.UpdateRequesterStatus(req.submittedBy, req.userDisplayName, req.pkId, WorkflowState.Change_Requested, comment);
        }

        protected bool denyBy(ActorApprovalType approvalType, string comment)
        {

            if (req.statusEnum != (byte)RequestState.Pending)
                return false;

            using (TransactionScope ts = new TransactionScope())
            {

                if (wfStateChange(approvalType, WorkflowState.Closed_Denied))
                {
                    approvalDeny(approvalType, comment);
                    db.SubmitChanges();
                    ts.Complete();
                    return true;
                }

            }
            return false;

        }

        protected void approvalDeny(ActorApprovalType type, string comment)
        {
            var wfs = accessReq.FindApprovalTypeWF(db, (byte)type);
            foreach (var f in wfs)
            {
                if (f.SNAP_Workflow_States.Count(s => s.workflowStatusEnum == (byte)WorkflowState.Closed_Denied) == 1)
                {
                    f.SNAP_Workflow_Comments.Add(new SNAP_Workflow_Comment()
                    {
                        commentText = comment,
                        createdDate = DateTime.Now,
                        commentTypeEnum = (byte)CommentsType.Denied
                    });
                    // set accessTeam WF and request to close-denied
                    AccessRequest.stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF, WorkflowState.Pending_Approval, WorkflowState.Closed_Denied);
                    req.statusEnum = (byte)RequestState.Closed;
                    Email.UpdateRequesterStatus(req.submittedBy, req.userDisplayName, req.pkId, WorkflowState.Closed_Denied, comment);
                    break;
                }

            }
        }


        public abstract bool Approve();
        public abstract bool Deny(string comment);
        public abstract bool RequestToChange(string comment);
    }

    public class ManagerApprovalWorkflow : ApprovalWorkflow
    {
        public ManagerApprovalWorkflow(int id) : base(id){}

        public override bool Approve()
        {
            return approveAndInformOhterSequentialManager();
        }

        public override bool Deny(string comment)
        {
            return denyBy(ActorApprovalType.Manager, comment);
        }

        public override bool RequestToChange(string comment)
        {
            return requestToChangeBy(ActorApprovalType.Manager, comment);

        }
    }


    public class TeamApprovalWorkflow : ApprovalWorkflow
    {
        public TeamApprovalWorkflow(int id): base(id){}

        public override bool Approve()
        {
            return approveAndInformOhterSequentialManager();
        }

        public override bool Deny(string comment)
        {
            return denyBy(ActorApprovalType.Team_Approver, comment);
            
        }

        public override bool RequestToChange(string comment)
        {
            return requestToChangeBy(ActorApprovalType.Team_Approver, comment);
        }

    }

    public class TechnicalApprovalWorkflow : ApprovalWorkflow
    {
        public TechnicalApprovalWorkflow(int id): base(id){}

        public override bool Approve()
        {
            if (req.statusEnum != (byte)RequestState.Pending)
                return false;

            using (TransactionScope ts = new TransactionScope())
            {
                if (wfStateChange(ActorApprovalType.Technical_Approver, WorkflowState.Approved))
                {
                    completeRequestApprovalCheck();
                    db.SubmitChanges();
                    ts.Complete();
                    return true;
                }
            }

            return false;
        }

        public override bool Deny(string comment)
        {
            return denyBy(ActorApprovalType.Technical_Approver, comment);
        }

        public override bool RequestToChange(string comment)
        {
            return requestToChangeBy(ActorApprovalType.Technical_Approver, comment);
        }
    }

#endregion

}
