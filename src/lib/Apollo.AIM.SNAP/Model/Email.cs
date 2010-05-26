using System;
using System.Collections;
using System.Configuration;

using Apollo.AIM.SNAP.CA;
using Apollo.CA.Logging;
using Apollo.Ultimus.CAP;

namespace Apollo.AIM.SNAP.Model
{
    public class Email
    {
		public static bool SendTaskEmail(EmailTaskType taskType, string toEmailAddress, string toName, long requestId, string affectedEndUser)
		{
			return SendTaskEmail(taskType, toEmailAddress, toName, requestId, affectedEndUser, WorkflowState.Not_Active, null);
		}    
		
		public static bool SendTaskEmail(EmailTaskType taskType, string toEmailAddress, string toName, long requestId, string affectedEndUser, WorkflowState workflowState, string reason)
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

				case EmailTaskType.ProxyForAffectedEndUser:
					recipientName = toName;
					followPageName = PageNames.USER_VIEW;
					subjectAction += "Submitted";
					templatePath += ConfigurationManager.AppSettings["RequestorForAEU"];
					break;
					
				case EmailTaskType.UpdateRequester:
					recipientName = toName;

					switch (workflowState)
					{
						case WorkflowState.Pending_Acknowledgement:
							subjectAction += "Submitted";
							followPageName = PageNames.USER_VIEW;
							templatePath += ConfigurationManager.AppSettings["ConfirmSubmitToSubmitter"];
							break;
							
						case WorkflowState.Closed_Completed:
							subjectAction += "Completed";
							followPageName = PageNames.USER_VIEW;
							templatePath += ConfigurationManager.AppSettings["CompleteToSubmitter"];
							break;
							
						case WorkflowState.Change_Requested:
							subjectAction += "Change Has Been Requested";
							followPageName = PageNames.REQUEST_FORM;
							templatePath += ConfigurationManager.AppSettings["RequestChangeToSubmitter"];
							break;
							
						case WorkflowState.Closed_Denied:
							subjectAction += "Denied";
							followPageName = PageNames.USER_VIEW;
							templatePath += ConfigurationManager.AppSettings["DenyToSubmitter"];
							break;
							
						case WorkflowState.Closed_Cancelled:
							subjectAction += "Cancelled";
							followPageName = PageNames.USER_VIEW;
							templatePath += ConfigurationManager.AppSettings["DenyToSubmitter"];
							break;
					}
					break;
			}
			
			Hashtable bodyParameters = new Hashtable()
				{
					{"ROOT_PATH", Utilities.WebRootUrl},
					{"RECIPIENT_NAME", recipientName},
					{"AFFECTED_END_USER", affectedEndUser},
					{"FOLLOW_URL", Utilities.WebRootUrl + followPageName + ".aspx?requestId=" +  requestId},
					{"REASON", reason},
					{"REQUEST_ID", requestId}
				};

			return SendEmail(toEmailAddress, subjectAction, templatePath, bodyParameters);						
		}

		private static bool SendEmail(string recipientEmailAddress, string subject, string templatePath, Hashtable bodyParameters)
		{
			try
			{
				if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendAllEmailToAIM"]))
				{
					FormattedEmailTool.SendFormattedEmail(ConfigurationManager.AppSettings["AIM-DG"], subject, templatePath, bodyParameters);
				}
				else
				{
					// TODO: Hardcoded above for development, don't want emails/nags going to managers
					//
					//FormattedEmailTool.SendFormattedEmail(recipientEmailAddress, subject, templatePath, bodyParameters);
				}
			}
			catch (Exception ex)
			{
                Logger.Error("Email - SendEmail, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
				return false;
			}
			return true;
		}
    }
}
