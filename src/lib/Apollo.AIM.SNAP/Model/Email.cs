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
			string subjectAction = GetSubjectAction;
			string templatePath = Utilities.AbsolutePath;


			switch (taskType)
			{
				case EmailTaskType.AccessTeamAcknowledge:
					recipientName = "Access Team";
					followPageName = PageNames.ACCESS_TEAM;
					subjectAction += "Acknowledgement Needed";
					templatePath += ConfigurationManager.AppSettings["Acknowledgement"];
					break;
					
				case EmailTaskType.OverdueApproval:
					recipientName = toName;
					followPageName = PageNames.APPROVING_MANAGER;
					subjectAction += "Approval Reminder";
					templatePath += ConfigurationManager.AppSettings["NagApproval"];
					break;

                /*
                case EmailTaskType.OverdueApprovalCC:
                    recipientName = toName;
                    followPageName = PageNames.APPROVING_MANAGER;
                    subjectAction = string.Format("{0}Approval is Overdue (FYI: You are copied on this email)", subjectAction);
                    templatePath += ConfigurationManager.AppSettings["NagApproval"];
                    break;
                 */

                case EmailTaskType.OverdueChangeRequested:
                    recipientName = toName;
                    followPageName = PageNames.USER_VIEW;
                    subjectAction += "Change Requested Reminder";
                    templatePath += ConfigurationManager.AppSettings["NagChangeRequested"];
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

				case EmailTaskType.TransitionToPendingProvisioning:
					recipientName = "Access Team";
					followPageName = PageNames.ACCESS_TEAM;
					subjectAction += "Pending Provisioning";
					templatePath += ConfigurationManager.AppSettings["TransitionToPendingProvisioning"];
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
                        case WorkflowState.Closed_Abandon:
                            subjectAction += "Abandoned";
                            followPageName = PageNames.USER_VIEW;
                            templatePath += ConfigurationManager.AppSettings["DenyToSubmitter"];
                            break;

					}
					break;
			}
			
			Hashtable bodyParameters = new Hashtable()
				{
					{"ROOT_PATH", Utilities.WebRootUrl},
					{"IMAGE_PATH", Utilities.ImagePath},
					{"RECIPIENT_NAME", recipientName},
					{"AFFECTED_END_USER", affectedEndUser},
					{"FOLLOW_URL", Utilities.WebRootUrl + followPageName + ".aspx?requestId=" +  requestId},
					{"REASON", reason},
					{"REQUEST_ID", requestId}
				};

			return SendEmail(toEmailAddress, subjectAction, templatePath, bodyParameters);						
		}

        public static string GetSubjectAction
        {
            get
            {
                var header = string.Empty;

                try
                {
                    var env = ConfigurationManager.AppSettings["EnvironmentPath"].ToLower();

                    if (env.Contains("devapollo"))
                    {
                        header = "* Dev * ";
                    }
                    else if (env.Contains("qaapollo"))
                    {
                        header = "* QA * ";
                    }
                }
                catch
                {
                    
                }
                return header + "Supplemental Access Process - ";
            }
        }

		private static bool SendEmail(string recipientEmailAddress, string subject, string templatePath, Hashtable bodyParameters)
		{
			Logger.Info(string.Format("Automated Email [SUBJECT:{0}]  [TO:{1}]  [REQID:{2}]\r\n"
					, subject.Split('-')[1].Trim(), recipientEmailAddress, bodyParameters["REQUEST_ID"].ToString()));

			try
			{
				if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendAllEmailToAIM"]))
				{
					FormattedEmailTool.SendFormattedEmail(ConfigurationManager.AppSettings["AIM-DG"], subject, templatePath, bodyParameters);
				}
				else
				{
					FormattedEmailTool.SendFormattedEmail(recipientEmailAddress, subject, templatePath, bodyParameters);
				}
			}
			catch (Exception ex)
			{
                Logger.Error("Email > SendEmail:\r\n" + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
				return false;
			}
			return true;
		}
    }
}


    //  http://www.systemnetmail.com/faq/4.4.aspx
    //      static void Main(string[] args)
    //    {
    //        var mail = new MailMessage("pxlee@apollogrp.edu", "pxlee@apollogrp.edu");
    //        var smtp = new SmtpClient("mailhost.apollogrp.edu");
    //        mail.Subject = "Embedded Images";
    //        var plainView = AlternateView.CreateAlternateViewFromString("plain text", null, @"text/plain");
    //        var htmlView = AlternateView.CreateAlternateViewFromString(
    //            @"<b>bold text with image</b><img alt='NunitLogo' hspace=0 src='cid:uniqueId' align=baseline border=0 />", 
    //            null, 
    //            @"text/html");
    //        /*
    //        var imgView = new AlternateView(@"E:\Program Files\NUnit 2.5.5\doc\img\logo.gif", MediaTypeNames.Image.Gif);
    //        imgView.ContentId = "uniqueId";
    //        imgView.TransferEncoding = TransferEncoding.Base64;
    //        */
            
    //        var img = new LinkedResource(@"E:\Program Files\NUnit 2.5.5\doc\img\logo.gif");
    //        img.ContentId = "uniqueId";
    //        htmlView.LinkedResources.Add(img);

    //        mail.AlternateViews.Add(plainView);
    //        mail.AlternateViews.Add(htmlView);
    //        //mail.AlternateViews.Add(imgView);
    //        smtp.Send(mail);
    //        Console.WriteLine("Sending ....");
    //        Console.ReadKey();
    //    }
    //}

