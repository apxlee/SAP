using System;
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
        private string managerUserId = string.Empty;
        private int secondManagerActorId = 0;
        private string secondMagerUserId = string.Empty;
        private int teamApprovalActorId = 0;
        private string teamApprovalUserId = string.Empty;
        private int windowsServerActorId = 0;
        private string WindowsServerUserId = string.Empty;
        private int networkShareActorId = 0;
        private string networkShareUserId = string.Empty;
        private int databaseActorId = 0;
        private string databaseUserId = string.Empty;

        
        [TestFixtureSetUp]
        public void Init()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var actors = db.SNAP_Actors.Where(a => a.actor_groupId == TEAMAPPROVALGROUP).ToList();
                teamApprovalActorId = actors[0].pkId;
                teamApprovalUserId = actors[0].userId;
                actors = db.SNAP_Actors.Where(a => a.actor_groupId == TECHNICALAPPROVALGROUP).ToList();
                windowsServerActorId = actors[0].pkId;
                WindowsServerUserId = actors[0].userId;
                networkShareActorId = actors[1].pkId;
                networkShareUserId = actors[1].userId;
                databaseActorId = actors[2].pkId;
                databaseUserId = actors[2].userId;
                actors = db.SNAP_Actors.Where(a => a.actor_groupId == MANAGERGROUP).ToList();
                managerActorId = actors[0].pkId;
                managerUserId = actors[0].userId;
                secondManagerActorId = actors[1].pkId;
                secondMagerUserId = actors[1].userId;
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
                Assert.IsTrue(states.Count(x => x.workflowStatusEnum == (byte) WorkflowState.Pending_Acknowledgement) == states.Count(x => x.workflowStatusEnum == (byte) WorkflowState.Pending_Workflow));
                
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

        private void verifyWorkflowTransition(SNAP_Workflow wf, WorkflowState fr, WorkflowState to)
        {
            verifyWorkflowStateComplete(wf, fr);
            verifyWorkflowState(wf, to);            
        }

        private void removeManagerActorByUserId(string mgrUid)
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                try
                {
                    var groupId =
                        db.SNAP_Actor_Groups.Single(g => g.actorGroupType == (byte)ActorGroupType.Manager).pkId;
                    var toRemoveActor = db.SNAP_Actors.Single(a => a.userId == mgrUid && a.actor_groupId == groupId);

                    db.SNAP_Actors.DeleteOnSubmit(toRemoveActor);

                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                }
            }
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
            var accessTeamId = 1;
            for (int i = 0; i < 5; i++)
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                    var accessReq = new AccessRequest(req.pkId);
                    accessReq.Ack();
                    //accessReq.RequestToChange("Please change it");
                    accessReq.RequestToChange(accessTeamId, "Please change it");
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
                    Assert.IsTrue(req.SNAP_Workflows[0].SNAP_Workflow_States.Count(s => s.workflowStatusEnum == (byte) WorkflowState.Pending_Acknowledgement) > cnt);
                }
            }
        }


        [Test]
        public void ShouldFailToCreateWorkflowByAccessTeamDueToMissingManagerApprover()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);

                accessReq.Ack();

                Assert.IsFalse(accessReq.CreateWorkflow(new List<int>() { windowsServerActorId, databaseActorId }));

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

                verifyWorkflowState(accessTeamWF, WorkflowState.Workflow_Created);
                var managerWF = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == managerActorId);
                var managerWFStates = db.SNAP_Workflow_States.Where(x => x.workflowId == managerWF.pkId);
                foreach (var s in managerWFStates)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," + ((s.completedDate != null) ? s.completedDate.ToString() : "TBD"));
                }
                verifyWorkflowStateComplete(managerWF, WorkflowState.Not_Active);
                verifyWorkflowState(managerWF, WorkflowState.Pending_Approval);
            }
        }


        [Test]
        public void ShouldCreateWorkflowByAccessTeamAndThenMakeModification()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);

                accessReq.Ack();

                accessReq.CreateWorkflow(new List<int>() { managerActorId, teamApprovalActorId, windowsServerActorId });
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);

                accessReq.EditWorkflow(managerUserId, new List<int>() { windowsServerActorId, databaseActorId, networkShareActorId });
                //accessReq.CreateWorkflow(new List<int>() { managerActorId });


                
                var accessTeamWF = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == accessTeamActorId);
                var accessTeamWFStates = db.SNAP_Workflow_States.Where(x => x.workflowId == accessTeamWF.pkId);
                foreach (var s in accessTeamWFStates)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," + ((s.completedDate != null) ? s.completedDate.ToString() : "TBD"));
                }

                verifyWorkflowState(accessTeamWF, WorkflowState.Workflow_Created);
                var managerWF = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == managerActorId);
                var managerWFStates = db.SNAP_Workflow_States.Where(x => x.workflowId == managerWF.pkId);
                foreach (var s in managerWFStates)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," + ((s.completedDate != null) ? s.completedDate.ToString() : "TBD"));
                }
                verifyWorkflowStateComplete(managerWF, WorkflowState.Not_Active);
                verifyWorkflowState(managerWF, WorkflowState.Pending_Approval);

                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                Assert.IsTrue(wfs.Count == 0);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(wfs.Count == 3);

                foreach(var wf in wfs)
                {
                    verifyWorkflowState(wf, WorkflowState.Not_Active);
                }
            }

        }

        [Test]
        public void ShouldCreateWorkflowByAccessTeamAndThenMakeManagerModification()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);

                accessReq.Ack();

                accessReq.CreateWorkflow(new List<int>() { managerActorId, teamApprovalActorId, windowsServerActorId });
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);

                accessReq.EditWorkflow(secondMagerUserId, new List<int>()
                                                              {
                                                                  //secondManagerActorId,
                                                                  windowsServerActorId,
                                                                  databaseActorId,
                                                                  networkShareActorId
                                                              });



                var accessTeamWF = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == accessTeamActorId);
                var accessTeamWFStates = db.SNAP_Workflow_States.Where(x => x.workflowId == accessTeamWF.pkId);
                foreach (var s in accessTeamWFStates)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," +
                                      ((s.completedDate != null) ? s.completedDate.ToString() : "TBD"));
                }

                verifyWorkflowState(accessTeamWF, WorkflowState.Workflow_Created);
                var managerWF = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == secondManagerActorId);
                var managerWFStates = db.SNAP_Workflow_States.Where(x => x.workflowId == managerWF.pkId);
                foreach (var s in managerWFStates)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," +
                                      ((s.completedDate != null) ? s.completedDate.ToString() : "TBD"));
                }
                verifyWorkflowStateComplete(managerWF, WorkflowState.Not_Active);
                verifyWorkflowState(managerWF, WorkflowState.Pending_Approval);

                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                Assert.IsTrue(wfs.Count == 0);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(wfs.Count == 3);

                foreach (var wf in wfs)
                {
                    wf.SNAP_Workflow_States.Single(
                        s =>
                        s.completedDate == null && s.notifyDate == null &&
                        s.workflowStatusEnum == (byte) WorkflowState.Not_Active);
                }

            }

        }

        [Test]
        public void ShouldCreateWorkflowByAccessTeamUsingMgrUsrId()
        {
            var mgrUid = "pxlee";
            removeManagerActorByUserId(mgrUid);

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);

                accessReq.Ack();

                accessReq.CreateWorkflow(mgrUid, new List<int>() { teamApprovalActorId });

                var mgrGroupId = db.SNAP_Actor_Groups.Single(g => g.actorGroupType == (byte) ActorGroupType.Manager).pkId;
                var newManagerActorId = db.SNAP_Actors.Single(a => a.userId == mgrUid && a.actor_groupId == mgrGroupId).pkId;

                var accessTeamWF = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == accessTeamActorId);
                var accessTeamWFStates = db.SNAP_Workflow_States.Where(x => x.workflowId == accessTeamWF.pkId);
                foreach (var s in accessTeamWFStates)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," + ((s.completedDate != null) ? s.completedDate.ToString() : "TBD"));
                }

                verifyWorkflowState(accessTeamWF, WorkflowState.Workflow_Created);

                var managerWF = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == newManagerActorId);
                var managerWFStates = db.SNAP_Workflow_States.Where(x => x.workflowId == managerWF.pkId);
                foreach (var s in managerWFStates)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," + ((s.completedDate != null) ? s.completedDate.ToString() : "TBD"));
                }
                verifyWorkflowStateComplete(managerWF, WorkflowState.Not_Active);
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
                accessReq.CreateWorkflow(new List<int>() { teamApprovalActorId, managerActorId });

                var wfs = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Manager);
                Assert.IsTrue(wfs.Count == 1);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                Assert.IsTrue(wfs.Count == 1);

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
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Not_Active);
                verifyWorkflowTransition(wfs[0], WorkflowState.Not_Active, WorkflowState.Pending_Approval);
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

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                verifyWorkflowTransition(wfs[0], WorkflowState.Not_Active, WorkflowState.Pending_Approval);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                verifyWorkflowState(wfs[0], WorkflowState.Not_Active);
                verifyWorkflowState(wfs[1], WorkflowState.Not_Active);
                verifyWorkflowState(wfs[2], WorkflowState.Not_Active);

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
                verifyWorkflowTransition(wfs[0], WorkflowState.Pending_Approval, WorkflowState.Approved);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                verifyWorkflowTransition(wfs[0], WorkflowState.Pending_Approval, WorkflowState.Approved);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                verifyWorkflowTransition(wfs[0], WorkflowState.Not_Active, WorkflowState.Pending_Approval);
                verifyWorkflowTransition(wfs[1], WorkflowState.Not_Active, WorkflowState.Pending_Approval);
                verifyWorkflowTransition(wfs[2], WorkflowState.Not_Active, WorkflowState.Pending_Approval);
            }

        }


        [Test]
        public void ShouldHandleonlyManagerApproval()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                             });

                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                verifyWorkflowTransition(wfs[0], WorkflowState.Pending_Approval, WorkflowState.Approved);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                verifyWorkflowTransition(wfs[0], WorkflowState.Workflow_Created, WorkflowState.Approved);


                /*
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                verifyWorkflowTransition(wfs[0], WorkflowState.Pending_Approval, WorkflowState.Approved);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                verifyWorkflowTransition(wfs[0], WorkflowState.Not_Active, WorkflowState.Pending_Approval);
                verifyWorkflowTransition(wfs[1], WorkflowState.Not_Active, WorkflowState.Pending_Approval);
                verifyWorkflowTransition(wfs[2], WorkflowState.Not_Active, WorkflowState.Pending_Approval);
                 */
            }

        }

        [Test]
        public void ShouldHandleFromManagerToTeamApproveAndModifyTechApprover()
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

                accessReq.EditWorkflow(managerUserId, new List<int>()
                                             {
                                                 teamApprovalActorId,
                                                 //windowsServerActorId,
                                                 databaseActorId,
                                                 //networkShareActorId
                                             });

            }

            
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                verifyWorkflowTransition(wfs[0], WorkflowState.Pending_Approval, WorkflowState.Approved);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                verifyWorkflowTransition(wfs[0], WorkflowState.Pending_Approval, WorkflowState.Approved);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                verifyWorkflowTransition(wfs[0], WorkflowState.Not_Active, WorkflowState.Pending_Approval);
                Assert.IsTrue(wfs[0].actorId == databaseActorId);
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
                var accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Workflow_Created);

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
                //ystem.Threading.Thread.Sleep(90000);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approval
                //System.Threading.Thread.Sleep(90000);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get all technical approval

                // !!! for individual test, please uncommnet it
                // !!! comment it out to save time

                //System.Threading.Thread.Sleep(90000);
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
                var accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Workflow_Created);

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
        public void ShouldGetPendingActorId()
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

                Assert.IsTrue(accessReq.GetPendingApprovalActorId()[0] == managerActorId);
                Assert.IsTrue(accessReq.GetPendingApprovalActorType() == (byte)ActorApprovalType.Manager);

                // get manager approal
                //ystem.Threading.Thread.Sleep(90000);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                Assert.IsTrue(accessReq.GetPendingApprovalActorId()[0] == teamApprovalActorId);
                Assert.IsTrue(accessReq.GetPendingApprovalActorType() == (byte)ActorApprovalType.Team_Approver);

                // get team approval
                //System.Threading.Thread.Sleep(90000);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                Assert.IsTrue(accessReq.GetPendingApprovalActorId().Contains(windowsServerActorId));
                Assert.IsTrue(accessReq.GetPendingApprovalActorType() == (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(accessReq.GetPendingApprovalActorId().Contains(networkShareActorId));
                Assert.IsTrue(accessReq.GetPendingApprovalActorType() == (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(accessReq.GetPendingApprovalActorId().Contains(databaseActorId));
                Assert.IsTrue(accessReq.GetPendingApprovalActorType() == (byte)ActorApprovalType.Technical_Approver);
                // get all technical approval

                // !!! for individual test, please uncommnet it
                // !!! comment it out to save time

                //System.Threading.Thread.Sleep(90000);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);
                Assert.IsFalse(accessReq.GetPendingApprovalActorId().Contains(wfs[0].actorId));
                accessReq.WorkflowAck(wfs[1].pkId, WorkflowAction.Approved);
                Assert.IsFalse(accessReq.GetPendingApprovalActorId().Contains(wfs[1].actorId));
                accessReq.WorkflowAck(wfs[2].pkId, WorkflowAction.Approved);
                Assert.IsTrue(accessReq.GetPendingApprovalActorId().Count == 0);

            }

        }

        [Test]
        public void ShouldHandleAccessTeamCloseAfterAllApproved()
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

                accessReq.NoAccess(WorkflowAction.Cancel, "Please close it");

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Closed_Cancelled);
                verifyWorkflowComment(wfs[0], CommentsType.Cancelled);
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
                var techApprovers = new List<int>() { 
                                       windowsServerActorId,
                                       databaseActorId,
                                       networkShareActorId
                                    };
                var actorIds = new List<int>()
                                   {
                                       managerActorId,
                                       teamApprovalActorId,
                                   };
                actorIds.AddRange(techApprovers);
                accessReq.CreateWorkflow(actorIds);


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
                techApprovers.Remove(wfs[0].actorId);
                accessReq.WorkflowAck(wfs[1].pkId, WorkflowAction.Approved);
                techApprovers.Remove(wfs[1].actorId);
                //accessReq.WorkflowAck(wfs[2].pkId, WorkflowAction.Change, "change it");
                accessReq.RequestToChange(techApprovers[0], "change it");

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

        [Test]
        public void ShouldHandleUpdateCreateWorkFlowByAddingNewApprovals()
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
                                                 //databaseActorId,
                                                 //networkShareActorId
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
                //accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);
                //accessReq.WorkflowAck(wfs[1].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Change, "change it");

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Change_Requested);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                //verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);
                //verifyWorkflowStateComplete(wfs[1], WorkflowState.Approved);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Change_Requested);

                verifyWorkflowComment(wfs[0], CommentsType.Requested_Change);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Change_Requested).completedDate == null);

            }

            // recreate wf

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

                // get technical approval, but the last one request to change
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(wfs.Count == 3);
                
            }
        }

        [Test]
        public void ShouldHandleUpdateCreateWorkFlowByRemovingApproval()
        {
            int goneWFid = 0;
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
                                                 //databaseActorId,
                                                 //networkShareActorId
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
                goneWFid = wfs[0].pkId;
                //accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);
                //accessReq.WorkflowAck(wfs[1].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Change, "change it");

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Change_Requested);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                //verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);
                //verifyWorkflowStateComplete(wfs[1], WorkflowState.Approved);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Change_Requested);

                verifyWorkflowComment(wfs[0], CommentsType.Requested_Change);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Change_Requested).completedDate == null);

            }


            // recreate wf
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
                                             //windowsServerActorId,
                                             databaseActorId,
                                             networkShareActorId
                                         });

                // get technical approval, but the last one request to change
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(wfs.Count == 2);

                var noSuchWorkflow = db.SNAP_Workflows.Where(w => w.actorId == windowsServerActorId);
                Assert.IsTrue(noSuchWorkflow.Count() == 0);

                var noSuchWorkflowState = db.SNAP_Workflow_States.Where(s => s.workflowId == goneWFid);
                Assert.IsTrue(noSuchWorkflowState.Count() == 0);

                var noSuchWorkflowComment = db.SNAP_Workflow_Comments.Where(c=> c.workflowId == goneWFid);
                Assert.IsTrue(noSuchWorkflowComment.Count() == 0);


            }
        }



        [Test]
        public void ShouldHandleUpdateCreateWorkFlowByRemovingTechApproval()
        {
            int goneWFid = 0;
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
                                                 //databaseActorId,
                                                 //networkShareActorId
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
                goneWFid = wfs[0].pkId;


                // get technical approval, but the last one request to change
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                //accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);
                //accessReq.WorkflowAck(wfs[1].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Change, "change it");

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Change_Requested);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                //verifyWorkflowStateComplete(wfs[0], WorkflowState.Approved);
                //verifyWorkflowStateComplete(wfs[1], WorkflowState.Approved);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Change_Requested);

                verifyWorkflowComment(wfs[0], CommentsType.Requested_Change);
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Change_Requested).completedDate == null);

            }


            // recreate wf
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);

                accessReq.RequestChanged();
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                         {
                                             managerActorId,
                                             //teamApprovalActorId,
                                             windowsServerActorId,
                                             //databaseActorId,
                                             //networkShareActorId
                                         });

                // get technical approval, but the last one request to change
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(wfs.Count == 1);

                /*
                var noSuchWorkflow = db.SNAP_Workflows.Where(w => w.actorId == teamApprovalActorId);
                Assert.IsTrue(noSuchWorkflow.Count() == 0);
                */

                var noSuchWorkflow = req.SNAP_Workflows.Where(w => w.actorId == teamApprovalActorId);
                Assert.IsTrue(noSuchWorkflow.Count() == 0);


                var noSuchWorkflowState = db.SNAP_Workflow_States.Where(s => s.workflowId == goneWFid);
                //var noSuchWorkflowState = reNAP_Workflow_States.Where(s => s.workflowId == goneWFid);
                Assert.IsTrue(noSuchWorkflowState.Count() == 0);

                var noSuchWorkflowComment = db.SNAP_Workflow_Comments.Where(c => c.workflowId == goneWFid);
                Assert.IsTrue(noSuchWorkflowComment.Count() == 0);


            }
        }

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

            for (int i = 0; i < 5; i++)
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
                                                 //databaseActorId,
                                                 networkShareActorId
                                             });


                    var accessWF = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                    var state = accessWF[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Workflow_Created
                        && s.completedDate == null); // get lastest 'pending approval' for the workflowadmin state

                    // get manager approal
                    var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                    //Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                    accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                    Assert.IsTrue(
                        wfs[0].SNAP_Workflow_States.Count(
                            s => s.completedDate != null
                                && s.workflowStatusEnum == (byte)WorkflowState.Approved
                                && s.pkId > state.pkId) == 1);

                    // get team approval
                    wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);

                    accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                    Assert.IsTrue(
                            wfs[0].SNAP_Workflow_States.Count(
                                s => s.completedDate != null
                                    && s.workflowStatusEnum == (byte)WorkflowState.Approved
                                    && s.pkId > state.pkId) == 1);

                    var r = new Random();
                    //var last = r.Next(2);
                    var last = r.Next(1);
                    //var last = 0;
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
        
        //[Ignore]
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
                // login to to http://awhdht02.devapollogrp.edu/CAisd/pdmweb.exe
                // sign in as svc_cap/S31H9D&amp;2j6
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

        //[Ignore]
        [Test]
        public void ShouldHandleAccessTeamCloseAfterCreateTicket()
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
                accessReq.CreateServiceDeskTicket();
                accessReq.NoAccess(WorkflowAction.Cancel, "Please close it");

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Closed_Cancelled);
                verifyWorkflowComment(wfs[0], CommentsType.Cancelled);
            }
        }

        //[Ignore]
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
        public void ShouldHandleFinalizeRequestWithSDticket()
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
                //accessReq.CreateServiceDeskTicket();

                // finalize it
                accessReq.FinalizeRequest();

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                //verifyWorkflowStateComplete(wfs[0], WorkflowState.Pending_Provisioning);
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
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Workflow_Created);
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
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Workflow_Created);
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
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Workflow_Created);
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
                verifyWorkflowStateComplete(wfs[0], WorkflowState.Workflow_Created);
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


        [Test] public void ShouldReturnActorId()
        {
            string usrid = "pxlee";

            //g.actorGroupType == (byte)ActorGroupType.Manager
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var c =
                        db.SNAP_Actors.Count(
                            a => a.userId == usrid && a.SNAP_Actor_Group.actorGroupType == (byte) ActorGroupType.Manager);
                    if (c == 0)
                    {
                        db.SNAP_Actors.InsertOnSubmit(new SNAP_Actor()
                                                          {
                                                              actor_groupId = 4,
                                                              displayName = "Pong Lee",
                                                              emailAddress = "pxlee@apollogrp.edu",
                                                              isActive = true,
                                                              isDefault = false
                                                          });
                        db.SubmitChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                
            }
            removeManagerActorByUserId(usrid);
            Assert.IsTrue(ApprovalWorkflow.GetActorIdByUserId(ActorGroupType.Manager, usrid) != 0);
        }

        [Test]
        public void ShouldReturnZeroActorId()
        {
            string usrid = "pxleepxlee";

            removeManagerActorByUserId(usrid);
            Assert.IsTrue(ApprovalWorkflow.GetActorIdByUserId(ActorGroupType.Manager, usrid) == 0);
        }

        [Test]
        public void ShouldReturnWorkflowIdByFromRequestIDAndUsrId()
        {
            SNAP_Request req;
            SNAP_Workflow wf;
            AccessRequest accessReq;

            using (var db = new SNAPDatabaseDataContext())
            {
                req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>() {managerActorId, teamApprovalActorId, windowsServerActorId,databaseActorId,networkShareActorId});

                //Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, managerUserId) == managerActorId);
                Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, managerUserId) != 0);
                // they are not active approver yet
                Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, teamApprovalUserId) == 0);
                Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, WindowsServerUserId) == 0);

                wf = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Manager)[0];
                accessReq.WorkflowAck(wf.pkId, WorkflowAction.Approved);
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                // this is the only active approver
                //Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, teamApprovalUserId) == teamApprovalActorId);
                Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, teamApprovalUserId) != 0);
                // this are not active approver
                //Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, managerUserId) == 0);
                //Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, WindowsServerUserId) == 0);
            }


            using (var db = new SNAPDatabaseDataContext())
            {
                req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                wf = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Team_Approver)[0];
                accessReq.WorkflowAck(wf.pkId, WorkflowAction.Approved);
            }

            using (var db = new SNAPDatabaseDataContext()) {
                // these are active approvers
                /*
                Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, WindowsServerUserId) == windowsServerActorId);
                Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, networkShareUserId) == networkShareActorId);
                Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, databaseUserId) == databaseActorId);
                */
                Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, WindowsServerUserId) != 0);
                Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, networkShareUserId) != 0);
                Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, databaseUserId) != 0);

                // there are not active approvers
                //Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, managerUserId) == 0);
                //Assert.IsTrue(db.GetActiveWorkflowId(req.pkId, teamApprovalUserId) == 0);
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
