
namespace Apollo.AIM.SNAP.Web.Common
{
	public enum Role
	{
		User,
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
}
