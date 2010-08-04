using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.AIM.SNAP.Model;
using NUnit.Framework;


namespace Apollo.AIM.SNAP.Test
{
    [TestFixture]
    public class Utilities_Test
    {
        [Test]
        public void ShouldGetAbsPath()
        {
            Assert.IsTrue(Utilities.AbsolutePath == string.Empty);
        }

    }
}
