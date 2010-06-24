﻿using System;
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
        [SetUp]
        public void SetUp()
        {
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
        public void RUD_SNAP_Workflow_State_Types()
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

                foreach (var type in test1)
                {
                    db.SNAP_Workflow_State_Types.DeleteOnSubmit(type);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Workflow_State_Types.Where(t => t.pkId == 20).ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_SNAP_Request_State_Types()
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

                foreach (var type in test1)
                {
                    db.SNAP_Request_State_Types.DeleteOnSubmit(type);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Request_State_Types.Where(t => t.pkId == 20).ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_SNAP_Comments_Types()
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

                foreach (var type in test1)
                {
                    db.SNAP_Comments_Types.DeleteOnSubmit(type);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Comments_Types.Where(t => t.pkId == 20).ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

        [Test]
        public void RUD_SNAP_Actor_Group_Types()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                db.SNAP_Actor_Group_Types.InsertOnSubmit(new SNAP_Actor_Group_Type()
                {
                    pkId = 20,
                    typeName = "TESTTYPE"
                });
                db.SubmitChanges();

                var test1 = db.SNAP_Actor_Group_Types.Where(t => t.pkId == 20).ToList();
                Assert.IsTrue(test1.Count > 0);

                foreach (var type in test1)
                {
                    db.SNAP_Actor_Group_Types.DeleteOnSubmit(type);
                }
                db.SubmitChanges();

                var test2 = db.SNAP_Actor_Group_Types.Where(t => t.pkId == 20).ToList();
                Assert.IsTrue(test2.Count == 0);
            }
        }

    }
}
