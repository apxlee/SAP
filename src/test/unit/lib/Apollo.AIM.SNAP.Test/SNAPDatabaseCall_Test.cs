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
        public void ShouldReturnMyOpenRequest()
        {
            using (var db = new SNAPDatabaseDataContext())
            {

                var requests = db.MyOpenRequests("pxlee");
                var reqDetails = (List<usp_open_my_request_detailsResult>)requests["reqDetails"];
                foreach (var d in reqDetails)
                {
                    Console.WriteLine(d.pkId + "," + d.statusEnum);
                }
                var wfComments = (List<usp_open_my_request_workflow_commentsResult>)requests["wfComments"];
                foreach (var c in wfComments)
                {
                    Console.WriteLine(c.requestId + "," + c.commentText);
                }
                var texts = (List<SNAP_Access_User_Text>) requests["reqText"];
                foreach (SNAP_Access_User_Text list in texts)
                {
                    Console.WriteLine(list.access_details_formId + "," + list.userText);
                }
            }    
        }

        [Test]
        public void ShouldReturnMyApprovalRequest()
        {
            using (var db = new SNAPDatabaseDataContext())
            {

                var requests = db.MyOpenApprovalRequests("clschwim");
                var reqDetails = (List<usp_open_my_request_detailsResult>)requests["reqDetails"];
                foreach (var d in reqDetails)
                {
                    Console.WriteLine(d.pkId + "," + d.statusEnum);
                }
                var wfComments = (List<usp_open_my_request_workflow_commentsResult>)requests["wfComments"];
                foreach (var c in wfComments)
                {
                    Console.WriteLine(c.requestId + "," + c.commentText);
                }
                var texts = (List<SNAP_Access_User_Text>)requests["reqText"];
                foreach (SNAP_Access_User_Text list in texts)
                {
                    Console.WriteLine(list.access_details_formId + "," + list.userText);
                }
            }
        }

        
        //[Test]
        //public void ShouldReturnNoRequestData()
        //{
        //    using (var db = new SNAPDatabaseDataContext())
        //    {
        //        var maxRequestId = db.SNAP_Requests.Max(x => x.pkId);
        //        var data = db.RetrieveRequest(++maxRequestId);
        //        Assert.IsTrue(data.Count() == 0);              
        //    }

        //}

        //[Test]
        //public void ShouldReturnRequestData()
        //{
        //    using (var db = new SNAPDatabaseDataContext())
        //    {
        //        var maxRequestId = db.SNAP_Requests.Max(x => x.pkId);
        //        var requestTexts = db.RetrieveRequest(maxRequestId);

        //        foreach (var text in requestTexts)
        //        {
        //            Console.WriteLine(text.userText);
        //        }
        //        Assert.IsTrue(requestTexts.Count() > 0);

        //    }
        //}

        //[Test] public void SouldReturnUserViewTab()
        //{
        //    var userViewTab = new Dictionary<string, object>();
        //    Console.WriteLine("org: " + userViewTab.Count);
        //    using (var db = new SNAPDatabaseDataContext())
        //    {
        //        IMultipleResults result = db.usp_open_user_view_tab("clschwim");
        //        if (result.ReturnValue.ToString() == "0")
        //        {
        //            userViewTab.Add("details", result.GetResult<usp_open_user_view_detailsResult>().ToList());
        //            userViewTab.Add("status", result.GetResult<usp_open_user_view_statusResult>().ToList());
        //        }
        //    }
        //    Console.WriteLine("final: " + userViewTab.Count);
        //    foreach (var x in (IEnumerable<usp_open_user_view_detailsResult>)userViewTab["details"])
        //    {
        //        Console.WriteLine(x.requestId);
        //    }

        //    foreach (var x in (IEnumerable<usp_open_user_view_statusResult>)userViewTab["status"])
        //    {
        //        Console.WriteLine(x.requestId);
        //    }

        //}
    }
}
