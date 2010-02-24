using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.AIM.SNAP.Model;
using NUnit.Framework;

namespace Apollo.AIM.SNAP.Test
{
    [TestFixture]
    public class RequestData_Test
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]public void ShoudMax()
        {
            var orgData = new List<usp_open_request_tabResult>()
                              {
                                  new usp_open_request_tabResult()
                                      {
                                          fieldId = 1,
                                          fieldText = "Test 1",
                                          createdDate = DateTime.Now.AddHours(-1),
                                      },
                                  new usp_open_request_tabResult()
                                      {
                                          fieldId = 2,
                                          fieldText = "Test 2"
                                      },
                                  new usp_open_request_tabResult()
                                     {
                                          fieldId = 1,
                                          fieldText = "Test 111",
                                          createdDate = DateTime.Now,
                                      }

                              };
            /*
            var d = orgData.Single(x => x.fieldId == 1);
            Console.WriteLine(d.fieldText);
            */

            var data = from x in orgData
                       where x.fieldId == 1
                       select x;
            Console.WriteLine(data.Count());

            DateTime t = data.Max(x => x.createdDate);

            var finalData = data.Single(x => x.createdDate == t);
            Console.WriteLine(finalData.fieldText);
            


            
        }
        [Test]
        public void ShouldFindNoDifferent()
        {
            var newData = new List<RequestData>()
                              {
                                    new RequestData() {FormId = "1", UserText = "Test 1"},
                                    new RequestData() {FormId = "2", UserText = "Test 2"},
                              };

            var orgData = new List<usp_open_request_tabResult>()
                              {
                                  new usp_open_request_tabResult()
                                      {
                                          fieldId = 1,
                                          fieldText = "Test 1"
                                      },
                                  new usp_open_request_tabResult()
                                      {
                                          fieldId = 2,
                                          fieldText = "Test 2"
                                      }
                              };

            Assert.IsTrue(RequestData.UpdateRequestList(newData, orgData).Count == 0);
        }

        [Test]
        public void ShouldFindDifferent()
        {
            var newData = new List<RequestData>()
                              {
                                    new RequestData() {FormId = "1", UserText = "Test 1"},
                                    new RequestData() {FormId = "2", UserText = "Test 2"},
                              };

            var orgData = new List<usp_open_request_tabResult>()
                              {
                                  new usp_open_request_tabResult()
                                      {
                                          fieldId = 1,
                                          fieldText = "Test 1"
                                      },
                                  new usp_open_request_tabResult()
                                     {
                                          fieldId = 2,
                                          fieldText = "Test 222"
                                      }
                              };
            var result = RequestData.UpdateRequestList(newData, orgData);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].FormId == "2");
            Assert.IsTrue(result[0].UserText == "Test 222");

        }

    }
}
