using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Apollo.AIM.SNAP.CA;
using Apollo.AIM.SNAP.Model;
using Apollo.CA;
using NUnit.Framework;

using Apollo.CA.Logging;
using Apollo.ServiceDesk;
using DirectoryServices=Apollo.CA.DirectoryServices;

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
                Console.WriteLine("key:{0}, Value:{1}", key, list[key]);
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
            changeRequest.AffectedUser.Get("pxlee");

            changeRequest.Attributes["description"] = "Here is the description";
            //changeRequest.Properties.SetPropertyValue("key1", "value");
            //changeRequest.Properties.SetPropertyValue("key2", "value");

            changeRequest.Create();
            Console.WriteLine(changeRequest.Number);
            Assert.IsTrue(changeRequest.Number != string.Empty);


        }


        [Test]
        public void ShouldQueryActiveDirectory()
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
            Emailer.Send("pong.lee@apollogrp.edu", "Pong.Lee@apollogrp.edu", "", "Test", "<h1>Test From Apollo.CA.Emailer</h1>", true);
        }

        [Test]
        public void ShouldSendFormattedEmail()
        {
            // There are the template keys
            // APPROVERNAME
            // SUBJECTFIRSTNAME
            // SUBJECTLASTNAME
            // CAPURL
            // PROCESSNAME
            // INCIDENT

            Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail("pong.lee@apollogrp.edu",
                                                                     "Test Formated Email",
                                                                     @".\newTaskNotification.html",
                                                                     new Hashtable()
                                                                         {
                                                                             {"APPROVERNAME", "approvaer"},
                                                                             {"SUBJECTFIRSTNAME", "firstName"},
                                                                             {"SUBJECTLASTNAME", "lastName"},
                                                                             {"CAPURL", "url"},
                                                                             {"PROCESSNAME", "processName"},
                                                                             {"INCIDENT", "incident"}
                                                                         });
        }

        [Test]
        public void ShouldReturnUserDetailsByLoginName()
        {
            ADUserDetail detail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName("pxlee");
            Assert.IsNotNull(detail);
            CA.DirectoryServices.DisplayDetails(detail);
        }

        [Test]
        public void ShouldReturnUserDetailsByLoginFullName()
        {
            ADUserDetail detail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByFullName("pong lee");
            Assert.IsNotNull(detail);
            CA.DirectoryServices.DisplayDetails(detail);
        }

        [Test]
        public void ShouldReturnUserDetailsByLoginFullName2()
        {
            ADUserDetail detail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByFullName("John Smith, Jr.");
            Assert.IsNotNull(detail);
            CA.DirectoryServices.DisplayDetails(detail);
        }

        [Test]
        public void ShoudReturnMuiltpleUsers()
        {

            List<string> users = CA.DirectoryServices.GetAllUserByFullName("jason");
            //List<string> users = CA.DirectoryServices.GetAllUserByFullName("Guest");
            Assert.IsTrue(users.Count > 1);
            //users.ForEach(s => Console.WriteLine(s));
        }

        [Test]
        public void ShouldReturnUserManagerInfo()
        {
            List<UserManagerInfo> result = CA.DirectoryServices.GetUserManagerInfo("pong lee");
            Assert.IsTrue(result.Count >= 1);
        }

        [Test]
        public void ShoudMatchGetByFullNameAndGetUserManagerInfo()
        {
            List<string> users = CA.DirectoryServices.GetAllUserByFullName("john smith");
            Assert.IsTrue(users.Count > 1);
            List<UserManagerInfo> result = CA.DirectoryServices.GetUserManagerInfo("john smith");
            Assert.IsTrue(result.Count == users.Count);
        }

        [Test]
        [Ignore]
        public void ShoudMatchAllGetByFullNameAndGetUserManagerInfo()
        {
            List<string> users = CA.DirectoryServices.GetAllUserByFullName("");
            Assert.IsTrue(users.Count > 1);
            Console.WriteLine("All users(by full name): " + users.Count);
            List<UserManagerInfo> result = CA.DirectoryServices.GetUserManagerInfo("");
            Console.WriteLine("All users(by user manager info): " + result.Count);
            Assert.IsTrue(result.Count == users.Count);
        }

        [Test]
        public void ShouldGetUserManagerInfo()
        {
            UserManagerInfo u = CA.DirectoryServices.GetUserManagerInfoByFullName("Pong Lee");
            Assert.IsTrue(u.Name != "unknown");
            Assert.IsTrue(u.LoginId != "unknown");
        }

        [Test]
        public void ShouldGetUnknownUserManagerInfo()
        {
            UserManagerInfo u = CA.DirectoryServices.GetUserManagerInfoByFullName("XYZABC");
            Assert.IsTrue(u.Name == "unknown");
        }

        [Test]
        public void ShouldGetAListOfUser()
        {
            UserManagerInfo u = CA.DirectoryServices.GetUserManagerInfoByFullName("XYZABC");
            Assert.IsTrue(u.Name == "unknown");
        }

        [Test]
        public void ShouldGetSimplifiedNameMultipleMatch()
        {
            List<UserManagerInfo> u = CA.DirectoryServices.GetSimplifiedUserManagerInfo("Pong Lee");
            Assert.IsTrue(u.Count > 1);
            Assert.IsTrue(u[0].Name != "unknown");
            Assert.IsTrue(u[0].ManagerName == "unknown");
            Assert.IsTrue(u[0].LoginId == "unknown");
            Assert.IsTrue(u[0].ManagerLoginId == "unknown");
        }

        [Test]
        public void ShouldGetSimplifiedNameDirectMatch()
        {
            List<UserManagerInfo> u = CA.DirectoryServices.GetSimplifiedUserManagerInfo("Pong Lee (Admin)");
            Assert.IsTrue(u.Count == 1);
            Assert.IsTrue(u[0].Name == "Pong Lee (Admin)");
            //Assert.IsTrue(u[0].ManagerName == "unknown");
            Assert.IsTrue(u[0].LoginId != "unknown");
            //Assert.IsTrue(u[0].ManagerLoginId == "unknown");
        }

        [Test]
        public void ShouldGetRole()
        {
            var userid = "jwmccorm"; /* jsmith7 jwmccorm */
            ADUserDetail detail = CA.DirectoryServices.GetUserByLoginName(userid);
            Console.WriteLine(detail.MemberOf);
            using (var db = new SNAPDatabaseDataContext())
            {
                if (detail.MemberOf.Contains("Access"))
                {
                    // 1 - aim team
                    var rolecheck = db.SNAP_Actors.Where(a => a.actor_groupId == 1 && a.userId == userid && a.isActive == true);
                    if (rolecheck.Count() > 0)
                        Console.WriteLine("super user");
                    else
                        Console.WriteLine("access team");
                }
                else
                {
                    var rolecheck = db.SNAP_Actors.Where(a => a.actor_groupId != 1 && a.userId == userid && a.isActive == true);
                    if (rolecheck.Count() > 0)
                        Console.WriteLine("approval manager");
                    else
                        Console.WriteLine("requestor");

                }
            }   
        }   
    }
}
