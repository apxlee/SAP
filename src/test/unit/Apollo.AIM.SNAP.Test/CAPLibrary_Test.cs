using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Apollo.CA;
using NUnit.Framework;

using Apollo.CA.Logging;
using Apollo.ServiceDesk;

namespace Apollo.AIM.SNAP.Test
{
    [TestFixture]
    public class CapLibraryTest
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void ShouldReadConfigFile()
        {
            Assert.IsTrue(ConfigurationManager.AppSettings["ServiceDesk.ServiceDeskWebService.USD_WebService"] != string.Empty, "Should read from machine.config");
        }

        [Test]
        public void ShouldHaveLogger()
        {
            // make sure log4net is configured
            // c:/CapLogs with proper config files are there
            Logger.Debug("This is logger test");
        }

        [Test]
        public void ShouldBeReadyForServiceDesck()
        {
            //var sdService = new Service();
            /*
              == Turn out in the Service ctor, it tries to login to the Service Desk
             * 
            string login = ConfigurationManager.AppSettings["ServiceDeskLogin"];
            string password = ConfigurationManager.AppSettings["ServiceDeskPassword"];
            sdService.Login(login, password);
             */
            //sdService.Logout();

            var sdConfigData = string.Format("Login:{0},Password:{1},Domain:{2}",
                                                SDConfig.Instance.Login, 
                                                SDConfig.Instance.Password,
                                                SDConfig.Instance.Domain);
            AttributeList list = SDConfig.Instance.DefaultServiceDeskAttributes;
            foreach (var key in list.Keys)
            {
                Console.WriteLine("key:{0}, Value:{1}",key,list[key]);
            }
            Console.WriteLine(sdConfigData);
        }

        [Test]
        [Ignore]
        public void ShouldCreateSDTicket()
        {
            var changeRequest = new ServiceDesk.ChangeRequest(SDConfig.Instance.Login, SDConfig.Instance.Password);

            changeRequest.CategoryName = "Server.Systems.Privileged Access";
            
            changeRequest.Submitter.Get("svc_Cap");
            changeRequest.AffectedUser.Get("svc_Cap");
            
            changeRequest.Attributes["description"] = "Here is the description";
            //changeRequest.Properties.SetPropertyValue("key1", "value");
            //changeRequest.Properties.SetPropertyValue("key2", "value");

            changeRequest.Create();
            Console.WriteLine(changeRequest.Number);
            Assert.IsTrue(changeRequest.Number != string.Empty);
        }

        [Test] public void ShouldQueryActiveDirectory()
        {
            var ldap = new LDAPConnection();
            Console.WriteLine("host: " + ldap.Host);
            Console.WriteLine("base: " + ldap.LDAPbase);
            Console.WriteLine("connection: " + ldap.LdapConnectionString);
            Console.WriteLine("ldap host: " + ldap.LDAPHost);
            Console.WriteLine("password" + ldap.Password);
            Console.WriteLine("port: " + ldap.Port);
            Console.WriteLine("username: " + ldap.Username);
            Console.WriteLine("SearchBase: " + ldap.UserSearchBase);

            Console.WriteLine("a.pxlee DN: " + Apollo.CA.DirectoryServices.GetUserDNByID("a.pxlee"));
            //Console.WriteLine("auth: " + DirectoryServices.IsAuthenticated("a.pxlee", "abc"));
            //Console.WriteLine("ADAM Group count: " + (DirectoryServices.getADAMGroups("a.pxlee")).Count);
        }

        [Test]
        public void ShouldSendEmail()
        {
            Emailer.Send("pong.lee@apollogrp.edu", "Pong.Lee@apollogrp.edu", "", "Test", "<h1>Test From Apollo.CA.Emailer</h1>",true);
        }
       
    }
}
