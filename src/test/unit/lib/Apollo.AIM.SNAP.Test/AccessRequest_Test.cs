﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Apollo.AIM.SNAP.CA;
using Apollo.AIM.SNAP.Model;
using NUnit.Framework;

namespace Apollo.AIM.SNAP.Test
{
    [TestFixture]
    public class AccessRequest_Test
    {
        // this is from actor group table
        private static int TEAMAPPROVALGROUP = 2;
        private static int TECHNICALAPPROVALGROUP = 3;
        private static int MANAGERGROUP = 4;



        private int accessTeamActorId = 1;
        private int managerActorId = 0;
        private int teamApprovalActorId = 0;
        private int windowsServerActorId = 0;
        private int networkShareActorId = 0;
        private int databaseActorId = 0;

        
        [TestFixtureSetUp]
        public void Init()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var actors = db.SNAP_Actors.Where(a => a.actor_groupId == TEAMAPPROVALGROUP).ToList();
                teamApprovalActorId = actors[0].pkId;
                actors = db.SNAP_Actors.Where(a => a.actor_groupId == TECHNICALAPPROVALGROUP).ToList();
                windowsServerActorId = actors[0].pkId;
                networkShareActorId = actors[1].pkId;
                databaseActorId = actors[2].pkId;
                actors = db.SNAP_Actors.Where(a => a.actor_groupId == MANAGERGROUP).ToList();
                managerActorId = actors[0].pkId;

            }
        }


        [SetUp]
        public void SetUp()
        {
            
            cleanUp();

            
            using (var db = new SNAPDatabaseDataContext())
            {

                List<RequestData> requestDataList = new List<RequestData>()
                                                        {
                                                            new RequestData() {FormId = "2", UserText = "Request Access 2"},
                                                            new RequestData() {FormId = "3", UserText = "Request Access 3"},
                                                            new RequestData() {FormId = "4", UserText = "Request Access 4"},
                                                            new RequestData() {FormId = "5", UserText = "Request Access 5"}
                                                        };

                var xmlInput = RequestData.ToXml(requestDataList);

                db.usp_insert_request_xml(xmlInput, "UnitTester", "pxlee", "Pong Lee", "Programmer", "gjbelang", "Greg Belanger");
            }
            
            
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        private void cleanUp()
        {
            using (var db = new SNAPDatabaseDataContext())
            {

                var reqs = db.SNAP_Requests.Where(x => x.submittedBy == "UnitTester").ToList();
                
                foreach (var r in reqs)
                {
                    var wfs = db.SNAP_Workflows.Where(x => x.requestId == r.pkId).ToList();
                    foreach (SNAP_Workflow wf in wfs)
                    {
                        var states = db.SNAP_Workflow_States.Where(x => x.workflowId == wf.pkId);
                        db.SNAP_Workflow_States.DeleteAllOnSubmit(states);

                        var comments = db.SNAP_Workflow_Comments.Where(c => c.workflowId == wf.pkId);
                        db.SNAP_Workflow_Comments.DeleteAllOnSubmit(comments);
                    }

                    db.SNAP_Workflows.DeleteAllOnSubmit(wfs);

                    var uts = db.SNAP_Access_User_Texts.Where(x => x.requestId == r.pkId);
                    db.SNAP_Access_User_Texts.DeleteAllOnSubmit(uts);
                    db.SNAP_Requests.DeleteOnSubmit(r);

                    var reqComments = db.SNAP_Request_Comments.Where(c => c.requestId == r.pkId);
                    db.SNAP_Request_Comments.DeleteAllOnSubmit(reqComments);
                }

                db.SubmitChanges();
            }

        }

        [Test]
        public void ShouldAckByAccessTeam()
        {
            
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();

                var wf = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == accessTeamActorId);
                var states = db.SNAP_Workflow_States.Where(x => x.workflowId == wf.pkId);
                foreach (var s in states)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," + ((s.completedDate != null) ? s.completedDate.ToString() : string.Empty));
                }

                // access team wf pending ack -> pending workflow
                Assert.IsTrue(states.Count(x => x.workflowStatusEnum == (byte) WorkflowState.Pending_Acknowlegement) == states.Count(x => x.workflowStatusEnum == (byte) WorkflowState.Pending_Workflow));
                
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Pending);
            }
        }

        [Test]
        public void ShouldAllowAccessTeamToComment()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                accessReq.AddComment("This is access team comment", CommentsType.Access_Notes_AccessTeam);
                accessReq.AddComment("This is access team for approver comment", CommentsType.Access_Notes_ApprovingManager);
                accessReq.AddComment("This is access team for requestor comment", CommentsType.Access_Notes_Requestor);
                accessReq.AddComment("This is access team for superuser comment", CommentsType.Access_Notes_SuperUser);
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                Assert.IsTrue(req.SNAP_Request_Comments.Count == 4);
            }

        }

        [Test]
        public void ShouldCloseDeniedByAccessTeam()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.NoAccess(WorkflowAction.Denied, "Deny");
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                verifyWorkflowState(req.SNAP_Workflows[0], WorkflowState.Closed_Denied);
                verifyWorkflowComment(req.SNAP_Workflows[0], CommentsType.Denied);
            }
            
        }

        private void verifyWorkflowState(SNAP_Workflow wf, WorkflowState state)
        {
            Assert.IsTrue(wf.SNAP_Workflow_States.Count(
                              s => s.workflowStatusEnum == (byte) state) == 1);
        }

        private void verifyWorkflowComment(SNAP_Workflow wf, CommentsType type)
        {
            Assert.IsTrue(wf.SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte)type) == 1);
        }

        private void verifyWorkflowStateComplete(SNAP_Workflow wf, WorkflowState state)
        {
            Assert.IsTrue(wf.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)state).completedDate != null);
        }
        [Test]
        public void ShouldCloseCancelledByAccessTeam()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.NoAccess(WorkflowAction.Cancel, "Cancel");
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                verifyWorkflowState(req.SNAP_Workflows[0], WorkflowState.Closed_Cancelled);
                verifyWorkflowComment(req.SNAP_Workflows[0], CommentsType.Cancelled);
            }

        }

        [Test]
        public void ShouldRequestToChangeByAccessTeanMultipleTimes()
        {
            for (int i = 0; i < 5; i++)
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                    var accessReq = new AccessRequest(req.pkId);
                    accessReq.Ack();
                    accessReq.RequestToChange("Please change it");
                }

                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                    Assert.IsTrue(req.statusEnum == (byte) RequestState.Change_Requested);
                    Assert.IsTrue(req.SNAP_Workflows[0].SNAP_Workflow_States.Count(s => s.workflowStatusEnum == (byte) WorkflowState.Change_Requested) > i);
                    Assert.IsTrue(req.SNAP_Workflows[0].SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte) CommentsType.Requested_Change) > i);
                }


                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                    var accessReq = new AccessRequest(req.pkId);
                    accessReq.RequestChanged();
                }


                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                    Assert.IsTrue(req.statusEnum == (byte) RequestState.Open);
                    var cnt = i + 1;
                    Assert.IsTrue(req.SNAP_Workflows[0].SNAP_Workflow_States.Count(s => s.workflowStatusEnum == (byte) WorkflowState.Pending_Acknowlegement) > cnt);
                }
            }
        }

        
        [Test] public void ShouldCreateWorkflowByAccessTeam()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);

                accessReq.Ack();

                accessReq.CreateWorkflow(new List<int>() { managerActorId });

                
                var accessTeamWF = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == accessTeamActorId);
                var accessTeamWFStates = db.SNAP_Workflow_States.Where(x => x.workflowId == accessTeamWF.pkId);
                foreach (var s in accessTeamWFStates)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," + ((s.completedDate != null) ? s.completedDate.ToString() : "TBD"));
                }

                verifyWorkflowState(accessTeamWF, WorkflowState.Pending_Approval);
                var managerWF = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == managerActorId);
                var managerWFStates = db.SNAP_Workflow_States.Where(x => x.workflowId == managerWF.pkId);
                foreach (var s in managerWFStates)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," + ((s.completedDate != null) ? s.completedDate.ToString() : "TBD"));
                }
                verifyWorkflowState(managerWF, WorkflowState.Pending_Approval);
            }
        }
         
        [Test]
        public void ShouldReturnManagerWorkflowApprovalType()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>() { managerActorId });

                var wfs = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Manager);
                Assert.IsTrue(wfs.Count == 1);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                Assert.IsTrue(wfs.Count == 0);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(wfs.Count == 0);

            }

        }

        [Test]
        public void ShouldReturnManagerAndTeamWorkflowApprovalType()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>() { managerActorId, teamApprovalActorId });

                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs.Count == 1);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                Assert.IsTrue(wfs.Count == 1);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(wfs.Count == 0);

            }

        }


        [Test]
        public void ShouldReturnManagerAndTeamAndTechnicalWorkflowApprovalType()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>() { managerActorId, teamApprovalActorId, windowsServerActorId, databaseActorId, networkShareActorId });

                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs.Count == 1);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                Assert.IsTrue(wfs.Count == 1);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(wfs.Count == 3);

            }

        }

        [Test]
        public void ShouldInforManagerForAction()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });

                var wfs = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Manager);
                Assert.IsTrue(wfs.Count == 1);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States[0].notifyDate != null);
            }

        }

        [Test]
        public void ShouldHandleManagerApprove()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });

                var wfs = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Manager);

                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);
                
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);


                verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Approval);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);
            }

        }

        [Test]
        public void ShouldHandleManagerRequestToChange()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });

                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);

                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Change);

                //accessReq.RequestChanged();
                //accessReq.Ack();

            }

            
            
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                


                Assert.IsTrue(req.statusEnum == (byte)RequestState.Change_Requested);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Change_Requested);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                verifyWorkflowState(wfs[0], WorkflowState.Change_Requested);
            }

        }

        [Test]
        public void ShouldHandleFromManagerToTeamApprove()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });

                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Approval);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Approval);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);

            }

        }


        [Test]
        public void ShouldHandleTeamApproverRequestToChange()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });


                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approer request to change
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Change);

                /*
                accessReq.RequestChanged();
                */
            }


            
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Change_Requested);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Change_Requested).completedDate != null);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Change_Requested);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Change_Requested).completedDate == null);
                verifyWorkflowState(wfs[0], WorkflowState.Change_Requested);
            }
            
        }


        [Test]
        public void ShouldHandleFromManagerToTeamToTechicalApprove()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });

                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get technical approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Approval);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);


                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Approval);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);


                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Approval);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);

            }


        }



        [Test]
        public void ShouldHandleFromManagerToTeamToOneTechicalApprove()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });


                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get technical approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);

                var accessTeamWF = req.SNAP_Workflows.Single(w => w.actorId == 1); // actid = 1 => accessTeam
                var accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval);

                Assert.IsTrue(accessTeamState.completedDate == null);  // can't complete this yet, need to wait for all technical approvals

            }


        }


        [Test]
        public void ShouldHandleFromManagerToTeamToOneTechicalRequestToChange()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });


                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get technical approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Change);

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Change_Requested);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Change_Requested);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Change_Requested).completedDate == null);

            }


        }

        [Test]
        public void ShouldHandleFromManagerToTeamToAllTechicalApprove()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });


                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get all technical approval

                // !!! for individual test, please uncommnet it
                // !!! comment it out to save time

                // System.Threading.Thread.Sleep(90000);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[1].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[2].pkId, WorkflowAction.Approved);


            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);

                //var accessTeamWF = req.SNAP_Workflows.Single(w => w.actorId == 1); // actid = 1 => accessTeam
                var accessTeamWF = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin)[0]; // there is only on workflow admin
                var accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval);

                Assert.IsTrue(accessTeamState.completedDate != null);  // all technical approval received, complete it

                accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Approved);
                Assert.IsTrue(accessTeamState.completedDate == null);  // all technical approval received, complete it

                // make sure approving manager wf propergates notify and due date to final state
                var firstApprover = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver)[0];
                var firstApproverPendingState = firstApprover.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval);
                var firstApproverApproveState = firstApprover.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Approved);
                Assert.IsTrue(firstApproverPendingState.notifyDate == firstApproverApproveState.notifyDate);
                Assert.IsTrue(firstApproverPendingState.dueDate == firstApproverApproveState.dueDate);

            }

        }


        [Test]
        public void ShouldNotEnterToDifferentStateWhenNotReady()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                Assert.IsFalse(accessReq.CreateServiceDeskTicket());
                Assert.IsFalse(accessReq.CreateWorkflow(new List<int>() { 1, 2, 3 }));
                Assert.IsFalse(accessReq.FinalizeRequest());
                Assert.IsFalse(accessReq.RequestChanged());
                accessReq.Ack();
                Assert.IsFalse(accessReq.CreateServiceDeskTicket());
                Assert.IsFalse(accessReq.Ack());
                Assert.IsFalse(accessReq.FinalizeRequest());
                Assert.IsFalse(accessReq.RequestChanged());
            }

        }

        [Test]
        public void ShouldHandleFromManagerToTeamToLastTechicalRequestToChange()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });


                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Manager);
                Assert.IsTrue(
                    wfs[0].SNAP_Workflow_States.Single(
                        s => s.workflowStatusEnum == (byte) WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get technical approval, but the last one request to change
                wfs = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[1].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[2].pkId, WorkflowAction.Change, "change it");

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Change_Requested);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);
                verifyWorkflowStateComplete(wfs[1], WorkflowState.Approved);
                verifyWorkflowStateComplete(wfs[2], WorkflowState.Change_Requested);

                //Assert.IsTrue(wfs[2].SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte) CommentsType.Requested_Change) == 
                //    1);
                verifyWorkflowComment(wfs[2], CommentsType.Requested_Change);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Change_Requested).completedDate == null);

            }

        }

        //[Ignore]
        [Test]
        public void ShouldHandleFromManagerToTeamToLastTechicalRequestToChangeLoop()
        {
            // set up for first request to change
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });

                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(
                    wfs[0].SNAP_Workflow_States.Single(
                        s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get technical approval, but the last one request to change
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[1].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[2].pkId, WorkflowAction.Change, "change it");

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Change_Requested);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);
                verifyWorkflowStateComplete(wfs[1], WorkflowState.Approved);
                verifyWorkflowStateComplete(wfs[2], WorkflowState.Change_Requested);

                verifyWorkflowComment(wfs[2], CommentsType.Requested_Change);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Change_Requested).completedDate == null);

            }

            for (int i = 0; i < 5; i++ )
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                    var accessReq = new AccessRequest(req.pkId);

                    accessReq.RequestChanged();
                    accessReq.Ack();
                    accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });


                    var accessWF = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                    var state = accessWF[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval
                        && s.completedDate == null); // get lastest 'pending approval' for the workflowadmin state

                    // get manager approal
                    var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                    //Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                    accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);
                    Assert.IsTrue(
                        wfs[0].SNAP_Workflow_States.Count(
                            s => s.completedDate != null 
                                && s.workflowStatusEnum == (byte) WorkflowState.Approved
                                && s.pkId > state.pkId) == 1 );

                    // get team approval
                    wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);

                    accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                    Assert.IsTrue(
                            wfs[0].SNAP_Workflow_States.Count(
                                s => s.completedDate != null
                                    && s.workflowStatusEnum == (byte)WorkflowState.Approved
                                    && s.pkId > state.pkId) == 1);

                    var r = new Random();
                    var last = r.Next(2);
                    // get only one technical request to change
                    wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                    for (int x = 0; x <= last; x++)
                    {
                        accessReq.WorkflowAck(wfs[x].pkId, WorkflowAction.Approved);
                        Assert.IsTrue(
                            wfs[0].SNAP_Workflow_States.Count(
                                s => s.completedDate != null
                                    && s.workflowStatusEnum == (byte)WorkflowState.Approved
                                    && s.pkId > state.pkId) == 1);
                    }

                    accessReq.WorkflowAck(wfs[++last].pkId, WorkflowAction.Change, "change it");
                    Assert.IsTrue(
                            wfs[last].SNAP_Workflow_States.Count(
                                s => s.completedDate != null
                                    && s.workflowStatusEnum == (byte)WorkflowState.Change_Requested
                                    && s.pkId > state.pkId) == 1);

                    /*
                    Assert.IsTrue(accessWF[0].SNAP_Workflow_States.Single(
                                s => s.completedDate == null
                                    && s.workflowStatusEnum == (byte)WorkflowState.Change_Requested)
                                   == 1);
                     */
                    
                }
        }

        [Ignore]
        [Test]
        public void ShouldHandleCreateSDTicket()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 //databaseActorId,
                                                 //networkShareActorId
                                             });


                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get technical approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // create SD ticket
                accessReq.CreateServiceDeskTicket();

                // For dev:
                // login to to http://awhdht02:8080/CAisd/pdmweb.exe 
                // sign in as svc_cap
                // look at Change Orders/Unassinged/all 

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Provisioning).completedDate == null);
                Assert.IsNotNull(req.ticketNumber);

            }

        }

        [Ignore]
        [Test]
        public void ShouldHandleFinalizeRequest()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 //databaseActorId,
                                                 //networkShareActorId
                                             });

                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get technical approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // create SD ticket
                accessReq.CreateServiceDeskTicket();

                // finalize it
                accessReq.FinalizeRequest();

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Provisioning);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Closed_Completed);
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
            }

        }


        [Test]
        public void ShouldHandleManagerDeniedRequest()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });

                // get manager disapproval
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Denied, "Bad Request");

            }

            
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Approval);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Closed_Denied);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                var wf = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Manager)[0];
                //Assert.IsTrue(wf.SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte) CommentsType.Denied) > 0);
                verifyWorkflowComment(wf, CommentsType.Denied);
            }
            
        }

        [Test]
        public void ShouldHandleTeamApprovalDeniedRequest()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });

                // get manager approval
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                
                // get team disapproval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Denied, "Still Bad");

                /*
                // get technical approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);
                */

            }


            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Approval);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Closed_Denied);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                var wf = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver)[0];
                Assert.IsTrue(wf.SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte)CommentsType.Denied) > 0);
                verifyWorkflowComment(wf, CommentsType.Denied);
            }

        }

        [Test]
        public void ShouldHandleTechicalApprovalDeniedRequest()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });

                // get manager approval
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                
                // get technical disapproval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Denied, "Bad!!!");
                
            }


            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Approval);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Closed_Denied);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                var wf = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver)[0];
                Assert.IsTrue(wf.SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte)CommentsType.Denied) > 0);
                verifyWorkflowComment(wf, CommentsType.Denied);
            }

        }

        [Test]
        public void ShouldHandleLastTechicalApprovalDeniedRequest()
        {
            int denier = 2;

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });

                // get manager approval
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get technical approval but last disapproval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[1].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[denier].pkId, WorkflowAction.Denied, "Last One Deny");

            }

            
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Approval);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Closed_Denied);


                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                verifyWorkflowStateComplete(wfs[denier], WorkflowState.Closed_Denied);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                var wfStates = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                verifyWorkflowComment(wfStates[denier], CommentsType.Denied);
            }
            
        }



        [Test] public void SouldReturnApprovalTypeObject()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 windowsServerActorId,
                                                 databaseActorId,
                                                 networkShareActorId
                                             });

                var wf = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Manager)[0];

                var testWf = ApprovalWorkflow.CreateApprovalWorkflow(wf.pkId);
                Assert.IsTrue(testWf.GetType() == (typeof(ManagerApprovalWorkflow)));

                wf = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver)[0];

                testWf = ApprovalWorkflow.CreateApprovalWorkflow(wf.pkId);
                Assert.IsTrue(testWf.GetType() == (typeof(TeamApprovalWorkflow)));

                wf = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver)[0];
                testWf = ApprovalWorkflow.CreateApprovalWorkflow(wf.pkId);
                Assert.IsTrue(testWf.GetType() == (typeof(TechnicalApprovalWorkflow)));

            }
        }

        [Test] public void TestDateDiff()
        {
            DateTime dueDate = DateTime.Parse("3/2/2010 1:00:00 AM");
            DateTime currentDate = DateTime.Parse("3/2/2010 2:05:00 AM");
            TimeSpan diff = currentDate.Subtract(dueDate);
            Console.WriteLine(diff.Days);

            currentDate = DateTime.Parse("3/3/2010 2:05:00 AM");
            diff = currentDate.Subtract(dueDate);
            Console.WriteLine(diff.Days);

            currentDate = DateTime.Parse("3/4/2010 2:05:00 AM");
            diff = currentDate.Subtract(dueDate);
            Console.WriteLine(diff.Days);


            dueDate = DateTime.Parse("3/2/2010 3:00:00 AM");
            currentDate = DateTime.Parse("3/2/2010 2:05:00 AM");
            diff = currentDate.Subtract(dueDate);
            Console.WriteLine(diff.Days);

            currentDate = DateTime.Parse("3/3/2010 2:05:00 AM");
            diff = currentDate.Subtract(dueDate);
            Console.WriteLine(diff.Days);

            currentDate = DateTime.Parse("3/4/2010 2:05:00 AM");
            diff = currentDate.Subtract(dueDate);
            Console.WriteLine(diff.Days);


            dueDate = DateTime.Parse("3/2/2010 3:00:00 PM");
            currentDate = DateTime.Parse("3/2/2010 2:05:00 AM");
            diff = currentDate.Subtract(dueDate);
            Console.WriteLine(diff.Days);

            currentDate = DateTime.Parse("3/3/2010 2:05:00 AM");
            diff = currentDate.Subtract(dueDate);
            Console.WriteLine(diff.Days);

            currentDate = DateTime.Parse("3/4/2010 2:05:00 AM");
            diff = currentDate.Subtract(dueDate);
            Console.WriteLine(diff.Days);
        }

        [Test]public void ShouldUpdateRequest()
        {
            int id;
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                Console.WriteLine("Before ...");
                Console.WriteLine("submitter: " + req.submittedBy);
                Console.WriteLine("userId : " + req.userId);
                Console.WriteLine("title: " + req.userTitle);
                Console.WriteLine("name: " + req.userDisplayName);
                Console.WriteLine("MgrId: " + req.managerUserId);
                Console.WriteLine("Mgr Name: " + req.managerDisplayName);

                id = req.pkId;
                updateRequest(req.pkId, "NewTester", "jdsteele", "Steeler", "clschwim", "Swimmer");
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.pkId == id);
                Console.WriteLine("After ...");
                Console.WriteLine("submitter: " + req.submittedBy);
                Console.WriteLine("userId : " + req.userId);
                Console.WriteLine("title: " + req.userTitle);
                Console.WriteLine("name: " + req.userDisplayName);
                Console.WriteLine("MgrId: " + req.managerUserId);
                Console.WriteLine("Mgr Name: " + req.managerDisplayName);
            }

        }

        private void updateRequest(long requestId, 
                                string submitterId, 
                                string userId, 
                                string userName,
                                string mgrId,
                                string mgrName)
        {
            var change = false;
            ADUserDetail usrDetail = null;

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.pkId == requestId);

                
                if (req.submittedBy != submitterId)
                {
                    req.submittedBy = submitterId;
                    change = true;
                }

                
                if (req.userId != userId)
                {
                    req.userId = userId;
                    usrDetail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(userId);
                    req.userTitle = usrDetail.Title;
                    change = true;
                }
                
                if (req.userDisplayName != userName)
                {
                    req.userDisplayName = userName;
                    change = true;
                }

                if (req.managerUserId != mgrId)
                {
                    req.managerUserId = mgrId;
                    change = true;
                }

                if (req.managerDisplayName != mgrName)
                {
                    req.managerDisplayName = mgrName;
                    change = true;
                }
                

                if (change)
                {
                    req.lastModifiedDate = DateTime.Now;
                    db.SubmitChanges();
                }
            }

        }

    }
}