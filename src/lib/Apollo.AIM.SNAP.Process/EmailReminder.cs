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
            Utilities.OutputMessage("EmailReminderlogger: Service Start \n", EventLogEntryType.Information);
            Logger.Info("SNAP Email Reminder started");
            _timer.Start();

        }

        protected override void OnStop()
        {
            Utilities.OutputMessage("EmailReminderlogger: Service Stop \n", EventLogEntryType.Information);
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
            Utilities.OutputMessage("EmailReminderlogger: time elapsed, check time: " + checkTime, EventLogEntryType.Information);
                                                              
            if (DateTime.Now.Hour >= checkTime && !done)
            {
                Utilities.OutputMessage("EmailReminderlogger: Checking overdue tasks ... ", EventLogEntryType.Information);
                emailApproverForOverdueTask();

                done = true;

            }

            if (DateTime.Now.Hour < checkTime)
            {
                done = false;
            }

        }


        private void emailApproverForOverdueTask()
        {
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var unfinishedtasks = db.SNAP_Workflow_States.Where(s => s.completedDate == null
                                                                             && s.dueDate != null
                                                                             && s.notifyDate != null
                                                                             && s.SNAP_Workflow.SNAP_Request.statusEnum != (byte)RequestState.Closed); 
                    foreach (var state in unfinishedtasks)
                    {
                        try
                        {
                            var task = ReminderTask.CreateReminderTask(state);
                            if (task.RemindUser())
                                db.SubmitChanges();

                        }
                        catch (Exception ex)
                        {
                            Utilities.OutputMessage("State id: " + state.pkId + ",WF id: " + state.workflowId + ", Stack Trace : " + ex.StackTrace, EventLogEntryType.Error);
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                Utilities.OutputMessage(ex.Message + ", Stack Trace : " + ex.StackTrace, EventLogEntryType.Error);
            }
        }

    }
}
