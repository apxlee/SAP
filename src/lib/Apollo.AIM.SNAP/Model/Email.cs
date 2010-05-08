using System;
using System.Collections;

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Apollo.Ultimus.CAP;
using Apollo.AIM.SNAP.CA;
using Apollo.CA.Logging;

using System.Web;

namespace Apollo.AIM.SNAP.Model
{
    public class Email
    {
		//private static string _imageUrl = @"http://";
		//private static string _followLinkUrl = @"http://";

		public static bool SendTaskEmail(EmailTaskType taskType, string toEmailAddress, string toName, long requestId, string affectedEndUser) //, WorkflowState workflowState, string reason
		{
			string recipientName = string.Empty;
			string followPageName = string.Empty;
			string subjectAction = "Supplemental Access Process - ";
			string templatePath = Utilities.AbsolutePath;
			
			switch (taskType)
			{
				case EmailTaskType.AccessTeamAcknowledge:
					recipientName = "Access Team";
					followPageName = PageNames.ACCESS_TEAM;
					subjectAction += "Acknowledgement Needed";
					templatePath += ConfigurationManager.AppSettings["Acknowledgement"];
					break;
					
				case EmailTaskType.Overdue:
					recipientName = toName;
					followPageName = PageNames.APPROVING_MANAGER;
					subjectAction += "Overdue Alert";
					templatePath += ConfigurationManager.AppSettings["NagApproval"];
					break;
					
				case EmailTaskType.AssignToApprover:
					recipientName = toName;
					followPageName = PageNames.APPROVING_MANAGER;
					subjectAction += "Approval Needed";
					templatePath += ConfigurationManager.AppSettings["Approval"];
					break;							
			}
			
			Hashtable bodyParameters = new Hashtable()
				{
					{"ROOT_PATH", Utilities.WebRootUrl},
					{"RECIPIENT_NAME", recipientName},
					{"AFFECTED_END_USER", affectedEndUser},
					{"FOLLOW_URL", Utilities.WebRootUrl + followPageName + ".aspx?requestId=" +  requestId}
				};

			return SendEmail(toEmailAddress, subjectAction, templatePath, bodyParameters);						
		}

		//public static void OverdueTask(string toEmail, string toName, long requestId, string userName)
		//{
		//    //ConfigPerEnvironment(requestId, PageNames.APPROVING_MANAGER);
            
		//    //Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(toEmail,
		//    //                                                         "Supplemental Access Process - Overdue Alert",
		//    //                                                         AbsolutePath + ConfigurationManager.AppSettings["NagApproval"], // newTaskNotification.html",
		//    //                                                         new Hashtable()
		//    //                                                             {
		//    //                                                                 {"APPROVERNAME", toName},
		//    //                                                                 {"NAME", userName},
		//    //                                                                 {"URL", _followLinkUrl},
		//    //                                                                 {"PREFIX", _imageUrl}
		//    //                                                             });
		//}

        public static void UpdateRequesterStatus(string submitterUserId, string name, long requestId, WorkflowState workflowState, string reason)
        {

			//string subject = "";
			//string emailTemplatePath = AbsolutePath;
			
			//switch (workflowState)
			//{
			//    case WorkflowState.Pending_Acknowledgement:
			//        subject = "Supplemental Access Process - Submitted";
			//        emailTemplatePath +=  ConfigurationManager.AppSettings["ConfirmSubmitToSubmitter"];
			//        break;
			//    case WorkflowState.Closed_Completed:
			//        subject = "Supplemental Access Process - Completed";
			//        emailTemplatePath += ConfigurationManager.AppSettings["CompleteToSubmitter"];
			//        break;
			//    case WorkflowState.Change_Requested:
			//        subject = "Supplemental Access Process - Request Change";
			//        emailTemplatePath += ConfigurationManager.AppSettings["RequestChangeToSubmitter"];
			//        break;
			//    case WorkflowState.Closed_Denied:
			//        subject = "Supplemental Access Process - Denied";
			//        emailTemplatePath += ConfigurationManager.AppSettings["DenyToSubmitter"];
			//        break;
			//    case WorkflowState.Closed_Cancelled:
			//        subject = "Supplemental Access Process - Cancelled";
			//        emailTemplatePath += ConfigurationManager.AppSettings["DenyToSubmitter"];
			//        break;
			//}

			//ConfigPerEnvironment(requestId, PageNames.USER_VIEW);

			//Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(emailAddress(submitterUserId),
			//                                                         subject,
			//                                                         emailTemplatePath, // newTaskNotification.html",
			//                                                         new Hashtable()
			//                                                             {
			//                                                                 {"APPROVERNAME", name},
			//                                                                 {"NAME", affectedEndUser},
			//                                                                 {"URL", _followLinkUrl},
			//                                                                 {"REASON", reason},
			//                                                                 {"PREFIX", _imageUrl}
			//                                                             });

		}

