﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
	public enum Role
	{
		NotAuthorized,
		Requestor,
		ApprovingManager,
		AccessTeam,
		SuperUser
	}
	
    public enum RequestState
    {
        Open = 0,
        Change_Requested = 1,
        Pending = 2,
        Closed = 3,
        Search = 4
    }

    public enum WorkflowState
    {
        Approved = 0,
        Change_Requested = 1,
        Closed_Cancelled = 2,
        Closed_Completed = 3,
        Closed_Denied = 4,
        Not_Active = 5,
        Pending_Acknowledgement = 6,
        Pending_Approval = 7,
        Pending_Provisioning = 8,
        Pending_Workflow = 9,
        Workflow_Created = 10,
        In_Workflow = 11,
        Closed_Abandon = 12
    }

	// TODO: need to tweak how comments are written to db
    public enum CommentsType
    {
        Denied = 1,
        Cancelled = 2,
        Requested_Change = 3,
        Email_Reminder = 4,
        Commented = 10,
        Acknowledged = 11,
        Workflow_Created = 12,
        Ticket_Created = 13,
        Access_Notes_Requestor = 5,
        Access_Notes_ApprovingManager = 6,
        Access_Notes_AccessTeam = 7,
        Access_Notes_SuperUser = 9,
        Abandon = 10
    }

	// TODO: rename this enum to match database table
	public enum ActorApprovalType
	{
		Team_Approver = 0,
		Technical_Approver = 1,
		Manager = 2,
		Workflow_Admin = 3
	}
    
    public enum ActorGroupType
    {
		Team_Approver = 0,
		Technical_Approver = 1,
		Manager = 2,
		Workflow_Admin = 3
    }

    public enum WorkflowAction
    {
        Approved = 0,
        Denied = 1,
        Change = 2,
        Cancel = 3,
        Ack = 4,
        Ticket = 5,
        Complete = 6,
        Abandon = 7
    }
    
    public enum EmailTaskType
    {
		OverdueApproval,
        //OverdueApprovalCC,
        OverdueChangeRequested,
		UpdateRequester,
		AssignToApprover,
		AccessTeamAcknowledge,
		ProxyForAffectedEndUser,
		TransitionToPendingProvisioning
    }
}
