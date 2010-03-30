using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using Apollo.AIM.SNAP.Model;
using Apollo.CA;
using NUnit.Framework;

namespace Apollo.AIM.SNAP.Test
{
    [TestFixture]
    public class SLAConfiguration_Test
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void ShouldHaveSLAConfigurationInfo()
        {
            var sla = new SLAConfiguration((NameValueCollection)ConfigurationManager.GetSection(
                "Apollo.AIM.SNAP/Workflow.SLA"));

            Console.WriteLine(sla.AccessTeamAckInMinute);
            Console.WriteLine(sla.AccessTeamCreateWorkflowInDays);
            Console.WriteLine(sla.ManagerApprovalInDays);
            Console.WriteLine(sla.TeamApprovalInDays);
            Console.WriteLine(sla.TechnicalApprovalInDays);

        }

    }
}
