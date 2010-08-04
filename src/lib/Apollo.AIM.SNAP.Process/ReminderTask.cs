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

        protected SNAP_Workflow_State wfState;

        public static ReminderTask CreateReminderTask(SNAP_Workflow_State state)
        {
            ReminderTask tsk = null;

            var overdueType = Enum.GetName(typeof(WorkflowState), state.workflowStatusEnum);
            Utilities.OutputMessage(overdueType + ", Req id: " + state.SNAP_Workflow.SNAP_Request.pkId + ",WF id: " + state.SNAP_Workflow.pkId + "-" +
                state.SNAP_Workflow.SNAP_Actor.displayName + " due on: " + state.dueDate, EventLogEntryType.Information);

            if (overdue(state, overdueType))
            {
                if (state.workflowStatusEnum == (byte) WorkflowState.Pending_Approval &&
                    state.SNAP_Workflow.SNAP_Actor.pkId != AimActorID)
                    tsk =  new ApproverReminderTask(state);

                if (state.workflowStatusEnum == (byte) WorkflowState.Change_Requested)
                    tsk =  new ChangeRequestedReminderTask(state);

            }

            if (tsk != null)
                Utilities.OutputMessage(overdueType + ", Req id: " + state.SNAP_Workflow.SNAP_Request.pkId + ",WF id: " + state.SNAP_Workflow.pkId + " email nag", EventLogEntryType.Information);
            else
            {
                tsk = new ReminderTask(state);
            }

            return tsk;
        }


        public ReminderTask(SNAP_Workflow_State state)
        {
            wfState = state;
        }


        public virtual bool RemindUser()
        {
            return false;
        }

        static bool overdue(SNAP_Workflow_State wfState, string overdueType)
        {
            DateTime dueDate = DateTime.Parse(wfState.dueDate.ToString());
            TimeSpan diff = DateTime.Now.Subtract(dueDate);


            int overdueAlertIntervalInDays = Convert.ToInt16(ConfigurationManager.AppSettings["OverdueAlertIntervalInDays"]);
            int overdueAlertMaxDay = Convert.ToInt16(ConfigurationManager.AppSettings["OverdueAlertMaxDay"]);

            if (diff.Days < overdueAlertMaxDay)
            {
                // at least more than 24 hours over due
                if (diff.Days % overdueAlertIntervalInDays == 1) // also every x interval days to send alert
                {
                    wfState.SNAP_Workflow.SNAP_Workflow_Comments.Add(new SNAP_Workflow_Comment()
                    {
                        commentText = "Overdue:" + overdueType + " Alert",
                        commentTypeEnum = (byte)CommentsType.Email_Reminder,
                        createdDate = DateTime.Now
                    });

                    return true;
                }
            }
            return false;
        }
    }

    class ApproverReminderTask : ReminderTask
    {
        public ApproverReminderTask(SNAP_Workflow_State st) : base(st) {}
        public override bool RemindUser()
        {
            return (Email.SendTaskEmail(EmailTaskType.OverdueApproval
                                        , wfState.SNAP_Workflow.SNAP_Actor.emailAddress
                                        , wfState.SNAP_Workflow.SNAP_Actor.displayName
                                        , wfState.SNAP_Workflow.SNAP_Request.pkId
                                        , wfState.SNAP_Workflow.SNAP_Request.userDisplayName));
            /*

                    &&
                    Email.SendTaskEmail(EmailTaskType.OverdueApprovalCC
                                , wfState.SNAP_Workflow.SNAP_Request.userId + "@apollogrp.edu"
                                , wfState.SNAP_Workflow.SNAP_Actor.displayName
                                , wfState.SNAP_Workflow.SNAP_Request.pkId
                                , wfState.SNAP_Workflow.SNAP_Request.userDisplayName));
             */
        }
    }

    class ChangeRequestedReminderTask : ReminderTask
    {
        public ChangeRequestedReminderTask(SNAP_Workflow_State st) : base(st) {}

        public override bool RemindUser()
        {
            return Email.SendTaskEmail(EmailTaskType.OverdueChangeRequested
                , wfState.SNAP_Workflow.SNAP_Request.userId + "@apollogrp.edu"
                , wfState.SNAP_Workflow.SNAP_Request.userDisplayName
                , wfState.SNAP_Workflow.SNAP_Request.pkId
                , wfState.SNAP_Workflow.SNAP_Request.userDisplayName
                , WorkflowState.Change_Requested
                , requestToChangeReason(wfState.SNAP_Workflow.SNAP_Workflow_Comments));

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
