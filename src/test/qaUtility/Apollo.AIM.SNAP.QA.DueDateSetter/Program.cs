﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.QA.DueDateSetter
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Set due date for request to fire the email nag ...");
            Console.WriteLine("What is your intervals(in days):");
            var interval = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("What is your max over due (in days) to close abandoned  the request:");
            var maxOverDue = Convert.ToInt16(Console.ReadLine());

            Console.WriteLine("Due on 1 - Tomorrow, 2 - Now (for dev mostly)");
            var dueDateOption = Convert.ToInt16(Console.ReadLine());

            var answer = "Y";
            while (answer.Contains("Y")) {
            
                Console.WriteLine("Enter your request id which has approver waiting to approve the request");
                var requestId = Console.ReadLine();
                using (var db = new SNAPDatabaseDataContext())
                {
                    var req = db.SNAP_Requests.Single(x => (x.pkId == Convert.ToInt32(requestId) && x.statusEnum != (byte)RequestState.Closed) );
                    Console.WriteLine("req id: " + req.pkId + " - " + req.userDisplayName);
                    foreach(var wf in req.SNAP_Workflows)
                    {
                        var latestState = wf.SNAP_Workflow_States.OrderByDescending(s => s.pkId).First();
                        if (canChangeDueDate(latestState))
                        {
                            Console.WriteLine(wf.SNAP_Actor.displayName + "... " + Enum.GetName(typeof(WorkflowState), latestState.workflowStatusEnum));
                            var newDueDate = askUserToSelectDueDate(interval, dueDateOption, maxOverDue);

                            latestState.dueDate = newDueDate;
                            db.SubmitChanges();
                        }
                    }
                }

                Console.WriteLine("Press 'y' to continue .... any key to quit ...");
                answer = Console.ReadLine().ToUpper();
            }
            Console.WriteLine("Have a good day! Press enter ....");
            Console.ReadKey();
        }

        static bool canChangeDueDate(SNAP_Workflow_State latestState)
        {
            return (latestState.workflowStatusEnum == (byte) WorkflowState.Change_Requested ||
                    latestState.workflowStatusEnum == (byte) WorkflowState.Pending_Approval)
                   &&
                   (latestState.completedDate == null && latestState.notifyDate != null && latestState.dueDate != null);
        }

        static List<DateTime> displayListOfDueDate(int overdueAlertIntervalInDays, int dueDateOption, int maxOverdue)
        {
            DateTime dueDate=DateTime.Now;
            if (dueDateOption == 1)
            {
                DateTime tomorrow = DateTime.Now.AddDays(1);
                dueDate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 2, 0, 0);
            }

            var lst = new List<DateTime>();
            for(int i = 10; i >= 0; i--)
            {
                DateTime t = DateTime.Now.AddDays(-i);
                //Console.WriteLine(t);
                TimeSpan diff = t.Subtract(dueDate);
                //Console.WriteLine("Date diff: " + diff.Days);
                if (Math.Abs(diff.Days) % overdueAlertIntervalInDays == 1 || overdueAlertIntervalInDays == 1)
                {
                    lst.Add(t);
                }
            }
            lst.Add(dueDate.AddDays(-maxOverdue));
            return lst;
        }

        static DateTime askUserToSelectDueDate(int interval, int dueDateOption, int maxOverdue)
        {
            Console.WriteLine("Here is a list of potential new due dates:");
            var dueDates = displayListOfDueDate(interval, dueDateOption, maxOverdue).ToArray();
            for (int i = 0; i< dueDates.Length; i++)
            {
                var info = string.Empty;

                if (i == dueDates.Length - 1)
                    info = " <=== Close-abondaned date!";
                Console.WriteLine("{0}: {1} {2}", i,dueDates[i], info);
            }
            Console.WriteLine("Which one(enter index number)?");
            var x = Convert.ToInt16(Console.ReadLine());
            return dueDates[x];
        }

    }
}
