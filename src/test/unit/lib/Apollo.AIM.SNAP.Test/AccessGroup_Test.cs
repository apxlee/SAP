using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.AIM.SNAP.Model;
using NUnit.Framework;

namespace Apollo.AIM.SNAP.Test
{
    [TestFixture]
    public class AccessGroup_Test
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void TestDeepCopy()
        {
            List<AccessApprover> testApproverList = new List<AccessApprover>();
            AccessApprover testApprover = new AccessApprover();
            testApprover.ActorGroupType = ActorGroupType.Technical_Approver;
            testApprover.ActorId = 1;
            testApprover.DisplayName = "Unit Test Approver 1";
            testApprover.IsDefault = false;
            testApprover.IsSelected = true;
            testApprover.UserId = "testuser1";
            testApprover.WorkflowState = WorkflowState.Not_Active;
            testApproverList.Add(testApprover);

            AccessGroup testGroup1 = new AccessGroup();
            testGroup1.GroupId = 1;
            testGroup1.GroupName = "TestGroup";
            testGroup1.Description = "Test group for unit testing.";
            testGroup1.IsDisabled = false;
            testGroup1.IsLargeGroup = false;
            testGroup1.IsSelected = false;
            testGroup1.ActorGroupType = ActorGroupType.Technical_Approver;
            testGroup1.AvailableApprovers = testApproverList;

            AccessGroup testGroup2 = testGroup1.CreateDeepCopy(testGroup1);

            testGroup1.GroupId = 0;
            testGroup1.GroupName = "";
            testGroup1.Description = "";
            testGroup1.IsDisabled = true;
            testGroup1.IsLargeGroup = true;
            testGroup1.IsSelected = true;
            testGroup1.ActorGroupType = ActorGroupType.Team_Approver;
            testGroup1.AvailableApprovers.Clear();

            Assert.IsTrue(testGroup2.GroupId == 1);
            Assert.IsTrue(testGroup2.GroupName == "TestGroup");
            Assert.IsTrue(testGroup2.Description == "Test group for unit testing.");
            Assert.IsTrue(!testGroup2.IsDisabled);
            Assert.IsTrue(!testGroup2.IsLargeGroup);
            Assert.IsTrue(!testGroup2.IsSelected);
            Assert.IsTrue(testGroup2.ActorGroupType == ActorGroupType.Technical_Approver);
            Assert.IsTrue(testGroup2.AvailableApprovers.Count() > 0);
        }
    }
}
