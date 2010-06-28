using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Test
{
    [TestFixture]
    public class SNAPDatabaseCall_Test
    {
        // this is from actor group table
        private static int TEAMAPPROVALGROUP = 0;
        private static int TECHNICALAPPROVALGROUP = 1;
        private static int MANAGERGROUP = 2;



        //private int accessTeamActorId = 1;
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
                var actors = db.SNAP_Actors.Where(a => a.SNAP_Actor_Group.actorGroupType == TEAMAPPROVALGROUP).ToList();
                teamApprovalActorId = actors[0].pkId;
                teamApprovalUserId = actors[0].userId;
                //actors = db.SNAP_Actors.Where(a => a.actor_groupId == TECHNICALAPPROVALGROUP).ToList();
                actors = db.SNAP_Actors.Where(a => a.SNAP_Actor_Group.actorGroupType == TECHNICALAPPROVALGROUP).ToList();
                windowsServerActorId = actors[0].pkId;
                WindowsServerUserId = actors[0].userId;
                networkShareActorId = actors[1].pkId;
                networkShareUserId = actors[1].userId;
                databaseActorId = actors[2].pkId;
                databaseUserId = actors[2].userId;
                //actors = db.SNAP_Actors.Where(a => a.actor_groupId == MANAGERGROUP).ToList();
                actors = db.SNAP_Actors.Where(a => a.SNAP_Actor_Group.actorGroupType == MANAGERGROUP).ToList();
                if (actors.Count == 1)
                {
                    db.SNAP_Actors.InsertOnSubmit(new SNAP_Actor() { 
                        actor_groupId = actors[0].actor_groupId,
                        userId = "pxlee",
                        displayName = "Pong Lee",
                        emailAddress = "pxlee@apollogrp.edu",
                        isActive = true,
                        isDefault = false
                    });

                    db.SubmitChanges();
                    actors = db.SNAP_Actors.Where(a => a.SNAP_Actor_Group.actorGroupType == MANAGERGROUP).ToList();
                }                
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
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                db.SNAP_Request_Comments.InsertOnSubmit(new SNAP_Request_Comment()
                {
                    requestId = req.pkId,
                    commentText = "Test Comment",
                    commentTypeEnum = (byte)CommentsType.Acknowledged,
                    createdDate = DateTime.Now
                });
                db.SubmitChanges();
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
        public void ShouldReturnAllRequests()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var t1 = DateTime.Now;
                Console.WriteLine("Access Team Requests:") ;
                db.GetAllRequests("pxlee", "accessteam");
                
                Console.WriteLine("\tOpen Requests:");
                output(db.OpenRquests);
                Console.WriteLine("\tClose Requests:");
                output(db.CloseRquests);

                Console.WriteLine("\n======");
                Console.WriteLine("My Requests:");
                db.GetAllRequests("pxlee", "my");
                Console.WriteLine("\tOpen Requests:");
                output(db.OpenRquests);
                Console.WriteLine("\tClose Requests:");
                output(db.CloseRquests);

                Console.WriteLine("\n======");
                Console.WriteLine("Approval Requests:");
                db.GetAllRequests("pxlee", "approval");
                Console.WriteLine("\tOpen Requests:");
                output(db.OpenRquests);
                Console.WriteLine("\tClose Requests:");
                output(db.CloseRquests);
                 
                var t2 = DateTime.Now;
                Console.WriteLine("Duration: " + (t2-t1).Duration());

            }    

        }
        
        private void output(Dictionary<string, object> data)
        {
            var reqDetails = (List<usp_open_my_request_detailsResult>)data["reqDetails"];
            Console.WriteLine("\t\tReq details cnt: " + reqDetails.Count);

            
            foreach (var d in reqDetails)
            {
                Console.WriteLine(d.pkId + "," + d.statusEnum);
            }
            var texts = (List<SNAP_Access_User_Text>)data["reqText"];
            Console.WriteLine("\t\tReq texts cnt: " + texts.Count);
            foreach (SNAP_Access_User_Text list in texts)
            {
                //Console.WriteLine(list.access_details_formId + "," + list.userText);
            }
            
            var comments = (List<usp_open_my_request_commentsResult>)data["reqComments"];
            Console.WriteLine("\t\tReq comments cnt: " + comments.Count);
            foreach (usp_open_my_request_commentsResult list in comments)
            {
                //Console.WriteLine(list.pkId + "," + list.commentText);
            }
            var wfDetails = (List<usp_open_my_request_workflow_detailsResult>)data["wfDetails"];
            Console.WriteLine("\t\tWf details cnt: " + wfDetails.Count);
            foreach (usp_open_my_request_workflow_detailsResult list in wfDetails)
            {
                //Console.WriteLine(list.pkId+ "," + list.workflowStatusEnum);
            }
            var wfComments = (List<usp_open_my_request_workflow_commentsResult>)data["wfComments"];
            Console.WriteLine("\t\tWf comments cnt: " + wfComments.Count);
            foreach (var c in wfComments)
            {
                //Console.WriteLine(c.requestId + "," + c.commentText);
            }
            
            
        }
        [Test]
        public void ShouldReturnNoRequestData()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var maxRequestId = db.SNAP_Requests.Max(x => x.pkId);
                var data = db.RetrieveRequestUserText(++maxRequestId);
                Assert.IsTrue(data.Count() == 0);
            }

        }

        [Test]
        public void ShouldReturnRequestData()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var maxRequestId = db.SNAP_Requests.Max(x => x.pkId);
                var requestTexts = db.RetrieveRequestUserText(maxRequestId);

                foreach (var text in requestTexts)
                {
                    Console.WriteLine(text.userText);
                }
                Assert.IsTrue(requestTexts.Count() > 0);

            }
        }

        [Test]
        public void ShouldbeAbleToCallUDF()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
               var newDate = db.udf_get_next_business_day(DateTime.Now, 10);
                Assert.IsTrue(newDate != null);
            }
        }

        [Test]
        public void ShouldOk()
        {
            using(var db = new SNAPDatabaseDataContext())
            {
                DateTime current = DateTime.Parse("3/31/2010 1:00:00 PM");
                for (int i = 1; i < 20; i++)
                {
                    Console.WriteLine(db.udf_get_next_business_day(current, i));
                }
            }
        }

        [Test]
        public void RUD_SNAP_Access_Details_Form()
        {
            SNAP_Access_Details_Form test = new SNAP_Access_Details_Form();
            test.pkId = 0;
            test.parentId = 0;
            test.label = "test";
            test.isRequired = false;
            test.isActive = false;
            test.description = "This is a test";

            using (var db = new SNAPDatabaseDataContext())
            {
                db.SNAP_Access_Details_Forms.InsertOnSubmit(new SNAP_Access_Details_Form()
                {
                    parentId = 20,
                    label = "Test Field",
                    description = "For Unit Testing",
                    isActive = false,
                    isRequired = false
                });
                db.SubmitChanges();

                var test1 = db.SNAP_Access_Details_Forms.Where(t => t.label == "Test Field").ToList();
                Assert.IsTrue(test1.Count > 0);

                foreach (var row in test1)
                {
                    db.SNAP_Access_Details_Forms.DeleteOnSubmit(row);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Access_Details_Forms.Where(t => t.label == "Test Field").ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_SNAP_Access_User_Text()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                db.SNAP_Access_User_Texts.InsertOnSubmit(new SNAP_Access_User_Text()
                {
                    requestId = 0,
                    access_details_formId = 20,
                    userText = "For unit testing",
                    modifiedDate = DateTime.Now
                });
                db.SubmitChanges();

                var test1 = db.SNAP_Access_User_Texts.Where(t => t.requestId == 0).ToList();
                Assert.IsTrue(test1.Count > 0);

                foreach (var row in test1)
                {
                    db.SNAP_Access_User_Texts.DeleteOnSubmit(row);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Access_User_Texts.Where(t => t.requestId == 0).ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_SNAP_Actor()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                db.SNAP_Actors.InsertOnSubmit(new SNAP_Actor()
                {
                    userId = "testuser",
                    displayName = "Test User",
                    emailAddress = "testuser@apollogrp.edu",
                    isActive = false,
                    isDefault = false,
                    isGroup = false,
                    actor_groupId = 0
                });
                db.SubmitChanges();

                var test1 = db.SNAP_Actors.Where(t => t.userId == "testuser").ToList();
                Assert.IsTrue(test1.Count > 0);

                foreach (var row in test1)
                {
                    db.SNAP_Actors.DeleteOnSubmit(row);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Actors.Where(t => t.userId == "testuser").ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_SNAP_Actor_Group()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                db.SNAP_Actor_Groups.InsertOnSubmit(new SNAP_Actor_Group()
                {
                    groupName = "Test Group",
                    description = "Test Group Description",
                    actorGroupType = 0,
                    isActive = false,
                    isLargeGroup = false
                });
                db.SubmitChanges();

                var test1 = db.SNAP_Actor_Groups.Where(t => t.groupName == "Test Group").ToList();
                Assert.IsTrue(test1.Count > 0);

                foreach (var row in test1)
                {
                    db.SNAP_Actor_Groups.DeleteOnSubmit(row);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Actor_Groups.Where(t => t.groupName == "Test Group").ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_SNAP_Workflow_State_Type()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                db.SNAP_Workflow_State_Types.InsertOnSubmit(new SNAP_Workflow_State_Type()
                {
                    pkId = 20,
                    typeName = "TESTTYPE"
                });
                db.SubmitChanges();

                var test1 = db.SNAP_Workflow_State_Types.Where(t=>t.pkId==20).ToList();
                Assert.IsTrue(test1.Count > 0);

                foreach (var row in test1)
                {
                    db.SNAP_Workflow_State_Types.DeleteOnSubmit(row);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Workflow_State_Types.Where(t => t.pkId == 20).ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_SNAP_Comments_Type()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                db.SNAP_Comments_Types.InsertOnSubmit(new SNAP_Comments_Type()
                {
                    pkId = 20,
                    typeName = "TESTTYPE",
                    audience = "TESTAUDIENCE"
                });
                db.SubmitChanges();

                var test1 = db.SNAP_Comments_Types.Where(t => t.pkId == 20).ToList();
                Assert.IsTrue(test1.Count > 0);

                foreach (var row in test1)
                {
                    db.SNAP_Comments_Types.DeleteOnSubmit(row);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Comments_Types.Where(t => t.pkId == 20).ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_SNAP_Request()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                db.SNAP_Requests.InsertOnSubmit(new SNAP_Request()
                {
                    userId = "testuser",
                    userTitle = "Unit Tester",
                    userDisplayName = "Test Account",
                    managerUserId = "manager",
                    managerDisplayName = "Test Manager",
                    submittedBy = "submitter",
                    isChanged = false,
                    statusEnum = 0,
                    createdDate = DateTime.Now,
                    lastModifiedDate = DateTime.Now,
                    ticketNumber = "0000"
                });
                db.SubmitChanges();

                var test1 = db.SNAP_Requests.Where(t => t.userId == "testuser").ToList();
                Assert.IsTrue(test1.Count > 0);

                foreach (var row in test1)
                {
                    db.SNAP_Requests.DeleteOnSubmit(row);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Requests.Where(t => t.userId == "testuser").ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_Request_Comment()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                db.SNAP_Request_Comments.InsertOnSubmit(new SNAP_Request_Comment()
                {
                    requestId = 0,
                    commentText = "Test Comment",
                    commentTypeEnum = 0,
                    createdDate = DateTime.Now
                });
                db.SubmitChanges();

                var test1 = db.SNAP_Request_Comments.Where(t => t.commentText == "Test Comment").ToList();
                Assert.IsTrue(test1.Count > 0);

                foreach (var row in test1)
                {
                    db.SNAP_Request_Comments.DeleteOnSubmit(row);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Request_Comments.Where(t => t.commentText == "Test Comment").ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_SNAP_Request_State_Type()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                db.SNAP_Request_State_Types.InsertOnSubmit(new SNAP_Request_State_Type()
                {
                    pkId = 20,
                    typeName = "TESTTYPE"
                });
                db.SubmitChanges();

                var test1 = db.SNAP_Request_State_Types.Where(t => t.pkId == 20).ToList();
                Assert.IsTrue(test1.Count > 0);

                foreach (var row in test1)
                {
                    db.SNAP_Request_State_Types.DeleteOnSubmit(row);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Request_State_Types.Where(t => t.pkId == 20).ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        // TODO: SNAP_Weekends_and_Holiday doesnt have a primary key set
        public void RUD_SNAP_Weekends_and_Holiday()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                //db.SNAP_Weekends_and_Holidays.InsertOnSubmit(new SNAP_Weekends_and_Holiday()
                //{
                //    dayName = "TESTDAY",
                //    dayOfWeekDate = DateTime.Now
                //});
                //db.SubmitChanges();

                var test1 = db.SNAP_Weekends_and_Holidays.Where(t => t.dayName == "MON").ToList();
                Assert.IsTrue(test1.Count > 0);

                //foreach (var row in test1)
                //{
                //    db.SNAP_Weekends_and_Holidays.DeleteOnSubmit(row);
                //}
                //db.SubmitChanges();

                //var test2 = db.SNAP_Weekends_and_Holidays.Where(t => t.dayName == "TESTDAY").ToList();
                //Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        // TODO: SNAP_Actor_Group_Type doesnt have a primary key set
        public void RUD_SNAP_Actor_Group_Types()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
        //        db.SNAP_Actor_Group_Types.InsertOnSubmit(new SNAP_Actor_Group_Type()
        //        {
        //            pkId = 20,
        //            typeName = "TESTTYPE"
        //        });
        //        db.SubmitChanges();

                var test1 = db.SNAP_Actor_Group_Types.Where(t => t.pkId == 1).ToList();
                Assert.IsTrue(test1.Count > 0);

        //        foreach (var type in test1)
        //        {
        //            db.SNAP_Actor_Group_Types.DeleteOnSubmit(type);
        //        }
        //        db.SubmitChanges();

        //        var test2 = db.SNAP_Actor_Group_Types.Where(t => t.pkId == 20).ToList();
        //        Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_SNAP_Workflow()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                db.SNAP_Workflows.InsertOnSubmit(new SNAP_Workflow()
                {
                    actorId = 0,
                    requestId = 0
                });
                db.SubmitChanges();

                var test1 = db.SNAP_Workflows.Where(t => t.requestId == 0 && t.actorId == 0).ToList();
                Assert.IsTrue(test1.Count > 0);

                foreach (var row in test1)
                {
                    db.SNAP_Workflows.DeleteOnSubmit(row);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Workflows.Where(t => t.requestId == 0 && t.actorId == 0).ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void EXEC_usp_search_requests()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                IMultipleResults test = db.usp_search_requests("pxlee");
                Assert.IsTrue(test.GetResult<usp_search_requestsResult>().Count() > 0);
                if (test.GetResult<usp_requestsResult>().Count() > 0)
                {
                    foreach (usp_search_requestsResult result in test.GetResult<usp_search_requestsResult>())
                    {
                        Console.WriteLine(result.userId);
                        Console.WriteLine(result.userDisplayName);
                        Console.WriteLine(result.userTitle);
                        Console.WriteLine(result.submittedBy);
                        Console.WriteLine(result.managerUserId);
                        Console.WriteLine(result.managerDisplayName);
                        Console.WriteLine(result.statusEnum);
                        Console.WriteLine(result.isChanged);
                        Console.WriteLine(result.ticketNumber);
                        Console.WriteLine(result.lastModifiedDate);
                    }
                }
            }
        }

        [Test]
        public void EXEC_usp_requests()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                IMultipleResults test = db.usp_requests("pxlee","my");
                Assert.IsTrue(test.GetResult<usp_requestsResult>().Count() > 0);
                if (test.GetResult<usp_requestsResult>().Count() > 0)
                {
                    foreach (usp_requestsResult result in test.GetResult<usp_requestsResult>())
                    {
                        Console.WriteLine(result.userId);
                        Console.WriteLine(result.userDisplayName);
                        Console.WriteLine(result.userTitle);
                        Console.WriteLine(result.submittedBy);
                        Console.WriteLine(result.managerUserId);
                        Console.WriteLine(result.managerDisplayName);
                        Console.WriteLine(result.statusEnum);
                        Console.WriteLine(result.isChanged);
                        Console.WriteLine(result.ticketNumber);
                        Console.WriteLine(result.lastModifiedDate);
                    }
                }
            }
        }

        [Test]
        public void EXEC_usp_requests_details()
        {
            usp_request_detailsResult t = new usp_request_detailsResult();
            t.requestId = 0;
            t.userId = "test";
            t.userDisplayName = "test";
            t.userTitle = "test";
            t.submittedBy = "test";
            t.managerUserId = "test";
            t.managerDisplayName = "test";
            t.statusEnum = 0;
            t.isChanged = false;
            t.ticketNumber = "test";
            t.fieldId = 0;
            t.fieldLabel = "test";
            t.fieldText = "ttest";
            t.modifiedDate = DateTime.Now;
            t.createdDate = DateTime.Now;

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");

                usp_request_detailsResult test = db.usp_request_details(req.pkId).First();
                Assert.IsTrue(test.requestId > 0);
                Console.WriteLine(test.requestId);
                Console.WriteLine(test.userId);
                Console.WriteLine(test.userDisplayName);
                Console.WriteLine(test.userTitle);
                Console.WriteLine(test.submittedBy);
                Console.WriteLine(test.managerUserId);
                Console.WriteLine(test.managerDisplayName);
                Console.WriteLine(test.statusEnum);
                Console.WriteLine(test.isChanged);
                Console.WriteLine(test.ticketNumber);
                Console.WriteLine(test.fieldId);
                Console.WriteLine(test.fieldLabel);
                Console.WriteLine(test.fieldText);
                Console.WriteLine(test.modifiedDate);
                Console.WriteLine(test.createdDate);
            }
        }

        [Test]
        public void EXEC_usp_open_request_tab()
        {

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var accessReq = new AccessRequest(req.pkId);
                accessReq.Ack();
            }

            using (var db = new SNAPDatabaseDataContext())
            {

                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Pending);
                var accessReq = new AccessRequest(req.pkId);
                accessReq.RequestToChange("TEST");
            }
            usp_open_request_tabResult t = new usp_open_request_tabResult();
            t.requestId = 0;
            t.userId = "test";
            t.userDisplayName = "test";
            t.userTitle = "test";
            t.submittedBy = "test";
            t.managerUserId = "test";
            t.managerDisplayName = "test";
            t.statusEnum = 0;
            t.isChanged = false;
            t.ticketNumber = "test";
            t.fieldId = 0;
            t.fieldLabel = "test";
            t.fieldText = "ttest";
            t.modifiedDate = DateTime.Now;
            t.createdDate = DateTime.Now;
            using (var db = new SNAPDatabaseDataContext())
            {

                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                Assert.IsTrue(req.statusEnum == (byte)RequestState.Change_Requested);
               
                var test = db.usp_open_request_tab(req.userId, req.pkId).First();
                Assert.IsTrue(test.requestId > 0);
                Console.WriteLine(test.requestId);
                Console.WriteLine(test.userId);
                Console.WriteLine(test.userDisplayName);
                Console.WriteLine(test.userTitle);
                Console.WriteLine(test.submittedBy);
                Console.WriteLine(test.managerUserId);
                Console.WriteLine(test.managerDisplayName);
                Console.WriteLine(test.statusEnum);
                Console.WriteLine(test.isChanged);
                Console.WriteLine(test.ticketNumber);
                Console.WriteLine(test.fieldId);
                Console.WriteLine(test.fieldLabel);
                Console.WriteLine(test.fieldText);
                Console.WriteLine(test.modifiedDate);
                Console.WriteLine(test.createdDate);
            }
            usp_open_request_detailsResult t2 = new usp_open_request_detailsResult();
            t2.requestId = 0;
            t2.userId = "test";
            t2.userDisplayName = "test";
            t2.userTitle = "test";
            t2.submittedBy = "test";
            t2.managerUserId = "test";
            t2.managerDisplayName = "test";
            t2.statusEnum = 0;
            t2.isChanged = false;
            t2.ticketNumber = "test";
            t2.fieldId = 0;
            t2.fieldLabel = "test";
            t2.fieldText = "ttest";
            t2.modifiedDate = DateTime.Now;
            t2.createdDate = DateTime.Now;
            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                var test = db.usp_open_request_details(req.userId, req.pkId).First();
                Assert.IsTrue(test.requestId > 0);
                Console.WriteLine(test.requestId);
                Console.WriteLine(test.userId);
                Console.WriteLine(test.userDisplayName);
                Console.WriteLine(test.userTitle);
                Console.WriteLine(test.submittedBy);
                Console.WriteLine(test.managerUserId);
                Console.WriteLine(test.managerDisplayName);
                Console.WriteLine(test.statusEnum);
                Console.WriteLine(test.isChanged);
                Console.WriteLine(test.ticketNumber);
                Console.WriteLine(test.fieldId);
                Console.WriteLine(test.fieldLabel);
                Console.WriteLine(test.fieldText);
                Console.WriteLine(test.modifiedDate);
                Console.WriteLine(test.createdDate);
            }
        }

        [Test]
        public void EXEC_usp_open_my_request_workflow_details()
        {
            usp_open_my_request_workflow_detailsResult t = new usp_open_my_request_workflow_detailsResult();
            t.actor_groupId = 0;
            t.actorGroupType = 0;
            t.actorId = 0;
            t.completedDate = DateTime.Now;
            t.displayName = "UnitTester";
            t.dueDate = DateTime.Now;
            t.emailAddress = "test@test.com";
            t.isActive = false;
            t.isDefault = false;
            t.isGroup = false;
            t.notifyDate = DateTime.Now;
            t.requestId = 0;
            t.userId = "UnitTester";
            t.workflowId = 0;
            t.workflowStateId = 0;
            t.workflowStatusEnum = 0;

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                usp_open_my_request_workflow_detailsResult test = db.usp_open_my_request_workflow_details(req.userId).First();
                Assert.IsTrue(test.displayName != "");
                Console.WriteLine(test.userId);
                Console.WriteLine(test.workflowId);
                Console.WriteLine(test.workflowStateId);
                Console.WriteLine(test.workflowStatusEnum);
                Console.WriteLine(test.requestId);
                Console.WriteLine(test.notifyDate);
                Console.WriteLine(test.isGroup);
                Console.WriteLine(test.isDefault);
                Console.WriteLine(test.isActive);
                Console.WriteLine(test.emailAddress);
                Console.WriteLine(test.dueDate);
                Console.WriteLine(test.displayName);
                Console.WriteLine(test.completedDate);
                Console.WriteLine(test.actorId);
                Console.WriteLine(test.actorGroupType);
                Console.WriteLine(test.actor_groupId);
            }
        }

        [Test]
        public void EXEC_usp_open_my_request_workflow_comments()
        {
            usp_open_my_request_workflow_commentsResult t = new usp_open_my_request_workflow_commentsResult();
            t.pkId = 0;
            t.requestId = 0;
            t.workflowId = 0;
            t.createdDate = DateTime.Now;
            t.commentTypeEnum = 0;
            t.commentText = "Test";

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                usp_open_my_request_workflow_commentsResult test = db.usp_open_my_request_workflow_comments(req.userId).First();
                Assert.IsTrue(test.requestId > 0);
                Console.WriteLine(test.pkId);
                Console.WriteLine(test.requestId);
                Console.WriteLine(test.workflowId);
                Console.WriteLine(test.createdDate);
                Console.WriteLine(test.commentTypeEnum);
                Console.WriteLine(test.commentText);
            }
        }

        [Test]
        public void EXEC_usp_open_my_request_details()
        {
            usp_open_my_request_detailsResult t = new usp_open_my_request_detailsResult();
            t.pkId = 0;
            t.userId = "test";
            t.userDisplayName = "test";
            t.userTitle = "test";
            t.submittedBy = "test";
            t.managerUserId = "test";
            t.managerDisplayName = "test";
            t.statusEnum = 0;
            t.isChanged = false;
            t.ticketNumber = "test";
            t.createdDate = DateTime.Now;

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                usp_open_my_request_detailsResult test = db.usp_open_my_request_details(req.userId).First();
                Assert.IsTrue(test.pkId > 0);
                Console.WriteLine(test.pkId);
                Console.WriteLine(test.userId);
                Console.WriteLine(test.userDisplayName);
                Console.WriteLine(test.userTitle);
                Console.WriteLine(test.submittedBy);
                Console.WriteLine(test.managerUserId);
                Console.WriteLine(test.managerDisplayName);
                Console.WriteLine(test.statusEnum);
                Console.WriteLine(test.isChanged);
                Console.WriteLine(test.ticketNumber);
                Console.WriteLine(test.createdDate);
            }
        }

        [Test]
        public void EXEC_usp_open_my_request_comments()
        {
            usp_open_my_request_commentsResult t = new usp_open_my_request_commentsResult();
            t.pkId = 0;
            t.requestId = 0;
            t.createdDate = DateTime.Now;
            t.commentTypeEnum = 0;
            t.commentText = "Test";

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.submittedBy == "UnitTester");
                usp_open_my_request_commentsResult test = db.usp_open_my_request_comments(req.userId).First();
                Assert.IsTrue(test.requestId > 0);
                Console.WriteLine(test.pkId);
                Console.WriteLine(test.requestId);
                Console.WriteLine(test.createdDate);
                Console.WriteLine(test.commentTypeEnum);
                Console.WriteLine(test.commentText);
            }
        }
    }
}
