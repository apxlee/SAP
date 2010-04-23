using System;
using System.Collections;

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Apollo.AIM.SNAP.CA;

using System.Web;

namespace Apollo.AIM.SNAP.Model
{
    public class Email
    {
        //private static string link = @"http://access/snap/index.aspx?Requestid=";
        private static string prefix = @"http://";
        private static string url = @"http://";

        public static void OverdueTask(string toEmail, string toName, long id, string userName)
        {
            configPerEnvironment(id);

            Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(toEmail,
                                                                     "Supplemental Network Access Process-Overdue Alert",
                                                                     absPath + ConfigurationManager.AppSettings["NagApproval"], // newTaskNotification.html",
                                                                     new Hashtable()
                                                                         {
                                                                             {"APPROVERNAME", toName},
                                                                             {"NAME", userName},
                                                                             {"URL", url},
                                                                             {"PREFIX", prefix}
                                                                         });

             
        }

        public static void UpdateRequesterStatus(string submitterUserId, string name, long id, WorkflowState status, string reason)
        {

            string subject = "";
            string emailTemplatePath = absPath;
            switch (status)
            {
                case WorkflowState.Pending_Acknowlegement:
                    subject = "Supplemental Network Access Process-Submitted";
                    emailTemplatePath +=  ConfigurationManager.AppSettings["ConfirmSubmitToSubmitter"];
                    break;
                case WorkflowState.Closed_Completed:
                    subject = "Supplemental Network Access Process-Complete";
                    emailTemplatePath += ConfigurationManager.AppSettings["CompleteToSubmitter"];
                    break;
                case WorkflowState.Change_Requested:
                    subject = "Supplemental Network Access Process-Request Change";
                    emailTemplatePath += ConfigurationManager.AppSettings["RequestChangeToSubmitter"];
                    break;
                case WorkflowState.Closed_Denied:
                    subject = "Supplemental Network Access Process-Denied";
                    emailTemplatePath += ConfigurationManager.AppSettings["DenyToSubmitter"];
                    break;
                case WorkflowState.Closed_Cancelled:
                    subject = "Supplemental Network Access Process-Cancelled";
                    emailTemplatePath += ConfigurationManager.AppSettings["DenyToSubmitter"];
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
            configPerEnvironment(id);
            Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(toEmailAddress,
                                                                     "Supplemental Network Access Process-Approval Needed",
                                                                     absPath + ConfigurationManager.AppSettings["Approval"], // newTaskNotification.html",
                                                                     new Hashtable()
                                                                         {
                                                                             {"APPROVERNAME", to},
                                                                             {"NAME", name},
                                                                             {"URL", url},
                                                                         });

             
        }


        private static string emailAddress(string usrId)
        {
            // for unit test only
#if DEBUG
            if (usrId == "UnitTester")
                return "pong.lee@apollogrp.edu";

#endif
            ADUserDetail detail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(usrId);
            return detail.EmailAddress;
        }

        private static string absPath
        {
            get
            {
                string x = string.Empty;
                try
                {
                   x = HttpRuntime.AppDomainAppPath;
                }
                catch (Exception ex)
                {
                }

                return x;

            }
        }
        private static void configPerEnvironment(long id)
        {
            prefix = @"http://";
            url = @"http://";

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