		//public static void TaskAssignToApprover(string toEmailAddress, string to, long requestId, string affectedEndUser)
		//{
		//    //ConfigPerEnvironment(requestId, PageNames.APPROVING_MANAGER);
            
		//    //Apollo.Ultimus.CAP.FormattedEmailTool.SendFormattedEmail(toEmailAddress,
		//    //                                                         "Supplemental Access Process - Approval Needed",
		//    //                                                         AbsolutePath + ConfigurationManager.AppSettings["Approval"], // newTaskNotification.html",
		//    //                                                         new Hashtable()
		//    //                                                             {
		//    //                                                                 {"APPROVERNAME", to},
		//    //                                                                 {"NAME", affectedEndUser},
		//    //                                                                 {"URL", _followLinkUrl},
		//    //                                                                 {"PREFIX", _imageUrl}
		//    //                                                             });
		//}

		//public static void AccessTeamAcknowledge(string toEmailAddress, long requestId, string affectedEndUser)
		//{
		//    //Hashtable bodyParameters = new Hashtable()
		//    //    {
		//    //        {"ROOT_PATH", Utilities.WebRootUrl},
		//    //        {"RECIPIENT_NAME", "Access Team"},
		//    //        {"AFFECTED_END_USER", affectedEndUser},
		//    //        {"FOLLOW_URL", Utilities.WebRootUrl + PageNames.ACCESS_TEAM +  ".aspx?requestId=" +  requestId}
		//    //    };
			
		//    //SendEmail(toEmailAddress
		//    //    , "Supplemental Access Process - Acknowledgement Needed"
		//    //    , Utilities.AbsolutePath + ConfigurationManager.AppSettings["Acknowledgement"]
		//    //    , bodyParameters);
		//}

		private static bool SendEmail(string recipientEmailAddress, string subject, string templatePath, Hashtable bodyParameters)
		{
			try
			{
				FormattedEmailTool.SendFormattedEmail(recipientEmailAddress, subject, templatePath, bodyParameters);
			}
			catch (Exception ex)
			{
				Logger.Error("Email > SendEmail", ex);
				return false;
			}
			return true;
		}

        private static string emailAddress(string usrId)
        {
			// for unit test only
#if DEBUG
	if (usrId == "UnitTester") {return "pong.lee@apollogrp.edu";}
#endif
            ADUserDetail detail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(usrId);
            return detail.EmailAddress;
        }

        // TODO: need to put this in utilities so write comments works
		//private static void ConfigPerEnvironment(long requestId, string pageName)
		//{
		//    _imageUrl = @"http://";
		//    _followLinkUrl = @"http://";

		//    if (Environment.UserDomainName.ToUpper().Contains("DEV"))
		//    {
		//        _imageUrl += Environment.MachineName + ".devapollogrp.edu/snap/images";
		//        _followLinkUrl += (Environment.MachineName + ".devapollogrp.edu/snap/" + pageName + ".aspx?RequestId=" + requestId);
		//    }
		//    else if (Environment.UserDomainName.ToUpper().Contains("QA"))
		//    {
		//        _imageUrl += "access.qaapollogrp.edu/snap/images";
		//        _followLinkUrl += ("access.qaapollogrp.edu/snap/" + pageName + ".aspx?RequestId=" + requestId);
		//    }
		//    else
		//    {
		//        _imageUrl += "access.apollogrp.edu/snap/images";
		//        _followLinkUrl += ("access.apollogrp.edu/snap/" + pageName + ".aspx?RequestId=" + requestId);
		//    }

		//    _imageUrl = "http://dwaxulbp001.devapollogrp.edu/snap/images";
		//    _followLinkUrl = "http://dwaxulbp001.devapollogrp.edu/snap/" + pageName + ".aspx?RequestId=" + requestId;
		//}
    }
}
