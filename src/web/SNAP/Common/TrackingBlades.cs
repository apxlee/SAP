using System;
using System.Data;
using System.Linq;
using System.Text;

using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Common
{
	public class TrackingBlades
	{
		private static DataTable _filteredTrackingData;
		private static DataTable _unfilteredTrackingData;

		public static DataTable GetAllTracking(string requestId)
		{
			// Get all the raw tracking data, then build an empty DataTable and use the helper 
			// methods below to parse the raw data into a filtered set...
			//
			_unfilteredTrackingData = Database.GetAllTrackingData(requestId);
			_filteredTrackingData = Database.BuildEmptyTrackingBladeTable();

			// Note: Build these backwards to force sort order...
			//
			BuildTechnicalApprovers();
			BuildTeamApprovers();
			BuildManagerTracking();
			BuildAIMTracking();

			return _filteredTrackingData;
		}

		#region Tracking Groups

		private static void BuildAIMTracking()
		{
			try
			{
				DataRow selectedRow;

				selectedRow = (
					from bladeRow in _unfilteredTrackingData.AsEnumerable()
					where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Workflow_Admin
					select bladeRow).First();

				if ((int)selectedRow["workflow_status"] == (int)WorkflowState.Workflow_Created)
				{
					selectedRow.SetField("workflow_status", WorkflowState.In_Workflow);
					selectedRow.SetField("workflow_due_date", string.Empty);
				}
				else if ((int)selectedRow["workflow_status"] == (int)WorkflowState.Approved)
				{
					selectedRow.SetField("workflow_status", WorkflowState.Pending_Provisioning);
				}

				_filteredTrackingData.ImportRow(selectedRow);
			}
			catch
			{
				// TODO: Linq returns exception if query is null.  Need to refactor?
			}
		}

		private static void BuildManagerTracking()
		{
			try
			{
				DataRow selectedRow;

				selectedRow = (
					from bladeRow in _unfilteredTrackingData.AsEnumerable()
					where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Manager
					select bladeRow).First();

				_filteredTrackingData.ImportRow(selectedRow);
			}
			catch
			{
				// TODO: Linq returns exception if query is null.  Need to refactor?
			}
		}

		private static void BuildTeamApprovers()
		{
			try
			{
				DataRow selectedRow;

				var distinctTeamApprovers = (
					from bladeRow in _unfilteredTrackingData.AsEnumerable()
					where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Team_Approver
					select bladeRow["workflow_actor_name"]).Distinct();

				foreach (string teamApprover in distinctTeamApprovers)
				{
					selectedRow = (
						from bladeRow in _unfilteredTrackingData.AsEnumerable()
						where (string)bladeRow["workflow_actor_name"] == teamApprover
						where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Team_Approver
						select bladeRow).First();

					_filteredTrackingData.ImportRow(selectedRow);
				}
			}
			catch
			{
				// TODO: Linq returns exception if query is null.  Need to refactor?
			}
		}

		private static void BuildTechnicalApprovers()
		{
			try
			{
				DataRow selectedRow;

				var distinctTechnicalApprovers = (
					from bladeRow in _unfilteredTrackingData.AsEnumerable()
					where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Technical_Approver
					select bladeRow["workflow_actor_name"]).Distinct();

				foreach (string technicalApprover in distinctTechnicalApprovers)
				{
					selectedRow = (
						from bladeRow in _unfilteredTrackingData.AsEnumerable()
						where (string)bladeRow["workflow_actor_name"] == technicalApprover
						where (int)bladeRow["actor_group_type"] == (int)ActorGroupType.Technical_Approver
						select bladeRow).First();

					_filteredTrackingData.ImportRow(selectedRow);
				}
			}
			catch
			{
				// TODO: Linq returns exception if query is null.  Need to refactor?
			}
		}

		#endregion

		public static string BuildBladeComments(int WorkflowId)
		{
			DataTable workflowCommentsTable = Database.GetWorkflowComments(WorkflowId);

			if (workflowCommentsTable.Rows.Count > 0)
			{
				StringBuilder workflowComments = new StringBuilder();

				foreach (DataRow comment in workflowCommentsTable.Rows)
				{
					string actionActorName = string.Empty;
					if (comment["action"].ToString() == CommentsType.Email_Reminder.ToString()
						|| comment["action"].ToString() == CommentsType.Requested_Change.ToString())
					{ actionActorName = "AIM"; }
					else { actionActorName = comment["workflow_actor"].ToString(); }

					// TODO: move string to config file?
					workflowComments.AppendFormat("<p{0}><u>{1} by {2} on {3}</u><br />{4}</p>"
						, (bool)comment["is_new"] ? " class=\"csm_error_text\"" : string.Empty
						, Convert.ToString((CommentsType)Enum.Parse(typeof(CommentsType), comment["action"].ToString())).StripUnderscore()
						, actionActorName
						, Convert.ToDateTime(comment["comment_date"]).ToString("MMM d\\, yyyy")
						, comment["comment"].ToString().Replace("\n", "<br />"));
				}

				return workflowComments.ToString();
			}

			return string.Empty;
		}
	}
}
