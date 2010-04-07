using System;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.AIM.SNAP.CA;

namespace Apollo.AIM.SNAP.Model
{
    public class Email
    {
        //private static string link = @"http://access/snap/index.aspx?Requestid=";
        private static string prefix = @"http://";
        private static string url = @"http://";
        private static string fromEmail = "aim@apollogrp.edu";
        private static string aimDG = "pong.lee@apollogrp.edu";

        public static string OverdueTask(string to, long id)
        {

            var subject = "Supplemental Network Access Process-Overdue Alert";
            var body = "A task is waiting for your approval. For detail, <a href='" + url + id + "'>visit our site</a>";
             
            return body;
        }

        public static void UpdateRequesterStatus(string submitterUserId, string name, long id, WorkflowState status, string reason)
        {
            string subject;
            string emailTemplateKey;
            switch (status)
            {
                case WorkflowState.Pending_Acknowlegement:
                    subject = "Supplemental Network Access Process-Submitted";
                    emailTemplateKey = "confirmSubmitToSubmitter";
                    break;
                case WorkflowState.Closed_Completed:
                    subject = "Supplemental Network Access Process-Complete";
                    emailTemplateKey = "confirmSubmitToSubmitter";
                    break;
                case WorkflowState.Change_Requested:
                    subject = "Supplemental Network Access Process-Request Change";
                    emailTemplateKey = "confirmSubmitToSubmitter";
                    break;
                case WorkflowState.Closed_Denied:
                    subject = "Supplemental Network Access Process-Denied";
                    emailTemplateKey = "denyToSubmitter";
                    break;
            }
        }

        public static string TaskAssignToApprover(string toEmailAddress, long id)
        {
            var subject = "Supplemental Network Access Process-Approval Needed";
            var body = "A task is waiting for your approval. For detail, <a href='" + url + id + "'>visit our site</a>";
            return body;
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
