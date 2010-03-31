using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
    public class Email
    {
        public static string link = @"http://access/snap/index.aspx?Requestid=";
        public static string OverdueTask(string to, string id)
        {
            var subject = "Overdue - A workflow task has been assigned to you";
            var body = "A task is waiting for your approval. For detail, <a href='" + link + id + "'>visit our site</a>";
            return body;
        }

        public static string UpdateRequesterStatus(string toUsrId, string id, string status)
        {
            var subjecct = "Access Request Status";
            var body = "Your request has been " + status + ". For detail, <a href='" + link + id + "'>visit our site</a>"; // processed, denied, requested to change
            return body;
        }

        public static string TaskAssignToApprover(string toEmailAddress, string id)
        {
            var subject = "New - A workflow task has been assigned to you";
            var body = "A task is waiting for your approval. For detail, <a href='" + link + id + "'>visit our site</a>";
            return body;
        }

        private static string emailAddress(string usrId)
        {
            return "x.apollogrp.edu";
        }
    }
}
