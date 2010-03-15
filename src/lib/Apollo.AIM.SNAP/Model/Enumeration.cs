﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
    public enum RequestState
    {
        Open = 0,
        Change_Requested = 1,
        Pending = 2,
        Closed = 3
    }

    public enum WorkflowState
    {
        Approved = 0,
        Change_Requested = 1,
        Closed_Cancelled = 2,
        Closed_Completed = 3,
        Closed_Denied = 4,
        Not_Active = 5,
        Pending_Acknowlegement = 6,
        Pending_Approval = 7,
        Pending_Provisioning = 8,
        Pending_Workflow = 9,
        Workflow_Created = 10
    }

    public enum CommentsType
    {
        Denied = 1,
        Cancelled = 2,
        Requested_Change = 3,
        Email_Reminder = 4,
        Access_Notes_Requestor = 5,
        Access_Notes_ApprovingManager = 6,
        Access_Notes_AccessTeam = 7,
        Access_Notes_SuperUser = 9
    }
}
