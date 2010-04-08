﻿using System;
using System.Collections;

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Apollo.AIM.SNAP.CA;

namespace Apollo.AIM.SNAP.Model
{
    public class Email
    {
        /*
         *       <add key="Approval" value="D:\gitrepo\snap\trunk\src\lib\Apollo.AIM.SNAP\EmailTemplate\approval.html" />
      <add key="NagApproval" value="D:\gitrepo\snap\trunk\src\lib\Apollo.AIM.SNAP\EmailTemplate\nagApproval.html" />
      <add key="CompleteToSubmitter" value="D:\gitrepo\snap\trunk\src\lib\Apollo.AIM.SNAP\EmailTemplate\completeToSubmitter.html" /> 
      <add key="ConfrimToSubmitter" value="D:\gitrepo\snap\trunk\src\lib\Apollo.AIM.SNAP\EmailTemplate\confirmSubmitToSubmitter.html" />
      <add key="DenyToSubmitter" value="D:\gitrepo\snap\trunk\src\lib\Apollo.AIM.SNAP\EmailTemplate\denyToSubmitter.html" />
      <add key="RequestChangeToSubmitter" value="D:\gitrepo\snap\trunk\src\lib\Apollo.AIM.SNAP\EmailTemplate\RequestChangeToSubmitter.html" />

         */
        //private static string link = @"http://access/snap/index.aspx?Requestid=";
        private static string prefix = @"http://";
        private static string url = @"http://";
        private static string aimDG = "pong.lee@apollogrp.edu";

        public static void OverdueTask(string to, long id, string userName)
        {
            configPerEnvironment(id);

            Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(aimDG,
                                                                     "Supplemental Network Access Process-Overdue Alert",
                                                                     ConfigurationManager.AppSettings["NagApproval"], // newTaskNotification.html",
                                                                     new Hashtable()
                                                                         {
                                                                             {"APPROVERNAME", to},
                                                                             {"NAME", userName},
                                                                             {"URL", url},
                                                                             {"PREFIX", prefix}
                                                                         });

             
        }

        public static void UpdateRequesterStatus(string submitterUserId, string name, long id, WorkflowState status, string reason)
        {
            string subject = "";
            string emailTemplatePath = "";
            switch (status)
            {
                    /*
                case WorkflowState.Pending_Acknowlegement:
                    subject = "Supplemental Network Access Process-Submitted";
                    emailTemplatePath = ConfigurationManager.AppSettings["Approval"];
                    break;
                     */
                case WorkflowState.Closed_Completed:
                    subject = "Supplemental Network Access Process-Complete";
                    emailTemplatePath = ConfigurationManager.AppSettings["CompleteToSubmitter"];
                    break;
                case WorkflowState.Change_Requested:
                    subject = "Supplemental Network Access Process-Request Change";
                    emailTemplatePath = ConfigurationManager.AppSettings["RequestChangeToSubmitter"];
                    break;
                case WorkflowState.Closed_Denied:
                    subject = "Supplemental Network Access Process-Denied";
                    emailTemplatePath = ConfigurationManager.AppSettings["DenyToSubmitter"];
                    break;
            }

            configPerEnvironment(id);

            Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(emailAddress(submitterUserId),
                                                                     subject,
                                                                     emailTemplatePath, // newTaskNotification.html",
                                                                     new Hashtable()
                                                                         {
                                                                             //{"APPROVERNAME", name},
                                                                             {"NAME", name},
                                                                             {"URL", url},
                                                                             {"REASON", reason},
                                                                             {"PREFIX", prefix}
                                                                         });

        }

        public static void TaskAssignToApprover(string toEmailAddress, string to, long id, string name)
        {
            var subject = "Supplemental Network Access Process-Approval Needed";
            var body = "A task is waiting for your approval. For detail, <a href='" + url + id + "'>visit our site</a>";
            configPerEnvironment(id);

            Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(toEmailAddress,
                                                                     "Supplemental Network Access Process-Submit",
                                                                     ConfigurationManager.AppSettings["Approval"], // newTaskNotification.html",
                                                                     new Hashtable()
                                                                         {
                                                                             {"APPROVERNAME", to},
                                                                             {"NAME", name},
                                                                             {"URL", url},
                                                                             {"PREFIX", prefix}
                                                                         });

        }

        public static void RequestAsssignToAccessTeam(long id, string firstName, string lastName)
        {
            configPerEnvironment(id);

            Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(aimDG,
                                                                     "Test Formated Email",
                                                                     @".\approval.html", // newTaskNotification.html",
                                                                     new Hashtable()
                                                                         {
                                                                             {"APPROVERNAME", "Access Team"},
                                                                             {"SUBJECTFIRSTNAME", firstName},
                                                                             {"SUBJECTLASTNAME", lastName},
                                                                             {"SNAPURL", url},
                                                                             {"PREFIX", prefix},
                                                                             {"PROCESSNAME", "processName"},
                                                                             {"INCIDENT", "incident"}
                                                                         });

        }

        private static string emailAddress(string usrId)
        {
            ADUserDetail detail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(usrId);
            return detail.EmailAddress;
        }

        private static void configPerEnvironment(long id)
        {
            if (Environment.UserDomainName.ToUpper().Contains("DEVAPOLLO"))
            {
                prefix += "localhost/snap/images";
                url += ("localhost/snap/index.aspx?RequestId=" + id);
            }
            else if (Environment.UserDomainName.ToUpper().Contains("QAAPOLLO"))
            {
                prefix += "access.qaapollogrp.edu/snap/images";
                url += ("access.qaapollogrp.edu/snap/index.aspx?RequestId=" + id);
            }
            else
            {
                prefix += "access.apollogrp.edu/snap/images";
                url += ("access.apollogrp.edu/snap/index.aspx?RequestId=" + id);
            }
        }
    }
}