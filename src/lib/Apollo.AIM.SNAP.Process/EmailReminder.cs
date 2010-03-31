using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Process
{
    public partial class EmailReminder : ServiceBase
    {
        private Timer _timer = null;
        private bool done = false;
        private string fileName = @"d:\temp\EmailReminderlogger.txt";

        public EmailReminder()
        {
            InitializeComponent();
            ServiceName = "SNAP Email Reminder";

            // Set the timer to fire 
            // (remember the timer is in millisecond resolution,
            //  so 1000 = 1 second. )

            _timer = new Timer(1800000);


            // Now tell the timer when the timer fires
            // (the Elapsed event) call the _timer_Elapsed
            // method in our code

            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);

        }

        protected override void OnStart(string[] args)
        {
            FileStream fs = new FileStream(fileName,FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            m_streamWriter.WriteLine(" EmailReminderlogger: Service Start \n"); m_streamWriter.Flush();
            m_streamWriter.Close();
            _timer.Start();

        }

        protected override void OnStop()
        {
            FileStream fs = new FileStream(fileName,FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            m_streamWriter.WriteLine(" EmailReminderlogger: Service Stopped \n"); m_streamWriter.Flush();
            m_streamWriter.Close();
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

            FileStream fs = new FileStream(fileName,FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            m_streamWriter.WriteLine(" EmailReminderlogger: time elapsed " + DateTime.Now + "\n");
            m_streamWriter.Flush();
            m_streamWriter.Close();

            if (DateTime.Now.Hour >= 2 && !done)
            {
                fs = new FileStream(fileName,FileMode.OpenOrCreate, FileAccess.Write);
                m_streamWriter = new StreamWriter(fs);
                m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                m_streamWriter.WriteLine(" EmailReminderlogger: DO it time " + DateTime.Now + "\n");
                m_streamWriter.Flush();
                m_streamWriter.Close();

                emailApproverForOverdueTask();

                done = true;
            }

            if (DateTime.Now.Hour < 2)
            {
                done = false;
            }

            /*
            EventLog evt = new EventLog("ArcaneTimeLogger");

            string message = "Arcane Time:" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

            evt.Source = "ArcaneTimeLoggerService";

            evt.WriteEntry(message, EventLogEntryType.Information);
             */

        }

        private void emailApproverForOverdueTask()
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
                            if (state.workflowStatusEnum == (byte) WorkflowState.Pending_Approval 
                                && state.SNAP_Workflow.SNAP_Actor.SNAP_Actor_Group.actorGroupType != (byte)ActorApprovalType.Workflow_Admin)
                                // only remind approvers, not access team

                                Console.WriteLine(state.pkId + "-" + state.SNAP_Workflow.SNAP_Actor.displayName +
                                                  " due on: " +
                                                  state.dueDate);

                            if (state.dueDate != null)
                            {
                                DateTime dueDate = DateTime.Parse(state.dueDate.ToString());
                                TimeSpan diff = DateTime.Now.Subtract(dueDate);

                                if (diff.Days == 1) // more than 24 hours over due
                                {
                                    state.SNAP_Workflow.SNAP_Workflow_Comments.Add(new SNAP_Workflow_Comment()
                                                                                       {
                                                                                           commentText = "over due!",
                                                                                           commentTypeEnum =
                                                                                               (byte)
                                                                                               CommentsType.
                                                                                                   Email_Reminder,
                                                                                           createdDate = DateTime.Now,
                                                                                       });

                                }


                                db.SubmitChanges();
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("state id = " + state.pkId + " Message: " + ex.Message);
                    }
                }
            }
        }

    }
}
