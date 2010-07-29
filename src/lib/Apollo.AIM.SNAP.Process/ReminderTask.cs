using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Apollo.AIM.SNAP.Model;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Process
{
    public class ReminderTask
    {
        static int AimActorID = 1;
        private string overdueType = string.Empty; 

        protected SNAP_Workflow_State wfState;

        public static ReminderTask CreateReminderTask(SNAP_Workflow_State state)
        {
            if (state.workflowStatusEnum == (byte)WorkflowState.Pending_Approval && state.SNAP_Workflow.SNAP_Actor.pkId != AimActorID)
                return new ApproverReminderTask(state);

            if (state.workflowStatusEnum == (byte)WorkflowState.Change_Requested)
                return new ChangeRequestedReminderTask(state);

            return null;
        }


        public ReminderTask(SNAP_Workflow_State state)
        {
            wfState = state;
            overdueType = Enum.GetName(typeof(WorkflowState), wfState.workflowStatusEnum);

            outputMessage(overdueType + ", Req id: " + state.SNAP_Workflow.SNAP_Request.pkId + ",WF id: " + state.SNAP_Workflow.pkId + "-" +
                            state.SNAP_Workflow.SNAP_Actor.displayName + " due on: " + state.dueDate,EventLogEntryType.Information);

        }


        public virtual bool SendReminder()
        {
            if (overdue)
            {
                wfState.SNAP_Workflow.SNAP_Workflow_Comments.Add(new SNAP_Workflow_Comment()
                {
                    commentText = "Overdue:" + overdueType + " Alert",
                    commentTypeEnum = (byte)CommentsType.Email_Reminder,
                    createdDate = DateTime.Now
                });

                outputMessage(overdueType + ", Req id: " + wfState.SNAP_Workflow.SNAP_Request.pkId + ",WF id: " + wfState.SNAP_Workflow.pkId + " gets email nag",
                      EventLogEntryType.Information);

                return true;
            }

            return false;
        }

        private bool overdue
        {
            get
            {
                if (wfState.dueDate != null && wfState.notifyDate != null)
                {
                    DateTime dueDate = DateTime.Parse(wfState.dueDate.ToString());
                    TimeSpan diff = DateTime.Now.Subtract(dueDate);

                    return checkoverdueDays(diff);
                }
                return false;
            }
        }

        private bool checkoverdueDays(TimeSpan diff)
        {
            int overdueAlertIntervalInDays = Convert.ToInt16(ConfigurationManager.AppSettings["OverdueAlertIntervalInDays"]);
            int overdueAlertMaxDay = Convert.ToInt16(ConfigurationManager.AppSettings["OverdueAlertMaxDay"]);

            if (diff.Days < overdueAlertMaxDay)
            {
                // at least more than 24 hours over due
                if (diff.Days % overdueAlertIntervalInDays == 1)
                    return true;

            }
            return false;
        }

        private void outputMessage(string msg, EventLogEntryType type)
        {

            switch (type)
            {
                case EventLogEntryType.Error:
                    Logger.Error(msg);
                    break;
                case EventLogEntryType.Information:
                    Logger.Info(msg);
                    break;
                case EventLogEntryType.Warning:
                    Logger.Warn(msg);
                    break;
                default:
                    Logger.Info(msg);
                    break;
            }
        }


    }

    class ApproverReminderTask : ReminderTask
    {
        public ApproverReminderTask(SNAP_Workflow_State st) : base(st) {}
        public override bool SendReminder()
        {
            if (base.SendReminder())
            {
                return Email.SendTaskEmail(EmailTaskType.OverdueApproval
                                    , wfState.SNAP_Workflow.SNAP_Actor.emailAddress
                                    , wfState.SNAP_Workflow.SNAP_Actor.displayName
                                    , wfState.SNAP_Workflow.SNAP_Request.pkId
                                    , wfState.SNAP_Workflow.SNAP_Request.userDisplayName);
            }
            return false;
        }
    }

    class ChangeRequestedReminderTask : ReminderTask
    {
        public ChangeRequestedReminderTask(SNAP_Workflow_State st) : base(st) {}

        public override bool SendReminder()
        {
            if (base.SendReminder())
            {
                return Email.SendTaskEmail(EmailTaskType.OverdueChangeRequested
                    , wfState.SNAP_Workflow.SNAP_Request.userId + "@apollogrp.edu"
                    , wfState.SNAP_Workflow.SNAP_Request.userDisplayName
                    , wfState.SNAP_Workflow.SNAP_Request.pkId
                    , wfState.SNAP_Workflow.SNAP_Request.userDisplayName
                    , WorkflowState.Change_Requested
                    , requestToChangeReason(wfState.SNAP_Workflow.SNAP_Workflow_Comments));

            }
            return false;
        }

        private string requestToChangeReason(IEnumerable<SNAP_Workflow_Comment> comments)
        {
            var comment = comments.Where(c => c.commentTypeEnum == (byte)CommentsType.Requested_Change)
                                  .OrderByDescending(c => c.pkId)
                                  .First();
            if (comment != null)
            {
                // we add a <span> tag for each comment that user enters, so we need to strip that
                return Regex.Split(comment.commentText, "<span")[0];
            }
            return string.Empty;
        }

    }
}
