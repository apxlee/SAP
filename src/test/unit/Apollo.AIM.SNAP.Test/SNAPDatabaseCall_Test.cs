using System;
using System.Collections.Generic;
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
    }
}
