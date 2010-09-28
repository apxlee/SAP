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

            Assert.IsTrue(RequestData.UpdatedRequestDataList(newData, orgData).Count == 0);
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
            var result = RequestData.UpdatedRequestDataList(newData, orgData);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].FormId == "2");
            Assert.IsTrue(result[0].UserText == "Test 2");

        }

        [Test]
        public void ShouldFindNoDifferentWithMultipleModDate()
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
                                          fieldText = "Test 1",
                                          modifiedDate = DateTime.Now
                                      },
                                  new usp_open_request_tabResult()
                                     {
                                          fieldId = 2,
                                          fieldText = "Test 221",
                                          modifiedDate = DateTime.Now.AddHours(-2)
                                      },
                                   new usp_open_request_tabResult()
                                     {
                                          fieldId = 2,
                                          fieldText = "Test 222",
                                          modifiedDate = DateTime.Now.AddHours(-1)
                                      },
                                    new usp_open_request_tabResult()
                                     {
                                          fieldId = 2,
                                          fieldText = "Test 2",
                                          modifiedDate = DateTime.Now
                                      }
                              };
            var result = RequestData.UpdatedRequestDataList(newData, orgData);
            Assert.IsTrue(result.Count == 0);
        }

        [Test]
        public void ShouldFindDifferentWithMultipleModDate()
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
                                          fieldText = "Test 1",
                                          modifiedDate = DateTime.Now
                                      },
                                  new usp_open_request_tabResult()
                                     {
                                          fieldId = 2,
                                          fieldText = "Test 221",
                                          modifiedDate = DateTime.Now.AddHours(-2)
                                      },
                                   new usp_open_request_tabResult()
                                     {
                                          fieldId = 2,
                                          fieldText = "Test 2",
                                          modifiedDate = DateTime.Now.AddHours(-1)
                                      },
                                    new usp_open_request_tabResult()
                                     {
                                          fieldId = 2,
                                          fieldText = "Test 223",
                                          modifiedDate = DateTime.Now
                                      }
                              };
            var result = RequestData.UpdatedRequestDataList(newData, orgData);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].FormId == "2");
            Assert.IsTrue(result[0].UserText == "Test 2");

        }

    }
}
