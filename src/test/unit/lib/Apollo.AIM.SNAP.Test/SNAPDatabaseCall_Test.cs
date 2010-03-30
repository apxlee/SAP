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
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void ShouldReturnAllRequests()
        {
            using (var db = new SNAPDatabaseDataContext())
            {

                Console.WriteLine("Access Team Requests:");
                db.GetAllRequests("clschwim", "accessteam");

                Console.WriteLine("\tOpen Requests:");
                output(db.OpenRquests);
                Console.WriteLine("\tClose Requests:");
                output(db.CloseRquests);

                Console.WriteLine("\n======");
                Console.WriteLine("My Requests:");
                db.GetAllRequests("clschwim", "my");
                Console.WriteLine("\tOpen Requests:");
                output(db.OpenRquests);
                Console.WriteLine("\tClose Requests:");
                output(db.CloseRquests);

                Console.WriteLine("\n======");
                Console.WriteLine("Approval Requests:");
                db.GetAllRequests("clschwim", "approval");
                Console.WriteLine("\tOpen Requests:");
                output(db.OpenRquests);
                Console.WriteLine("\tClose Requests:");
                output(db.CloseRquests);

            }    

        }
        
        private void output(Dictionary<string, object> data)
        {
            var reqDetails = (List<usp_open_my_request_detailsResult>)data["reqDetails"];
            Console.WriteLine("\t\tReq details cnt: " + reqDetails.Count);

            foreach (var d in reqDetails)
            {
                //Console.WriteLine(d.pkId + "," + d.statusEnum);
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
                var data = db.RetrieveRequest(++maxRequestId);
                Assert.IsTrue(data.Count() == 0);
            }

        }

        [Test]
        public void ShouldReturnRequestData()
        {
            using (var db = new SNAPDatabaseDataContext())
            {
                var maxRequestId = db.SNAP_Requests.Max(x => x.pkId);
                var requestTexts = db.RetrieveRequest(maxRequestId);

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

    }
}
