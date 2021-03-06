﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;

using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using Apollo.AIM.SNAP.CA;
using Apollo.AIM.SNAP.Model;
using System.Collections.Specialized;
using System.Configuration;
using Apollo.CA.Logging;
using Apollo.HPServiceManager;

namespace Apollo.AIM.SNAP.Model
{

    #region AccessReqeust class

    public class AccessRequest
    {
        public const string SeeDetailsReqTracking = "Incorrect State. Please see details in Request Tracking. ";

        private int _id;
        private static int AccessTeamActorId = 1;

        private List<int> deletedActors, addedActors;

        public AccessRequest(int i)
        {
            _id = i;
            deletedActors = new List<int>();
            addedActors = new List<int>();
        }

        #region public methods

        public WebMethodResponse Ack()
        { 
            WebMethodResponse resp = new WebMethodResponse() ;
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    
                    SNAP_Workflow accessTeamWF;
                    DateTime? dueDate;
                    SNAP_Request req;

                    initializeData(db, WorkflowState.Pending_Acknowledgement, out req, out accessTeamWF, out dueDate);
                    
                    var result = reqStateTransition(req, RequestState.Open, RequestState.Pending,
                                                accessTeamWF, WorkflowState.Pending_Acknowledgement,
                                                WorkflowState.Pending_Workflow);

                    if (result)
                    {
						addAccessTeamComment(accessTeamWF, "Due Date: " + Convert.ToDateTime(dueDate).ToString("MMM d, yyyy"), CommentsType.Acknowledged);
                        db.SubmitChanges();
                        resp = new WebMethodResponse(true, "Acknowledgement", "Success");
                    }
                    else
                    {
                        resp = new WebMethodResponse(false, "Acknowledgement Error", SeeDetailsReqTracking);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("AccessRequest - Ack, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                resp = new WebMethodResponse(false, "Acknowledgement Exception", ex.Message);
            }

            return resp;
        }

        public WebMethodResponse AddComment(string comment, CommentsType type)
        {
            WebMethodResponse resp = new WebMethodResponse();
			comment = comment.Replace("<br />", string.Empty);
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
                    resp = new WebMethodResponse(true, "Add Comment", "Success");
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("AccessRequest - AddComment, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                resp = new WebMethodResponse(false, "Add Comment Exception", ex.Message);
            }
            return resp;
        }

        public WebMethodResponse NoAccess(WorkflowAction action, string comment)
        {
            WebMethodResponse resp = new WebMethodResponse();

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
                case WorkflowAction.Abandon:
                    wfState = WorkflowState.Closed_Abandon;
                    commentType = CommentsType.Abandon;
                    break;

            }

            try
            {
                
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
                    var currentAccessTeamWFState = (WorkflowState) accessTeamWF.SNAP_Workflow_States.Single(s => s.completedDate == null).workflowStatusEnum;
                    var result = reqStateTransition(req, RequestState.Pending, RequestState.Closed,
                                                accessTeamWF, currentAccessTeamWFState, /* WorkflowState.Pending_Workflow,*/
                                                wfState)
                            || 
                            reqStateTransition(req, RequestState.Change_Requested, RequestState.Closed,
                                                accessTeamWF, currentAccessTeamWFState, /* WorkflowState.Pending_Workflow,*/
                                                wfState);

                    if (result)
                    {
						comment = comment.Replace("<br />", string.Empty);
                        addAccessTeamComment(accessTeamWF, comment, commentType);

						if (req.submittedBy.ToString() != req.userId.ToString())
						{
							Email.SendTaskEmail(EmailTaskType.UpdateRequester, req.userId + "@apollogrp.edu", req.userDisplayName, _id, req.submittedBy, (WorkflowState)wfState, comment);
						}
						Email.SendTaskEmail(EmailTaskType.UpdateRequester, req.submittedBy + "@apollogrp.edu", Utilities.GetFullNameByLoginId(req.submittedBy), _id, req.submittedBy, (WorkflowState)wfState, comment);
                        db.SubmitChanges();
                        resp = new WebMethodResponse(true, "Cancel/Deny", "Success");
                    }
                    else
                    {
                        resp = new WebMethodResponse(false, "Cancel/Deny Error", SeeDetailsReqTracking); 
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("AccessRequest - NoAccess, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                resp = new WebMethodResponse(false, "Cancel/Deny Exception", ex.Message);
            }
            return resp;

        }


        public WebMethodResponse RequestToChange(int actorId, string comment)
        {
            try
            {
                if (actorId == AccessTeamActorId)
                {
                    return RequestToChange(comment);
                }
                else
                {
                    int wid = 0;
                    using (var db = new SNAPDatabaseDataContext())
                    {
                        var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                        wid = req.SNAP_Workflows.Single(w => w.actorId == actorId).pkId; // TODO: can one actor plays multiple roles such as manager and technical approver, if so, this will not work
                    }
                    return WorkflowAck(wid, WorkflowAction.Change);
                }
            }
            catch(Exception ex)
            {
                Logger.Error("SNAP - AccessRequst: Request To Change", ex);
                return new WebMethodResponse(false, "Request Change Exception", ex.Message);
            }
        }


        public WebMethodResponse RequestToChange(string comment)
        {
            WebMethodResponse resp = new WebMethodResponse();

            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
                    var result = reqStateTransition(req, RequestState.Pending, RequestState.Change_Requested,
                                                accessTeamWF, WorkflowState.Pending_Workflow,
                                                WorkflowState.Change_Requested);

					comment = comment.Replace("<br />", string.Empty); 

                    if (result)
                    {
                        addAccessTeamComment(accessTeamWF, comment, CommentsType.Requested_Change);
						// NOTE: don't send AIM extra email when AIM is the one making the change.
						if (req.submittedBy.ToString() != req.userId.ToString())
						{
							Email.SendTaskEmail(EmailTaskType.UpdateRequester, req.userId + "@apollogrp.edu", req.userDisplayName, _id, req.submittedBy, WorkflowState.Change_Requested, comment);
						}

						Email.SendTaskEmail(EmailTaskType.UpdateRequester, req.submittedBy + "@apollogrp.edu", Utilities.GetFullNameByLoginId(req.submittedBy), _id, req.submittedBy, WorkflowState.Change_Requested, comment);
						
                        db.SubmitChanges();
                        resp = new WebMethodResponse(true, "Request Change", "Success");
                    }
                    else
                    {
                        resp = new WebMethodResponse(false, "Request Change Error", SeeDetailsReqTracking); 
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("AccessRequest - RequestToChange, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                resp = new WebMethodResponse(false, "Request Change Exception", ex.Message);
            }

            return resp;
        }

        public WebMethodResponse RequestChanged()
        {
            WebMethodResponse resp = new WebMethodResponse();
             
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
                    var result = reqStateTransition(req, RequestState.Change_Requested, RequestState.Open,
                                                accessTeamWF, WorkflowState.Change_Requested,
                                                WorkflowState.Pending_Acknowledgement);

                    if (result)
                    {
                        db.SubmitChanges();
                        resp = new WebMethodResponse(true, "Request Changed", "Success");
                    }
                    else
                    {
                        resp = new WebMethodResponse(false, "Request Changed Error", SeeDetailsReqTracking);
                    }


                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("AccessRequest - RequestChanged, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                resp = new WebMethodResponse(false, "Request Changed Exception", ex.Message);
            }

            return resp;
        }

        // call this method when one create the workflow the first time of after request to change
        // add mgr into the list of actorid and create the wf after that
        public WebMethodResponse CreateWorkflow(string mgrusrId, List<int> actorIDs)
        {
            var mgrActorId = ApprovalWorkflow.GetActorIdByUserId(ActorGroupType.Manager, mgrusrId);
            if ( mgrActorId != 0 )
            {
                actorIDs.Add(mgrActorId);
                return CreateWorkflow(actorIDs);
            }
            
            return new WebMethodResponse(false, "Workflow Creation", "Manager Approver Not in AD. Please adjust approver. ");
        }

        /// <summary>
        /// call this method when one creates the workflow the first time or after request to change
        /// </summary>
        /// <param name="actorIds"></param>
        /// <returns></returns>
        public WebMethodResponse CreateWorkflow(List<int> actorIds)
        {
            WebMethodResponse resp = new WebMethodResponse();
            var result = false;
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    
                    SNAP_Workflow accessTeamWF;
                    DateTime? dueDate;
                    SNAP_Request req; 

                    initializeData(db, WorkflowState.Pending_Workflow, out req, out accessTeamWF, out dueDate);
                    

                    result = reqStateTransition(req, RequestState.Pending, RequestState.Pending,
                                                accessTeamWF, WorkflowState.Pending_Workflow,
                                                WorkflowState.Workflow_Created);


                    if (result)
                    {
                        result = mustHaveManagerInWorkflow(db, actorIds);
                        if (result)
                        {
                            createrApprovalWorkFlow(db, actorIds);
                            addAccessTeamComment(accessTeamWF, "Due Date: " + Convert.ToDateTime(dueDate).ToString("MMM d, yyyy"), CommentsType.Workflow_Created);
                            db.SubmitChanges();
                            resp = new WebMethodResponse(true, "Workflow Creation", "Success");
                        }
                        else
                        {
                            resp = new WebMethodResponse(false, "Workflow Creation Error", "Missing Manager. Please adjust manager. ");
                        }
                    }
                    else
                    {
                        resp = new WebMethodResponse(false, "Workflow Creation Error", SeeDetailsReqTracking);
                    }

                }
                if (result) 
                    InformApproverForAction();
            }
            catch (Exception ex) 
            {
                Logger.Fatal("AccessRequest - CreateWorkflow, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                resp = new WebMethodResponse(false, "Workflow Creation Exception", ex.Message);
            }
            return resp;
        }

        /// <summary>
        /// Call this method when you have a WF in place and in pending approval stage and u want to make mod to the approver list
        /// </summary>
        /// <param name="mgrusrId"></param>
        /// <param name="actorIDs"></param>
        /// <returns></returns>
        public WebMethodResponse EditWorkflow(string mgrusrId, List<int> actorIDs)
        {
            WebMethodResponse resp = new WebMethodResponse();

            var mgrActorId = ApprovalWorkflow.GetActorIdByUserId(ActorGroupType.Manager, mgrusrId);
            if (mgrActorId != 0)
            {
                // who is in pending state, we need to remember to inform the pending approver her new due day
                var currentPendingApproverType = GetPendingApprovalActorType();
                if (currentPendingApproverType != -1)
                {
                    actorIDs.Add(mgrActorId);
                    using (var db = new SNAPDatabaseDataContext())
                    {
                        var check = editWorkflowCheck(db, actorIDs, currentPendingApproverType);
                        if (check.Success)
                        {
                            var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                            var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
                            // make sure accessTeamWF stat is pending apporal 
                            if (
                                accessTeamWF.SNAP_Workflow_States.Count(
                                    s =>
                                    s.completedDate == null &&
                                    s.workflowStatusEnum == (byte) WorkflowState.Workflow_Created) == 1)
                            {
                                var newActorIds = editApprovalWorkFlow(db, actorIDs);
                                db.SubmitChanges();

                                req = db.SNAP_Requests.Single(r => r.pkId == _id);
                                foreach (var wf in req.SNAP_Workflows)
                                {
                                    if (wf.actorId != AccessTeamActorId && newActorIds.Contains(wf.actorId))
                                    {
                                        InformNewPendingApproverNewDueDate(wf, currentPendingApproverType);

                                        // update the request table
                                        if (wf.SNAP_Actor.SNAP_Actor_Group.actorGroupType == (byte)ActorApprovalType.Manager)
                                        {
                                            req.managerUserId = wf.SNAP_Actor.userId;
                                            req.managerDisplayName = wf.SNAP_Actor.displayName;
                                        }
                                    }
                                }

                                //addAccessTeamComment(accessTeamWF, "Workflow editted @" + DateTime.Now, CommentsType.Access_Notes_AccessTeam);
                                db.SubmitChanges();

                                // in case we are removing the last approver who has not approved yet, or removing special approver
                                editWorkflowCompletionCheck(_id, db);
                                resp = new WebMethodResponse(true, "Workflow Change", "Success");
                                
                            }
                            else
                            {
                                resp = new WebMethodResponse(false, "Workflow Change Error", SeeDetailsReqTracking);
                            }
                        }
                        else
                        {
                            resp = check;
                        }
                    }
                }
            }
            return resp;
        }

        private WebMethodResponse editWorkflowCheck(SNAPDatabaseDataContext db, List<int> actorIds, int currentPendingApproverType)
        {

            if (mustHaveManagerInWorkflow(db, actorIds))
            {
                switch (currentPendingApproverType)
                {
                    case (byte) ActorApprovalType.Manager:
                        return new WebMethodResponse(true, "Workflow Change", "Success");
                    case (byte) ActorApprovalType.Team_Approver:
                        return new WebMethodResponse(true, "Workflow Change", "Success");
                    case (byte) ActorApprovalType.Technical_Approver:
                        foreach (var actorId in actorIds)
                        {
                            var act = db.SNAP_Actors.Single(a => a.pkId == actorId);
                            {
                                var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                                var approverCnt = req.SNAP_Workflows.Count(x => x.actorId == actorId);

                                // trying to add new team/speacial approver in tech approving time frame
                                if (act.SNAP_Actor_Group.actorGroupType == (byte) ActorApprovalType.Team_Approver
                                    && approverCnt == 0
                                    )
                                    return new WebMethodResponse(false, "Workflow Change Error", "Wrong time frame. ");
                            }
                        }
                        return new WebMethodResponse(true, "Workflow Change", "Success"); ;
                    default:
                        return new WebMethodResponse(false, "Workflow Change Error", "Unknown Approver Type. Pleae adjust approver. ");
                }

            }
            return new WebMethodResponse(false, "Workflow Change Error", "No Manager. Please adjust approver. ");
        }

        private void InformNewPendingApproverNewDueDate(SNAP_Workflow wf, int currentPendingApproverType)
        {
            if (wf.SNAP_Actor.SNAP_Actor_Group.actorGroupType == currentPendingApproverType)
            {
                /*
                    var state =
                        wf.SNAP_Workflow_States.Where(
                            s => s.completedDate == null && s.notifyDate == null).ToList();

                    if ((state.Count() == 1) && state[0].workflowStatusEnum == (byte) WorkflowState.Not_Active)
                    {
                 */
                // there should be only one state since we just added it
                wf.SNAP_Workflow_States[0].notifyDate = DateTime.Now;
                wf.SNAP_Workflow_States[0].completedDate = DateTime.Now;
                                           
                Email.SendTaskEmail(EmailTaskType.AssignToApprover, wf.SNAP_Actor.emailAddress, wf.SNAP_Actor.displayName, _id, wf.SNAP_Request.userDisplayName);

                stateTransition((ActorApprovalType) wf.SNAP_Actor.SNAP_Actor_Group.actorGroupType, 
                    wf,WorkflowState.Not_Active, WorkflowState.Pending_Approval);
                //}
            }
        }

        private void editWorkflowCompletionCheck(int _id, SNAPDatabaseDataContext db)
        {
            
            // assuming only on special approval
            // dropping specail approver out of the approval chain, and the manager has alread approved, move on to tech approval
            foreach (var i in deletedActors)
            {
                var act = db.SNAP_Actors.Single(a => a.pkId == i);
                {
                    if (act.SNAP_Actor_Group.actorGroupType == (byte) ActorApprovalType.Team_Approver)
                    {
                        var wfs = FindApprovalTypeWF(db, (byte) ActorApprovalType.Team_Approver);
                        if (wfs.Count == 0 && managerApprovalDone(db)) // no team/special approver
                        {
                            var techApprovers = FindApprovalTypeWF(db, (byte) ActorApprovalType.Technical_Approver);
                            emailApproverForAction(techApprovers);
                            db.SubmitChanges();
                            break;
                        }
                    }
                }
            }

            ApprovalWorkflow.CompleteRequestApprovalCheck(_id);
        }

        private bool managerApprovalDone(SNAPDatabaseDataContext db)
        {
            var wfs = FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
            if (wfs[0].SNAP_Workflow_States.Count(s=>s.workflowStatusEnum == (byte)WorkflowState.Approved) == 1)
                return true;

            return false;
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

        public static byte GetRequestState(int requestId)
        {
            byte state = 0;

            using (var db = new SNAPDatabaseDataContext())
            {
                var result = db.SNAP_Requests
                             .Where(u => u.pkId == requestId)
                             .Select(s => s.statusEnum)
                             .First();

                state = (byte)result;
            }

            return state;
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

        public WebMethodResponse WorkflowAck(int wid, WorkflowAction action)
        {
            return WorkflowAck(wid, action, string.Empty);
        }

        public WebMethodResponse WorkflowAck(int wid, WorkflowAction action, string comment)
        {
            WebMethodResponse resp = new WebMethodResponse();
            ApprovalWorkflow wf = ApprovalWorkflow.CreateApprovalWorkflow(wid);
            if (wf == null)
                return new WebMethodResponse(false, "Approver Workflow Failed", "Unknown approver");

            switch (action)
            {
                case WorkflowAction.Approved : return wf.Approve();
                case WorkflowAction.Change : return wf.RequestToChange(comment);
                case WorkflowAction.Denied : return wf.Deny(comment);
            }

            return new WebMethodResponse(false, "Approver Workflow Failed", "Unknown action");
        }

        public WebMethodResponse CreateServiceDeskTicket()
        {
            WebMethodResponse resp = new WebMethodResponse();
            try 
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    SNAP_Workflow accessTeamWF;
                    DateTime? dueDate;
                    SNAP_Request req; 
                        
                    initializeData(db, WorkflowState.Approved, out req, out accessTeamWF, out dueDate);

                    var result = reqStateTransition(req, RequestState.Pending, RequestState.Pending, accessTeamWF
						, WorkflowState.Approved, WorkflowState.Pending_Provisioning);

                    if (result)
                    {
                        var sdSource = ConfigurationManager.AppSettings["SDSource"];
                        switch (sdSource)
                        {
                            case "CASD":
                                resp = createCASDTicket(db, accessTeamWF, dueDate, req);
                                break;
                            case "HPSM" :
                                resp = createHPSMTicket(db, accessTeamWF, dueDate, req);
                                break;
                            default :
                                resp = new WebMethodResponse(false, "Ticket Creation Error", SeeDetailsReqTracking);
                                break;
                        }
                        
                    }
                    else
                    {
                        resp = new WebMethodResponse(false, "Ticket Creation Error", SeeDetailsReqTracking);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("[SNAP] AccessRequest > CreateServiceDeskTicket", ex);
                resp = new WebMethodResponse(false, "Ticket Creation Exception", ex.Message);
            }
            return resp;
        }

        private WebMethodResponse createCASDTicket(SNAPDatabaseDataContext db, SNAP_Workflow accessTeamWF, DateTime? dueDate, SNAP_Request req)
        {
            var changeRequest = new ServiceDesk.ChangeRequest(Apollo.ServiceDesk.SDConfig.Instance.Login, Apollo.ServiceDesk.SDConfig.Instance.Password);
            string updatedDescription = string.Format("Supplemental Access Process Request Id: {0}\r\nAffected End User Id: {1}\r\nRequested By: {2}\r\n-------------------------------------------------------\r\n{3}"
                , req.pkId, req.userId, req.submittedBy, requestDescription);

            if (requestDescription.Length > 5000)
            {
                return new WebMethodResponse(false, "Ticket Creation Error", "Request Content Too Long. Please split the request");
            }

            changeRequest.CategoryName = "Server.Systems.Privileged Access";
            changeRequest.Submitter.Get("svc_Cap");
            changeRequest.AffectedUser.Get(Regex.Replace(req.userId, @"^a\.", "")); // remove a. acct
            changeRequest.Attributes["description"] = updatedDescription;
            changeRequest.Create();

            req.ticketNumber = changeRequest.Number;
            var handler = changeRequest.Handle.Split(':')[1]; // chg:12345
            var sdlink = ConfigurationManager.AppSettings["SDLink"] + handler;

            addAccessTeamComment(
                accessTeamWF
                , string.Format("Due Date: {0} | Service Desk Ticket: <a target=\"_blank\" href=\"{2}\">{1}</a>"
                    , Convert.ToDateTime(dueDate).ToString("MMM d, yyyy")
                    , req.ticketNumber
                    , sdlink)
                , CommentsType.Ticket_Created);

            db.SubmitChanges();
            return new WebMethodResponse(true, "Ticket Creation", "Success");

        }

        private WebMethodResponse createHPSMTicket(SNAPDatabaseDataContext db, SNAP_Workflow accessTeamWF, DateTime? dueDate, SNAP_Request req)
        {
            string updatedDescription = string.Format("Supplemental Access Process Request Id: {0}\r\nAffected End User Id: {1}\r\nRequested By: {2}\r\n-------------------------------------------------------\r\n{3}"
                , req.pkId, req.userId, req.submittedBy, requestDescription);

            Quote q = new Quote();
            q.Category = "apollo request";
            q.Subcategory = "Employee Management Support Service";
            q.Priority = "4 - R3 Regular";
            q.RequestedEndDate = dueDate ?? DateTime.Now.AddDays(1);
            q.CurrentPhase = "Working";
            q.ServiceContact = Regex.Replace(req.userId, @"^a\.", "");
            q.PrimaryContact = Regex.Replace(req.submittedBy, @"^a\.", "");
            q.Description = new List<string>() {updatedDescription};
            q.PartNumber = "ag1144"; //Server Privileged Access - Add, modify, or remove
            q.ItemCount = "1";
            q.ItemQuantity = "1";
            q.SaveTicket(); // uncomment this line to do end-end test

            req.ticketNumber = q.RequestID;

            var hpsmlink = ConfigurationManager.AppSettings["HPSMLink"];

            addAccessTeamComment(
                accessTeamWF
                , string.Format("Due Date: {0} | Service Desk Ticket: <a target=\"_blank\" href=\"{2}\">{1}</a>"
                    , Convert.ToDateTime(dueDate).ToString("MMM d, yyyy")
                    , req.ticketNumber
                    , hpsmlink)
                , CommentsType.Ticket_Created);

            db.SubmitChanges();
            return new WebMethodResponse(true, "Ticket Creation", "Success"); ;   
        }

        private void initializeData(SNAPDatabaseDataContext db, WorkflowState state, out SNAP_Request req, out SNAP_Workflow accessTeamWF, out DateTime? dueDate)
        {
            req = db.SNAP_Requests.Single(r => r.pkId == _id);
            accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
            dueDate = accessTeamWF.SNAP_Workflow_States.Single(
                s =>
                s.completedDate == null &&
                s.workflowStatusEnum == (byte)state).dueDate;
        }

        public WebMethodResponse FinalizeRequest()
        {
            WebMethodResponse resp = new WebMethodResponse();
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(r => r.pkId == _id);
                    var accessTeamWF = req.SNAP_Workflows.Single(x => x.actorId == AccessTeamActorId);
                    var result = reqStateTransition(req, RequestState.Pending, RequestState.Closed,
                                                    accessTeamWF, WorkflowState.Pending_Provisioning,
                                                    WorkflowState.Closed_Completed)
                                 ||
                                 reqStateTransition(req, RequestState.Pending, RequestState.Closed,
                                                    accessTeamWF, WorkflowState.Approved,
                                                    WorkflowState.Closed_Completed);

                    if (result)
                    {
						if (req.submittedBy.ToString() != req.userId.ToString())
						{
							Email.SendTaskEmail(EmailTaskType.UpdateRequester, req.userId + "@apollogrp.edu", req.userDisplayName, _id, req.submittedBy, WorkflowState.Closed_Completed, null);
						}
						Email.SendTaskEmail(EmailTaskType.UpdateRequester, req.submittedBy + "@apollogrp.edu", Utilities.GetFullNameByLoginId(req.submittedBy), _id, req.submittedBy, WorkflowState.Closed_Completed, null);
                        db.SubmitChanges();
                        resp = new WebMethodResponse(true, "Finalization", "Success");
                    }
                    else
                    {
                        resp = new WebMethodResponse(false, "Finalization Error", SeeDetailsReqTracking);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("AccessRequest - FinalizeRequest, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                resp = new WebMethodResponse(false, "Finalization Exception", ex.Message);
            }

            return resp;
        }

        public List<int> GetPendingApprovalActorId()
        {
            var pendingActorIds = new List<int>();
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(r => r.pkId == _id);

                try
                {
                    foreach (SNAP_Workflow w in req.SNAP_Workflows)
                    {
                        if (w.actorId != AccessTeamActorId)
                        {
                            var states = w.SNAP_Workflow_States.Where(s => s.completedDate == null && s.notifyDate != null).ToList();
                            foreach (var state in states)
                            {
                                if (state.workflowStatusEnum == (byte)WorkflowState.Pending_Approval)
                                {
                                    pendingActorIds.Add(w.actorId);
                                }
                                
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Logger.Fatal("AccessRequest - GetPendingApprovalActorId, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                }
            }
            return pendingActorIds;
        }

        public int GetPendingApprovalActorType()
        {
            int approvalType = -1;
            var actIds = GetPendingApprovalActorId();
            using (var db = new SNAPDatabaseDataContext())
            {
                foreach (var i in actIds)
                {
                    var actor = db.SNAP_Actors.Single(a => a.pkId == i);
                    var type = actor.SNAP_Actor_Group.actorGroupType ?? -1;
                    if (approvalType != -1)
                    {
                        if (approvalType != type)
                            return -1;
                    }
                    else
                    {
                        approvalType = type;
                    }
                }
            }

            return approvalType;
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

					//sb.AppendLine(string.Format("-------------------------------------------------------\r\n{0}\r\n----\r\n{1}"
					//    , text.SNAP_Access_Details_Form.label
					//    , text.userText));

					int i = 0;
					foreach (var text in requestTexts)
					{
						if (text.userText != "")
						{
							if (i != 0) { sb.AppendLine("-------------------------------------------------------"); }
							sb.AppendLine("");
							sb.AppendLine(text.SNAP_Access_Details_Form.label);
							sb.AppendLine("");
							sb.AppendLine(text.userText);
							sb.AppendLine("");
							i++;
						}
						else
						{
							if (i != 0) { sb.AppendLine("-------------------------------------------------------"); }
							sb.AppendLine("");
							sb.AppendLine(text.SNAP_Access_Details_Form.label);
							sb.AppendLine("");
							sb.AppendLine("EMPTY");
							sb.AppendLine("");
							i++;
						}
						if (i == requestTexts.Count())
						{
							sb.AppendLine("-------------------------------------------------------");
						}
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
			//comment = comment.Replace("<br />", string.Empty);
            accessTeamWF.SNAP_Workflow_Comments.Add(new SNAP_Workflow_Comment()
            {
                commentText = comment,
                commentTypeEnum = (byte)type,
                createdDate = DateTime.Now
            });

        }

        private void createrApprovalWorkFlow(SNAPDatabaseDataContext db, List<int> actorIds)
        {
            var req = db.SNAP_Requests.Single(x => x.pkId == _id);
            var toDeleteActorList = getToDeletedActorList(req,actorIds).ToList();

            // we need to remove old approvers who are not in the new list
            if (req.SNAP_Workflows.Count > 1)
            {
                foreach (var i in toDeleteActorList)
                {
                    var wf = req.SNAP_Workflows.Single(w => w.actorId == i);
                    deleteActorWorkflow(db, wf);
                }
            }
             

            foreach (var actId in actorIds)
            {
                SNAP_Workflow wf;
                if (req.SNAP_Workflows.Count(w => w.actorId == actId) == 0) // 
                {
                    wf = new SNAP_Workflow() { actorId = actId };
                    req.SNAP_Workflows.Add(wf);

                }
                else // wf already exists due to change request(all prev wf are completed) or access team just needs to update the wf component
                {
                    wf = req.SNAP_Workflows.Single(w => w.actorId == actId);
                }

                var actor = db.SNAP_Actors.Single(a => a.pkId == actId);
                var agt = actor.SNAP_Actor_Group.actorGroupType ?? 3; // default to accessteam if null

                ActorApprovalType t = (ActorApprovalType) agt;

                stateTransition(t, wf, WorkflowState.Not_Active, WorkflowState.Not_Active);


                // check to make sure we can update manager in the req table
                if (t == ActorApprovalType.Manager)
                {
                    if (req.managerUserId != actor.userId)
                    {
                        req.managerUserId = actor.userId;
                        req.managerDisplayName = actor.displayName;
                    }

                }
            }

        }

        private void deleteActorWorkflow(SNAPDatabaseDataContext db, SNAP_Workflow wf)
        {
            db.SNAP_Workflow_States.DeleteAllOnSubmit(wf.SNAP_Workflow_States);
            db.SNAP_Workflows.DeleteOnSubmit(wf);
            db.SNAP_Workflow_Comments.DeleteAllOnSubmit(wf.SNAP_Workflow_Comments);
        }

        private List<int> editApprovalWorkFlow(SNAPDatabaseDataContext db, List<int> actorIds)
        {
        /*
            var newAddedActorIds = new List<int>();
            var req = db.SNAP_Requests.Single(x => x.pkId == _id);

            var toDeleteActorList = getToDeletedActorList(req, actorIds);
            deletedActors = toDeleteActorList;

            // we need to remove old approvers who are not in the new list
            if (req.SNAP_Workflows.Count > 1)
            {
                foreach (var i in toDeleteActorList)
                {
                    var wf = req.SNAP_Workflows.Single(w => w.actorId == i);

                    // only remove pending approval and not active wf
                    if ((wf.SNAP_Workflow_States.Where(s => s.completedDate == null
                        && s.notifyDate != null
                        && (s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval)).Count() == 1)

                            ||

                        (wf.SNAP_Workflow_States.Where(s => s.completedDate == null
                            && s.notifyDate == null
                            && s.workflowStatusEnum == (byte)WorkflowState.Not_Active).Count() == 1)
                        )
                    {
                        deleteActorWorkflow(db, wf);
                    }
                }
            }


            foreach (var actId in actorIds)
            {
                SNAP_Workflow wf;
                if (req.SNAP_Workflows.Count(w => w.actorId == actId) == 0) // 
                {
                    newAddedActorIds.Add(actId);

                    wf = new SNAP_Workflow() { actorId = actId };
                    req.SNAP_Workflows.Add(wf);

                    var actor = db.SNAP_Actors.Single(a => a.pkId == actId);
                    var agt = actor.SNAP_Actor_Group.actorGroupType ?? 3; // default to accessteam if null

                    ActorApprovalType t = (ActorApprovalType)agt;

                    stateTransition(t, wf, WorkflowState.Not_Active, WorkflowState.Not_Active);
                }
            }

            return newAddedActorIds;

*/

            addedActors.Clear();
            deletedActors.Clear();

            //var newAddedActorIds = new List<int>();
            var req = db.SNAP_Requests.Single(x => x.pkId == _id);

            //var toDeleteActorList = getToDeletedActorList(req, actorIds);
            deletedActors = getToDeletedActorList(req, actorIds); ;

            // we need to remove old approvers who are not in the new list
            if (req.SNAP_Workflows.Count > 1)
            {
                foreach (var i in deletedActors)
                {
                    var wf = req.SNAP_Workflows.Single(w => w.actorId == i);

                    // only remove pending approval and not active wf
                    if ((wf.SNAP_Workflow_States.Where(s => s.completedDate == null
                        && s.notifyDate != null
                        && (s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval)).Count() == 1)

                            ||

                        (wf.SNAP_Workflow_States.Where(s => s.completedDate == null
                            && s.notifyDate == null
                            && s.workflowStatusEnum == (byte)WorkflowState.Not_Active).Count() == 1)
                        )
                    {
                        deleteActorWorkflow(db,wf);
                    }
                }
            }


            foreach (var actId in actorIds)
            {
                SNAP_Workflow wf;
                if (req.SNAP_Workflows.Count(w => w.actorId == actId) == 0) // 
                {
                    addedActors.Add(actId);

                    wf = new SNAP_Workflow() { actorId = actId };
                    req.SNAP_Workflows.Add(wf);

                    var actor = db.SNAP_Actors.Single(a => a.pkId == actId);
                    var agt = actor.SNAP_Actor_Group.actorGroupType ?? 3; // default to accessteam if null

                    ActorApprovalType t = (ActorApprovalType)agt;

                    stateTransition(t, wf, WorkflowState.Not_Active, WorkflowState.Not_Active);
                }
            }

            return addedActors;
 
        }


        private List<int> getToDeletedActorList(SNAP_Request req, List<int> actorIds)
        {
            var orgActorList = new List<int>();
            foreach (var wf in req.SNAP_Workflows)
            {
                if (wf.actorId != 1)
                    orgActorList.Add(wf.actorId);
            }
            return orgActorList.Except(actorIds).ToList();
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
                        //if (state.notifyDate == null && state.workflowStatusEnum == (byte)WorkflowState.Pending_Approval)
                        if (state.notifyDate == null && state.workflowStatusEnum == (byte)WorkflowState.Not_Active)
                        {
                            //Email.TaskAssignToApprover(state.SNAP_Workflow.SNAP_Actor.emailAddress, state.SNAP_Workflow.SNAP_Actor.displayName, _id, state.SNAP_Workflow.SNAP_Request.userDisplayName);

							Email.SendTaskEmail(EmailTaskType.AssignToApprover
								, state.SNAP_Workflow.SNAP_Actor.emailAddress
								, state.SNAP_Workflow.SNAP_Actor.displayName
								, _id
								, state.SNAP_Workflow.SNAP_Request.userDisplayName);
                            
                            state.notifyDate = DateTime.Now;
                            state.completedDate = DateTime.Now;
                            done = true;
                        }
                    }

                    // from not acitve -> pending approval
                    if (done)
                    {
                        var actorType = (ActorApprovalType) (wf.SNAP_Actor.SNAP_Actor_Group.actorGroupType ?? 3); // default workflow admin

                        stateTransition(actorType, wf, WorkflowState.Not_Active, WorkflowState.Pending_Approval);

                        // if we have multple team approvers, we need to sequential them, not all out 
                        if (actorType == ActorApprovalType.Team_Approver)
                            break;
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


        internal static void stateTransition(ActorApprovalType approvalType, SNAP_Workflow wf, WorkflowState from, WorkflowState to)
        {
            //
            // !!! if the state completion date is null, it is the WF current state !!!
            //

            // a brand new apporval WF has from not-active to not-active state and have not pref state yet, 
            // such as when creating a new manager, team and techical approval workflow.
            // Rememeber that: the accessTeamWF has prev state because it being inserted by the 
            // store procedure when inserting the request the first time
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
			{
				newState.notifyDate = DateTime.Now;
			}
			else if (from == WorkflowState.Not_Active && to == WorkflowState.Pending_Approval)
			{
				// approval wf from not-active -> pending approval, we alread send the email out
				newState.notifyDate = DateTime.Now;
			}
            
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
                    to == WorkflowState.Closed_Denied || to == WorkflowState.Closed_Abandon)
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
                        if (to == WorkflowState.Workflow_Created) // same as the due day for technical approval since workflow admin depend on it
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


        public static int GetActorIdByUserId(ActorGroupType actorGroupType, string userId)
        {
            //Console.WriteLine("GetAtorIdByUSerId: " + userId);
            using (var db = new SNAPDatabaseDataContext())
            {
                int actorId = 0;
                try
                {
                    var query = from sa in db.SNAP_Actors
                                join sag in db.SNAP_Actor_Groups on sa.actor_groupId equals sag.pkId
                                where sa.userId == userId && sag.actorGroupType == (byte) actorGroupType
                                select sa.pkId;
                    if (query.Count() > 0)
                    {
                        return (int)query.First();
                    }
                    else
                    {
                        ADUserDetail usrDetail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(userId);
                        if (usrDetail != null)
                        {
                            var actorGroupId = db.SNAP_Actor_Groups.Single(g => g.actorGroupType == (byte)actorGroupType).pkId;
                            InsertActor(userId, db, usrDetail, actorGroupId);
                            return GetActorIdByUserId(actorGroupType, userId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AccessRequest - GetAtorIdByUserId, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                    Logger.Fatal("AccessRequest - GetAtorIdByUserId, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                }

                return actorId;
            }
        }

        public static int GetActorIdByUserIdAndGroupId(string userId, int groupId)
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                int actorId = 0;
                try
                {
                    var results = from sa in db.SNAP_Actors
                                join sag in db.SNAP_Actor_Groups on sa.actor_groupId equals sag.pkId
                                where sa.userId == userId 
                                && sag.pkId == groupId
                                select sa.pkId;
                    if (results.Count() > 0) { return (int)results.First(); }
                    else
                    {
                        ADUserDetail usrDetail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(userId);
                        if (usrDetail != null)
                        {
                            var actorGroupId = db.SNAP_Actor_Groups.Single(g => g.pkId == groupId).pkId;
                            InsertActor(userId, db, usrDetail, actorGroupId);
                            return GetActorIdByUserIdAndGroupId(userId, groupId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Fatal("AccessRequest - GetAtorIdByUserIdAndGroupId, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
                }

                return actorId;
            }
        }

        private static void InsertActor(string userId, SNAPDatabaseDataContext db, ADUserDetail usrDetail, int actorGroupId)
        {
            db.SNAP_Actors.InsertOnSubmit(new SNAP_Actor()
            {
                displayName =
                    usrDetail.FirstName + " " + usrDetail.LastName,
                actor_groupId = actorGroupId,
                emailAddress = usrDetail.EmailAddress,
                isActive = true,
                isDefault = false,
                userId = userId

            });

            db.SubmitChanges();
        }


        public static int GetWorkflowId(int actorId, int requestId)
        {
            int workflowId = 0;

            using (var db = new SNAPDatabaseDataContext())
            {
                var result = db.SNAP_Workflows
                             .Where(u => u.actorId == actorId && u.requestId == requestId)
                             .OrderByDescending(o => o.pkId)
                             .Select(s => s.pkId)
                             .First();

                workflowId = (int)result;
            }

            return workflowId;
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

        protected WebMethodResponse approveAndInformOtherSequentialManager()
        {
            /*
            if (req.statusEnum != (byte)RequestState.Pending)
                return new WebMethodResponse(false, "Approver Error", AccessRequest.SeeDetailsReqTracking);

             */

            //using (TransactionScope ts = new TransactionScope())
            //{
                if (wfStateChange(ActorApprovalType.Manager, WorkflowState.Approved))
                {
                    informApproverForAction();
                    //db.SubmitChanges();
                    //ts.Complete();
                    CompleteRequestApprovalCheck(req.pkId);
                    db.SubmitChanges();
                    return new WebMethodResponse(true, "Approver", "Success");
                }
            //}

            return new WebMethodResponse(false, "Approver Error", AccessRequest.SeeDetailsReqTracking);

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

        public static void CompleteRequestApprovalCheck(int id)
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                //var twf = db.SNAP_Workflows.Single(w => w.pkId == id);
                var req = db.SNAP_Requests.Single(r => r.pkId == id);
                var accessReq = new AccessRequest(id);
                var accessTeamWF = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Workflow_Admin)[0];
                var mgrWF = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Manager)[0];
                var teamApproverWFs = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Team_Approver);
                var techApproverWFs = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Technical_Approver);


                // only manager in the approal wf
                if (teamApproverWFs.Count == 0 && techApproverWFs.Count == 0)
                {
                    // make sure manager has not outstanding approval
                    if (mgrWF.SNAP_Workflow_States.OrderByDescending(s => s.pkId).First().completedDate != null)
                    {
                        AccessRequest.stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF,
                                                      WorkflowState.Workflow_Created, WorkflowState.Approved);
                        Email.SendTaskEmail(EmailTaskType.TransitionToPendingProvisioning,
                                            ConfigurationManager.AppSettings["AIM-DG"], null, req.pkId,
                                            req.userDisplayName);
                    }
                }
                else
                {
                    // we may or may not have team approver in the wf, if we do make sure all team approvers are approved
                    var done = true;

                    foreach (var wf in teamApproverWFs)
                    {
                        if (wf.SNAP_Workflow_States.Count(s => s.completedDate != null && s.workflowStatusEnum == (byte) WorkflowState.Approved) == 1)
                        {
                            done = true;
                        }
                        else
                        {
                            done = false;
                            break;
                        }
                    }

                    // now check technical approvers
                    //wfs = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Technical_Approver);
                    var totalApproved = 0;
                    var state =
                        accessTeamWF.SNAP_Workflow_States.Single(
                            s => s.workflowStatusEnum == (byte) WorkflowState.Workflow_Created
                                 && s.completedDate == null);
                        // get lastest 'worflow created' for the workflowadmin state

                    foreach (var w in techApproverWFs)
                    {
                        var cnt = w.SNAP_Workflow_States.Count(
                            s => s.workflowStatusEnum == (byte) WorkflowState.Approved
                                 && s.completedDate != null
                                 && s.pkId >= state.pkId);
                        // only check approval for the latest iteration, ignore previously approved interateion

                        totalApproved += cnt;
                    }

                    if (totalApproved == techApproverWFs.Count && done)
                    {
                        AccessRequest.stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF,
                                                      WorkflowState.Workflow_Created, WorkflowState.Approved);

                        Email.SendTaskEmail(EmailTaskType.TransitionToPendingProvisioning,
                                            ConfigurationManager.AppSettings["AIM-DG"], null, req.pkId,
                                            req.userDisplayName);
                    }
                }

                db.SubmitChanges();
            }
        }

        protected WebMethodResponse requestToChangeBy(ActorApprovalType approvalType, string comment)
        {
            /*
            if (req.statusEnum != (byte)RequestState.Pending)
                return new WebMethodResponse(false, "Approver Error", AccessRequest.SeeDetailsReqTracking);
             */

            //using (TransactionScope ts = new TransactionScope())
            //{
                if (wfStateChange(approvalType, WorkflowState.Change_Requested))
                {
					comment = comment.Replace("<br />", string.Empty);
                    approvalRequestToChange(comment);
                    db.SubmitChanges();
                    //ts.Complete();
                    return new WebMethodResponse(true, "Approver", "Success"); ;
                }
            //}
                return new WebMethodResponse(false, "Approver Error", AccessRequest.SeeDetailsReqTracking); ;
        }

        protected void approvalRequestToChange(string comment)
        {
            AccessRequest.stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF, WorkflowState.Workflow_Created, WorkflowState.Change_Requested);
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

			comment = comment.Replace("<br />", string.Empty); 

			//if (result)
			//{
			//    addAccessTeamComment(accessTeamWF, comment, CommentsType.Requested_Change);
			//    Email.SendTaskEmail(EmailTaskType.UpdateRequester, req.submittedBy, req.userDisplayName, _id, req.submittedBy, WorkflowState.Change_Requested, comment);
			//    db.SubmitChanges();
			//}

            wf.SNAP_Workflow_Comments.Add(new SNAP_Workflow_Comment()
            {
                commentText = comment,
                commentTypeEnum = (byte)CommentsType.Requested_Change,
                createdDate = DateTime.Now
            });

			if (req.submittedBy.ToString() != req.userId.ToString())
			{
				Email.SendTaskEmail(EmailTaskType.UpdateRequester, req.userId + "@apollogrp.edu", req.userDisplayName, req.pkId, req.submittedBy, WorkflowState.Change_Requested, comment);
			}
            // Test: checking svn update
			Email.SendTaskEmail(EmailTaskType.UpdateRequester, req.submittedBy + "@apollogrp.edu", Utilities.GetFullNameByLoginId(req.submittedBy), req.pkId, req.submittedBy, WorkflowState.Change_Requested, comment);
			Email.SendTaskEmail(EmailTaskType.UpdateRequester, ConfigurationManager.AppSettings["AIM-DG"], "AIM for " + req.userDisplayName, req.pkId, req.submittedBy, WorkflowState.Change_Requested, comment);
            
        }

        protected WebMethodResponse denyBy(ActorApprovalType approvalType, string comment)
        {
            /*
            if (req.statusEnum != (byte)RequestState.Pending)
                return new WebMethodResponse(false, "Approver Error", AccessRequest.SeeDetailsReqTracking);
            */

            //using (TransactionScope ts = new TransactionScope())
            //{
                if (wfStateChange(approvalType, WorkflowState.Closed_Denied))
                {
					comment = comment.Replace("<br />", string.Empty);
                    approvalDeny(approvalType, comment);
                    db.SubmitChanges();
                    //ts.Complete();
                    return new WebMethodResponse(true, "Approver", "Success"); 
                }
            //}
                return new WebMethodResponse(false, "Approver Error", AccessRequest.SeeDetailsReqTracking);
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
                    AccessRequest.stateTransition(ActorApprovalType.Workflow_Admin, accessTeamWF, WorkflowState.Workflow_Created, WorkflowState.Closed_Denied);
                    req.statusEnum = (byte)RequestState.Closed;
					
					comment = comment.Replace("<br />", string.Empty);
					if (req.submittedBy.ToString() != req.userId.ToString())
					{
						Email.SendTaskEmail(EmailTaskType.UpdateRequester, req.userId + "@apollogrp.edu", req.userDisplayName, req.pkId, req.submittedBy, WorkflowState.Closed_Denied, comment);
					}
					Email.SendTaskEmail(EmailTaskType.UpdateRequester, req.submittedBy + "@apollogrp.edu", Utilities.GetFullNameByLoginId(req.submittedBy), req.pkId, req.submittedBy, WorkflowState.Closed_Denied, comment);
					Email.SendTaskEmail(EmailTaskType.UpdateRequester, ConfigurationManager.AppSettings["AIM-DG"], "AIM for " + req.userDisplayName, req.pkId, req.submittedBy, WorkflowState.Closed_Denied, comment);
                    break;
                }
            }
        }

		public abstract WebMethodResponse Approve();
        public abstract WebMethodResponse Deny(string comment);
        public abstract WebMethodResponse RequestToChange(string comment);
    }

    public class ManagerApprovalWorkflow : ApprovalWorkflow
    {
        public ManagerApprovalWorkflow(int id) : base(id){}

        public override WebMethodResponse Approve()
        {
            return approveAndInformOtherSequentialManager();
        }

        public override WebMethodResponse Deny(string comment)
        {
			comment = comment.Replace("<br />", string.Empty);
            return denyBy(ActorApprovalType.Manager, comment);
        }

        public override WebMethodResponse RequestToChange(string comment)
        {
			comment = comment.Replace("<br />", string.Empty);
            return requestToChangeBy(ActorApprovalType.Manager, comment);
        }
    }

    public class TeamApprovalWorkflow : ApprovalWorkflow
    {
        public TeamApprovalWorkflow(int id): base(id){}

        public override WebMethodResponse Approve()
        {
            return approveAndInformOtherSequentialManager();
        }

        public override WebMethodResponse Deny(string comment)
        {
			comment = comment.Replace("<br />", string.Empty);
            return denyBy(ActorApprovalType.Team_Approver, comment);
        }

        public override WebMethodResponse RequestToChange(string comment)
        {
			comment = comment.Replace("<br />", string.Empty);
            return requestToChangeBy(ActorApprovalType.Team_Approver, comment);
        }
    }

    public class TechnicalApprovalWorkflow : ApprovalWorkflow
    {
        public TechnicalApprovalWorkflow(int id): base(id){}

        public override WebMethodResponse Approve()
        {
            /*
            if (req.statusEnum != (byte)RequestState.Pending)
                return new WebMethodResponse(false, "Approver Error", AccessRequest.SeeDetailsReqTracking);
            */

            //using (TransactionScope ts = new TransactionScope())
            //{
                if (wfStateChange(ActorApprovalType.Technical_Approver, WorkflowState.Approved))
                {
                    CompleteRequestApprovalCheck(req.pkId);
                    db.SubmitChanges();
                    //ts.Complete();
                    return new WebMethodResponse(true, "Approver", "Success");
                }
            //}

                return new WebMethodResponse(false, "Approver Error", AccessRequest.SeeDetailsReqTracking); ;
        }

        public override WebMethodResponse Deny(string comment)
        {
			comment = comment.Replace("<br />", string.Empty);
            return denyBy(ActorApprovalType.Technical_Approver, comment);
        }

        public override WebMethodResponse RequestToChange(string comment)
        {
			comment = comment.Replace("<br />", string.Empty);
            return requestToChangeBy(ActorApprovalType.Technical_Approver, comment);
        }
    }

    #endregion

}
