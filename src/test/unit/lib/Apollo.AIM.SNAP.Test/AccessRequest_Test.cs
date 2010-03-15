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
        private int managerActorId = 2;
        private int windowsServerActorId = 3;
        private int networkShareActorId = 4;
        private int databaseActorId = 5;

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
                Assert.IsTrue(
                    states.Count(x => x.workflowStatusEnum == (byte) WorkflowState.Pending_Acknowlegement) 
                    == states.Count(x => x.workflowStatusEnum == (byte) WorkflowState.Pending_Workflow));
                
            }
        }


        [Test]
        public void ShouldCloseDeniedByAccessTeam() {}

        [Test]
        public void ShouldCloseCancelByAccessTeam() { }

        
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
         


    }
}
