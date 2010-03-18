using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Apollo.AIM.SNAP.Model;
using NUnit.Framework;

namespace Apollo.AIM.SNAP.Test
{
    [TestFixture]
    public class AccessRequest_Test
    {
        private int accessTeamActorId = 1;
        private int managerActorId = 29;
        private int teamApprovalActorId = 17;
        private int windowsServerActorId = 19;
        private int networkShareActorId = 20;
        private int databaseActorId = 21;

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

                db.usp_insert_request_xml(xmlInput, "UnitTester", "UnitTester", "Pong Lee", "Programmer", "gjbelang", "Greg Belanger");

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

                var reqs = db.SNAP_Requests.Where(x => x.userId == "UnitTester").ToList();
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
                }
                db.SubmitChanges();
            }

        }
        [Test]
        public void ShouldAckByAccessTeam()
        {
            
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Pending);
            }
        }


        [Test]
        public void ShouldCloseDeniedByAccessTeam()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.NoAccess(WorkflowAction.Denied, "Deny");
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                Assert.IsTrue(req.SNAP_Workflows[0].SNAP_Workflow_States.Count(s => s.workflowStatusEnum == (byte) WorkflowState.Closed_Denied) == 1);
                Assert.IsTrue(req.SNAP_Workflows[0].SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte)CommentsType.Denied) == 1);
            }
            
        }

        [Test]
        public void ShouldCloseCancelledByAccessTeam()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.NoAccess(WorkflowAction.Cancel, "Cancel");
            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                Assert.IsTrue(req.SNAP_Workflows[0].SNAP_Workflow_States.Count(s => s.workflowStatusEnum == (byte)WorkflowState.Closed_Cancelled) == 1);
                Assert.IsTrue(req.SNAP_Workflows[0].SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte)CommentsType.Cancelled) == 1);
            }

        }

        [Test]
        public void ShouldRequestToChangeByAccessTeanMultipleTimes()
        {
            for (int i = 0; i < 5; i++)
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

                    var accessReq = new AccessRequest(req.pkId);
                    accessReq.Ack();
                    accessReq.RequestToChange("Please change it");
                }

                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                    Assert.IsTrue(req.statusEnum == (byte) RequestState.Change_Requested);
                    Assert.IsTrue(req.SNAP_Workflows[0].SNAP_Workflow_States.Count(s => s.workflowStatusEnum == (byte) WorkflowState.Change_Requested) > i);
                    Assert.IsTrue(req.SNAP_Workflows[0].SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte) CommentsType.Requested_Change) > i);
                }


                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                    var accessReq = new AccessRequest(req.pkId);
                    accessReq.RequestChanged();
                }


                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);

                accessReq.Ack();

                accessReq.CreateWorkflow(new List<int>() { managerActorId });

                
                var accessTeamWF = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == accessTeamActorId);
                var accessTeamWFStates = db.SNAP_Workflow_States.Where(x => x.workflowId == accessTeamWF.pkId);
                foreach (var s in accessTeamWFStates)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," + ((s.completedDate != null) ? s.completedDate.ToString() : "TBD"));
                }

                Assert.IsTrue(accessTeamWFStates.Count(s => s.workflowStatusEnum == (byte) WorkflowState.Pending_Approval) == 1);

                var managerWF = db.SNAP_Workflows.Single(x => x.requestId == req.pkId && x.actorId == managerActorId);
                var managerWFStates = db.SNAP_Workflow_States.Where(x => x.workflowId == managerWF.pkId);
                foreach (var s in managerWFStates)
                {
                    Console.WriteLine(s.workflowId + "," + s.workflowStatusEnum + "," + ((s.completedDate != null) ? s.completedDate.ToString() : "TBD"));
                }
                Assert.IsTrue(managerWFStates.Count(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval) == 1);

            }
        }
         
        [Test]
        public void ShouldReturnManagerWorkflowApprovalType()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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

                accessReq.InformApproverForAction();


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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 //teamApprovalActorId,
                                                 //windowsServerActorId,
                                                 //databaseActorId,
                                                 //networkShareActorId
                                             });

                accessReq.InformApproverForAction();

                var wfs = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Manager);

                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);
                
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);


                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte) WorkflowState.Pending_Approval).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte) WorkflowState.Approved).completedDate != null);
            }


        }

        [Test]
        public void ShouldHandleFromManagerToTeamApprove()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
                accessReq.CreateWorkflow(new List<int>()
                                             {
                                                 managerActorId,
                                                 teamApprovalActorId,
                                                 //windowsServerActorId,
                                                 //databaseActorId,
                                                 //networkShareActorId
                                             });

                accessReq.InformApproverForAction();

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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Approved).completedDate != null);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Approved).completedDate != null);

            }


        }


        [Test]
        public void ShouldHandleFromManagerToTeamToTechicalApprove()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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

                accessReq.InformApproverForAction();

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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Approved).completedDate != null);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Approved).completedDate != null);

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Approved).completedDate != null);


            }


        }



        [Test]
        public void ShouldHandleFromManagerToTeamToOneTechicalApprove()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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

                accessReq.InformApproverForAction();

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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);

                var accessTeamWF = req.SNAP_Workflows.Single(w => w.actorId == 1); // actid = 1 => accessTeam
                var accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval);

                Assert.IsTrue(accessTeamState.completedDate == null);  // can't complete this yet, need to wait for all technical approvals

            }


        }


        [Test]
        public void ShouldHandleFromManagerToTeamToAllTechicalApprove()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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

                accessReq.InformApproverForAction();

                // get manager approal
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);

                // get team approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);


                // get all technical approval
                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[1].pkId, WorkflowAction.Approved);
                accessReq.WorkflowAck(wfs[2].pkId, WorkflowAction.Approved);


            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);

                //var accessTeamWF = req.SNAP_Workflows.Single(w => w.actorId == 1); // actid = 1 => accessTeam
                var accessTeamWF = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin)[0]; // there is only on workflow admin
                var accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval);

                Assert.IsTrue(accessTeamState.completedDate != null);  // all technical approval received, complete it

                accessTeamState = accessTeamWF.SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Approved);
                Assert.IsTrue(accessTeamState.completedDate == null);  // all technical approval received, complete it
            }

        }

        [Test]
        public void ShouldHandleCreateSDTicket()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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

                accessReq.InformApproverForAction();

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

            }

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Approved).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Provisioning).completedDate == null);

            }

        }

        [Test]
        public void ShouldHandleFinalizeRequest()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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

                accessReq.InformApproverForAction();

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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Provisioning).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Closed_Completed).completedDate != null);
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
            }

        }


        [Test]
        public void ShouldHandleManagerDeniedRequest()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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

                accessReq.InformApproverForAction();

                // get manager disapproval
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Manager);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate == null);

                accessReq.WorkflowAck(wfs[0].pkId, WorkflowAction.Denied, "Bad Request");

            }

            
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Closed_Denied).completedDate != null);
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                var wf = accessReq.FindApprovalTypeWF(db, (byte) ActorApprovalType.Manager)[0];
                Assert.IsTrue(wf.SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte) CommentsType.Denied) > 0);
            }
            
        }

        [Test]
        public void ShouldHandleTeamApprovalDeniedRequest()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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

                accessReq.InformApproverForAction();

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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Closed_Denied).completedDate != null);
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                var wf = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Team_Approver)[0];
                Assert.IsTrue(wf.SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte)CommentsType.Denied) > 0);
            }

        }

        [Test]
        public void ShouldHandleTechicalApprovalDeniedRequest()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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

                accessReq.InformApproverForAction();

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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Closed_Denied).completedDate != null);
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                var wf = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver)[0];
                Assert.IsTrue(wf.SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte)CommentsType.Denied) > 0);
            }

        }

        [Test]
        public void ShouldHandleLastTechicalApprovalDeniedRequest()
        {
            int denier = 2;

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");

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

                accessReq.InformApproverForAction();

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
                var req = db.SNAP_Requests.Single(x => x.userId == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                var wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Workflow_Admin);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Pending_Approval).completedDate != null);
                Assert.IsTrue(wfs[0].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Closed_Denied).completedDate != null);
                

                wfs = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(wfs[denier].SNAP_Workflow_States.Single(s => s.workflowStatusEnum == (byte)WorkflowState.Closed_Denied).completedDate != null);

                Assert.IsTrue(req.statusEnum == (byte)RequestState.Closed);
                var wfStates = accessReq.FindApprovalTypeWF(db, (byte)ActorApprovalType.Technical_Approver);
                Assert.IsTrue(wfStates[denier].SNAP_Workflow_Comments.Count(c => c.commentTypeEnum == (byte)CommentsType.Denied) == 1);
            }
            
        }

    }
}
