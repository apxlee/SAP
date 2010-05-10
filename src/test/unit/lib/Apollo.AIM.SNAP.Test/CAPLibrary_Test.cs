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
            //Email.RequestAsssignToAccessTeam(1, "submiter_first_name", "submitter last_name");
            //Email.OverdueTask("mplee168@hotmail.com", "Pong Lee", 1, "Requester Name" );
			//Email.UpdateRequesterStatus("pxlee", "Your Name", 1, WorkflowState.Closed_Completed, "The reason");
            //Email.UpdateRequesterStatus("pxlee", "Your Name", 1, WorkflowState.Closed_Denied, "The reason deny");
            //Email.UpdateRequesterStatus("pxlee", "Your Name", 1, WorkflowState.Change_Requested, "The reason change");
            //Email.TaskAssignToApprover("pong.lee@apollogrp.edu", "Pong Lee", 1, "RequestName Here");
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

        [Test] public void ShouldSendEmbededEmail()
        {
            
        }

        /*

        static void EmbedImages()
        {
            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress("me@mycompany.com");
            mail.To.Add("you@yourcompany.com");

            //set the content
            mail.Subject = "This is an email";

            //first we create the Plain Text part
            AlternateView plainView = AlternateView.CreateAlternateViewFromString("This is my plain text content, viewable by those clients that don't support html", null, "text/plain");

            //then we create the Html part
            //to embed images, we need to use the prefix 'cid' in the img src value
            //the cid value will map to the Content-Id of a Linked resource.
            //thus <img src='cid:companylogo'> will map to a LinkedResource with a ContentId of 'companylogo'
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString("Here is an embedded image.<img src=cid:companylogo>", null, "text/html");

            //create the LinkedResource (embedded image)
            LinkedResource logo = new LinkedResource("c:\\temp\\logo.gif");
            logo.ContentId = "companylogo";
            //add the LinkedResource to the appropriate view
            htmlView.LinkedResources.Add(logo);

            //add the views
            mail.AlternateViews.Add(plainView);
            mail.AlternateViews.Add(htmlView);


            //send the message
            SmtpClient smtp = new SmtpClient("127.0.0.1"); //specify the mail server address
            smtp.Send(mail);
        }

          protected void yourButton_Click(object sender, EventArgs e)
    {
            
            string strMailContent = "Welcome new user";
            string fromAddress = "yourname@yoursite.com";
            string toAddress = "newuser@hisdomain.com";
            string contentId  = "image1";
            string path = Server.MapPath(@"images/Logo.jpg"); // my logo is placed in images folder
            MailMessage mailMessage = new MailMessage( fromAddress, toAddress );
            mailMessage.Bcc.Add("inkrajesh@hotmail.com"); // put your id here
            mailMessage.Subject = "Welcome new User";
          

            LinkedResource logo = new LinkedResource(path);
            logo.ContentId = "companylogo";
     // done HTML formatting in the next line to display my logo
            AlternateView av1 = AlternateView.CreateAlternateViewFromString("<html><body><img src=cid:companylogo/><br></body></html>" + strMailContent, null, MediaTypeNames.Text.Html);
            av1.LinkedResources.Add(logo);


            mailMessage.AlternateViews.Add(av1);
            mailMessage.IsBodyHtml = true;
            SmtpClient mailSender = new SmtpClient("localhost"); //use this if you are in the development server
                        mailSender.Send(mailMessage);
           
        }    
         * 
         * 
         * 
         */
    }
}
