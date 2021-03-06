﻿using System;
using System.Text.RegularExpressions;
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

        [Test] public void ShouldSendOverdueApproval()
        {
            Email.SendTaskEmail(EmailTaskType.OverdueApproval, "hello@apollogrp.edu", "Hello", 123, "hello");
        }

        
        [Test]
        public void ShouldSendOverdueChangeRequested()
        {
            Email.SendTaskEmail(EmailTaskType.OverdueChangeRequested, "hello@apollogrp.edu", "Hello", 123, "hello");
        }

        [Test]
        public void ShouldSendProxyForAEU()
        {
            Email.SendTaskEmail(EmailTaskType.ProxyForAffectedEndUser, "hello@apollogrp.edu", "Hello", 123, "hello");
        }

        [Test]
        public void RegExp()
        {
                                                     //"^a\\."
            Console.WriteLine(Regex.Replace("a.pxlee", @"^a\.", ""));
            Console.WriteLine(Regex.Replace("pa.xlee", @"^a\.", ""));
            Console.WriteLine(Regex.Replace("a.agnair", @"^a\.", ""));
            Console.WriteLine(Regex.Replace("agnair", @"^a\.", ""));
        }
    }
}
