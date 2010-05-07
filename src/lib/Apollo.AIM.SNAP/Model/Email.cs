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
        private static string _imageUrl = @"http://";
        private static string _followLinkUrl = @"http://";

        public static void OverdueTask(string toEmail, string toName, long requestId, string userName)
        {
			ConfigPerEnvironment(requestId, PageNames.APPROVING_MANAGER);
            
            Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(toEmail,
                                                                     "Supplemental Access Process - Overdue Alert",
                                                                     AbsolutePath + ConfigurationManager.AppSettings["NagApproval"], // newTaskNotification.html",
                                                                     new Hashtable()
                                                                         {
                                                                             {"APPROVERNAME", toName},
                                                                             {"NAME", userName},
                                                                             {"URL", _followLinkUrl},
                                                                             {"PREFIX", _imageUrl}
                                                                         });

             
        }

        public static void UpdateRequesterStatus(string submitterUserId, string name, long requestId, WorkflowState workflowState, string reason)
        {

            string subject = "";
            string emailTemplatePath = AbsolutePath;
			
			switch (workflowState)
            {
                case WorkflowState.Pending_Acknowledgement:
                    subject = "Supplemental Access Process - Submitted";
                    emailTemplatePath +=  ConfigurationManager.AppSettings["ConfirmSubmitToSubmitter"];
                    break;
                case WorkflowState.Closed_Completed:
                    subject = "Supplemental Access Process - Completed";
                    emailTemplatePath += ConfigurationManager.AppSettings["CompleteToSubmitter"];
                    break;
                case WorkflowState.Change_Requested:
                    subject = "Supplemental Access Process - Request Change";
                    emailTemplatePath += ConfigurationManager.AppSettings["RequestChangeToSubmitter"];
                    break;
                case WorkflowState.Closed_Denied:
                    subject = "Supplemental Access Process - Denied";
                    emailTemplatePath += ConfigurationManager.AppSettings["DenyToSubmitter"];
                    break;
                case WorkflowState.Closed_Cancelled:
                    subject = "Supplemental Access Process - Cancelled";
                    emailTemplatePath += ConfigurationManager.AppSettings["DenyToSubmitter"];
                    break;
            }

			ConfigPerEnvironment(requestId, PageNames.USER_VIEW);

			/*
			Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(emailAddress(submitterUserId),
																	 subject,
																	 emailTemplatePath, // newTaskNotification.html",
																	 new Hashtable()
																		 {
																			 //{"APPROVERNAME", name},
																			 {"NAME", affectedEndUser},
																			 {"URL", _followLinkUrl},
																			 {"REASON", reason},
																			 {"PREFIX", _imageUrl}
																		 });

			 */
		}

        public static void TaskAssignToApprover(string toEmailAddress, string to, long requestId, string affectedEndUser)
        {
			ConfigPerEnvironment(requestId, PageNames.APPROVING_MANAGER);
            
            Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(toEmailAddress,
                                                                     "Supplemental Access Process - Approval Needed",
                                                                     AbsolutePath + ConfigurationManager.AppSettings["Approval"], // newTaskNotification.html",
                                                                     new Hashtable()
                                                                         {
                                                                             {"APPROVERNAME", to},
                                                                             {"NAME", affectedEndUser},
                                                                             {"URL", _followLinkUrl},
                                                                             {"PREFIX", _imageUrl}
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

        private static string AbsolutePath
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
        private static void ConfigPerEnvironment(long requestId, string pageName)
        {
            _imageUrl = @"http://";
            _followLinkUrl = @"http://";

            if (Environment.UserDomainName.ToUpper().Contains("DEV"))
            {
                _imageUrl += Environment.MachineName + ".devapollogrp.edu/snap/images";
				_followLinkUrl += (Environment.MachineName + ".devapollogrp.edu/snap/" + pageName + ".aspx?RequestId=" + requestId);
            }
            else if (Environment.UserDomainName.ToUpper().Contains("QA"))
            {
                _imageUrl += "access.qaapollogrp.edu/snap/images";
				_followLinkUrl += ("access.qaapollogrp.edu/snap/" + pageName + ".aspx?RequestId=" + requestId);
            }
            else
            {
                _imageUrl += "access.apollogrp.edu/snap/images";
				_followLinkUrl += ("access.apollogrp.edu/snap/" + pageName + ".aspx?RequestId=" + requestId);
            }
        }
    }
}
