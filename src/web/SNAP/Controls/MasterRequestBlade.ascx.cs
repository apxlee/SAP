using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class MasterRequestBlade : System.Web.UI.UserControl
	{
		public string RequestId { get; set; }
		public string AffectedEndUserName { get; set; }
		public string OverallRequestStatus { get; set; }
		public string LastUpdatedDate { get; set; }
		public bool IsSelectedRequest { get; set; }
		
		private Role _testRole = Role.AccessTeam;
	
		protected void Page_Load(object sender, EventArgs e)
		{
			PopulateUserInfo();
			LoadReadOnlyRequestPanel();
			LoadRequestTrackingPanel();

			try
			{
				switch ((Role)Page.Session["SNAPUserRole"])
				{
					case Role.ApprovingManager:
						LoadApprovingManagerPanel();
						break;

					case Role.AccessTeam:
						LoadAccessTeamPanels();
						break;

					case Role.SuperUser:
						LoadApprovingManagerPanel();
						LoadAccessTeamPanels();
						break;

					default:
						break;
				}
			}
			catch (Exception ex)
			{
				// TODO: if session not set, redirect to login?  throw message to user?  set role to user?
			}

		}
		
		private void PopulateUserInfo()
		{
			_affectedEndUserName.Text = AffectedEndUserName;
			_overallRequestStatus.Text = OverallRequestStatus;
			_lastUpdatedDate.Text = LastUpdatedDate;
			_requestId.Text = RequestId;
			
			if (IsSelectedRequest)
			{
				_toggleIconContainer.CssClass = "csm_toggle_icon_up";
				_toggledContentContainer.CssClass = "csm_displayed_block";
			}
		}
		
		private void LoadReadOnlyRequestPanel()
		{
			ReadOnlyRequestPanel readOnlyRequestPanel;

			readOnlyRequestPanel = LoadControl("~/Controls/ReadOnlyRequestPanel.ascx") as ReadOnlyRequestPanel;
			readOnlyRequestPanel.RequestId = RequestId.ToString();
			_readOnlyRequestPanelContainer.Controls.Add(readOnlyRequestPanel);		
		}
		
		private void LoadRequestTrackingPanel()
		{
			RequestTrackingPanel requestTrackingPanel;

            requestTrackingPanel = LoadControl("~/Controls/RequestTrackingPanel.ascx") as RequestTrackingPanel;
			requestTrackingPanel.RequestId = RequestId.ToString();
			_requestTrackingPanelContainer.Controls.Add(requestTrackingPanel);		
		}
		
		private void LoadAccessTeamPanels() 
		{
			AcknowledgementPanel acknowledgementPanel;
			WorkflowBuilderPanel workflowBuilderPanel;
			AccessCommentsPanel accessCommentsPanel;

            workflowBuilderPanel = LoadControl("~/Controls/WorkflowBuilderPanel.ascx") as WorkflowBuilderPanel;
			workflowBuilderPanel.RequestId = RequestId.ToString();
			_accessTeamPanelContainer.Controls.Add(workflowBuilderPanel);
			
		}
		
		private void LoadApprovingManagerPanel() {}
	}
}