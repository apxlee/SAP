﻿
namespace Apollo.AIM.SNAP.Web.Common
{
	public enum Role
	{
		NotAuthenticated, 
		Requestor,
		ApprovingManager,
		AccessTeam,
		SuperUser
	}
	
	public enum RequestState
	{
		Open,
		Pending,
		Change_Requested,
		Closed
	}
	
	public enum WorkflowState
	{
		Approved,
		Change_Requested,
		Closed_Cancelled,
		Closed_Completed,
		Closed_Denied,
		Not_Active,
		Pending_Acknowlegement,
		Pending_Approval,
		Pending_Provisioning,
		Pending_Workflow,
		Workflow_Created
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
	
	public enum ViewIndex
	{
		login = 0,
		request_form = 1,
		my_requests = 2,
		my_approvals = 3,
		access_team = 4,
		search = 5,
		support = 6
	}
}
