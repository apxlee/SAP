using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using Apollo.AIM.SNAP.Model;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Process
{
    public partial class EmailReminder : ServiceBase
    {
        private Timer _timer = null;
        private  bool done = false;
        private const int AccessTeamId = 1;
        //private string fileName = @"d:\temp\EmailReminderlogger.txt";

        public EmailReminder()
        {
            InitializeComponent();
            ServiceName = "SNAP Email Reminder";
            var pollingInterval = Convert.ToInt32(ConfigurationManager.AppSettings["PollingIntervalinMsec"]);

            // Set the timer to fire 
            // (remember the timer is in millisecond resolution,
            //  so 1000 = 1 second. )

            //_timer = new Timer(1800000);

            _timer = new Timer(pollingInterval);
            //    _timer = new Timer(30000);

            // Now tell the timer when the timer fires
            // (the Elapsed event) call the _timer_Elapsed
            // method in our code

            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);

        }

        protected override void OnStart(string[] args)
        {
            outputMessage("EmailReminderlogger: Service Start \n", EventLogEntryType.Information);
            Logger.Info("SNAP Email Reminder started");
            _timer.Start();

        }

        protected override void OnStop()
        {
            outputMessage("EmailReminderlogger: Service Stop \n", EventLogEntryType.Information);
            Logger.Info("SNAP Email Reminder stopped");
            _timer.Stop();

        }


        protected override void OnContinue()
        {
            base.OnContinue();
            _timer.Start();
        }



        protected override void OnPause()
        {
            base.OnPause();
            _timer.Stop();
        }



        protected override void OnShutdown()
        {
            base.OnShutdown();
            _timer.Stop();
        }

        protected void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var checkTime = Convert.ToInt16(ConfigurationManager.AppSettings["CheckTimeInHour"]);
            outputMessage("EmailReminderlogger: time elapsed, check time: " + checkTime, EventLogEntryType.Information);
                                                              
            if (DateTime.Now.Hour >= checkTime && !done)
            {
                outputMessage("EmailReminderlogger: Checking overdue tasks ... ", EventLogEntryType.Information);
                emailApproverForOverdueTask();

                done = true;

            }

            if (DateTime.Now.Hour < checkTime)
            {
                done = false;
            }

        }


        private void outputMessage(string msg, EventLogEntryType type)
        {
            /*
            EventLog evt = new EventLog();
            evt.Source = this.ServiceName;
            evt.WriteEntry(msg, type);
             */

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

        private void emailApproverForOverdueTask()
        {
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var unfinishedtasks = db.SNAP_Workflow_States.Where(s => s.completedDate == null);
                    foreach (var state in unfinishedtasks)
                    {
                        try
                        {
                            if (state.SNAP_Workflow.SNAP_Request.statusEnum != (byte) RequestState.Closed)
                            {
                                // TODO - should be remind submitter to work on 'request to change', SLA is 3 days or does that matter
                                /**
                                if ((state.workflowStatusEnum == (byte) WorkflowState.Pending_Approval 
                                    || state.workflowStatusEnum == (byte)WorkflowState.Change_Requested
                                    ) && state.SNAP_Workflow.SNAP_Actor.pkId != AccessTeamId)
                                 */
                                 if ((state.workflowStatusEnum == (byte)WorkflowState.Pending_Approval
                                      && state.SNAP_Workflow.SNAP_Actor.pkId != 1)
                                    ||
                                    (state.workflowStatusEnum == (byte)WorkflowState.Change_Requested))
                                {
                                    // only remind approvers, not access team

                                    outputMessage(
                                        Enum.GetName(typeof (WorkflowState), state.workflowStatusEnum) + ", Req id: " + state.SNAP_Workflow.SNAP_Request.pkId +  ",WF id: " + state.SNAP_Workflow.pkId + "-" +
                                        state.SNAP_Workflow.SNAP_Actor.displayName + " due on: " + state.dueDate,
                                        EventLogEntryType.Information);


                                    if (state.dueDate != null && state.notifyDate != null)
                                    {
                                        DateTime dueDate = DateTime.Parse(state.dueDate.ToString());
                                        TimeSpan diff = DateTime.Now.Subtract(dueDate);

                                        if (ReadyToSendOverdueAlert(diff))
                                        {
                                            state.SNAP_Workflow.SNAP_Workflow_Comments.Add(new SNAP_Workflow_Comment()
                                                                                               {
                                                                                                   commentText = "OverdueApproval Alert",
																								   commentTypeEnum = (byte) CommentsType.Email_Reminder,
																								   createdDate = DateTime.Now
                                                                                               });

                                            outputMessage(Enum.GetName(typeof(WorkflowState), state.workflowStatusEnum) + ", Req id: " + state.SNAP_Workflow.SNAP_Request.pkId + ",WF id: " + state.SNAP_Workflow.pkId + " gets email nag",
                                                          EventLogEntryType.Information);

                                            if (state.workflowStatusEnum == (byte)WorkflowState.Pending_Approval)
                                            {
                                                Email.SendTaskEmail(EmailTaskType.OverdueApproval
                                                                    , state.SNAP_Workflow.SNAP_Actor.emailAddress
                                                                    , state.SNAP_Workflow.SNAP_Actor.displayName
                                                                    , state.SNAP_Workflow.SNAP_Request.pkId
                                                                    , state.SNAP_Workflow.SNAP_Request.userDisplayName);
                                            }
                                            else if (state.workflowStatusEnum == (byte)WorkflowState.Change_Requested)
                                            {
                                                Email.SendTaskEmail(EmailTaskType.OverdueChangeRequested
                                                                    , state.SNAP_Workflow.SNAP_Request.userId +"@apollogrp.edu"
                                                                    , state.SNAP_Workflow.SNAP_Request.userDisplayName
                                                                    , state.SNAP_Workflow.SNAP_Request.pkId
                                                                    , state.SNAP_Workflow.SNAP_Request.userDisplayName
                                                                    , WorkflowState.Change_Requested
                                                                    , requestToChangeReason(state.SNAP_Workflow.SNAP_Workflow_Comments));
                                                
                                            } 
                                            db.SubmitChanges();
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            outputMessage("State id: " + state.pkId + ",WF id: " + state.workflowId + ", Stack Trace : " + ex.StackTrace, EventLogEntryType.Error);
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                outputMessage(ex.Message + ", Stack Trace : " + ex.StackTrace, EventLogEntryType.Error);
            }
        }

        private bool ReadyToSendOverdueAlert(TimeSpan diff)
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

        private string requestToChangeReason(IEnumerable<SNAP_Workflow_Comment> comments)
        {
            var comment = comments.Where(c => c.commentTypeEnum == (byte) CommentsType.Requested_Change)
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
