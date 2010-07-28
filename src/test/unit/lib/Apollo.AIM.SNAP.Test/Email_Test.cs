using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.AIM.SNAP.Model;
using NUnit.Framework;

namespace Apollo.AIM.SNAP.Test
{
    [TestFixture]
    public class Email_Test
    {
        [Test]
        public void ShouldGetEmailSubject()
        {
            Assert.IsTrue(Email.GetSubjectAction != String.Empty);
        }
    }
}
