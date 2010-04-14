using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class MasterRequestBlade : System.Web.UI.UserControl
	{
		public string RequestId { get; set; }
        public RequestState RequestState { get; set; }
		public string AffectedEndUserName { get; set; }
		public string OverallRequestStatus { get; set; }
		public string LastUpdatedDate { get; set; }
		public bool IsSelectedRequest { get; set; }
        public List<AccessGroup> AvailableGroups { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)
		{
			PopulateUserInfo();
			LoadReadOnlyRequestPanel();
			LoadRequestTrackingPanel();

			try
			{
				switch ((Role)SnapSession.CurrentUser.CurrentRole)
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
            readOnlyRequestPanel.RequestState = RequestState;
			_readOnlyRequestPanelContainer.Controls.Add(readOnlyRequestPanel);		
		}
		
		private void LoadRequestTrackingPanel()
		{
			RequestTrackingPanel requestTrackingPanel;

            requestTrackingPanel = LoadControl("~/Controls/RequestTrackingPanel.ascx") as RequestTrackingPanel;
			requestTrackingPanel.RequestId = RequestId.ToString();
            requestTrackingPanel.RequestState = RequestState;
			_requestTrackingPanelContainer.Controls.Add(requestTrackingPanel);		
		}
		
		private void LoadAccessTeamPanels() 
		{

			AcknowledgementPanel acknowledgementPanel;
			WorkflowBuilderPanel workflowBuilderPanel;
			AccessCommentsPanel accessCommentsPanel;

            if (RequestState != RequestState.Closed)
            {
                workflowBuilderPanel = LoadControl("~/Controls/WorkflowBuilderPanel.ascx") as WorkflowBuilderPanel;
                workflowBuilderPanel.RequestId = RequestId.ToString();
                workflowBuilderPanel.RequestState = RequestState;
                workflowBuilderPanel.RequestApprovers = ApprovalWorkflow.GetRequestApprovers(Convert.ToInt32(RequestId));
                workflowBuilderPanel.AvailableGroups = AvailableGroups;

                DataTable managerInfo = new DataTable();
                managerInfo = GetManagerInfo();

                DataRow row = managerInfo.Rows[0];

                Literal managerInfoLit = new Literal();
                managerInfoLit.Text = "<span id=\"_managerDisplayName_" + RequestId + "\" class=\"csm_inline\">" + row[0].ToString() + "</span>";
                managerInfoLit.Text += "<input id=\"_managerId_" + RequestId + "\" type=\"hidden\" value=\"" + row[1].ToString() + "\">";

                PlaceHolder managerInfoSection = new PlaceHolder();
                managerInfoSection = (PlaceHolder)WebUtilities.FindControlRecursive(workflowBuilderPanel, "_managerInfoSection");
                managerInfoSection.Controls.Add(managerInfoLit);

                Literal buttonLit = new Literal();
                buttonLit.Text = "<input type=\"hidden\" id=\"_selectedActors_" + RequestId.ToString() + "\" />" +
                "<input type=\"button\" value=\"Close Completed\" class=\"csm_html_button\"/>" +
                "<input type=\"button\" value=\"Create Ticket\" class=\"csm_html_button\"/>" +
                "<input type=\"button\" value=\"Edit Workflow\" class=\"csm_html_button\"/>" +
                "<input type=\"button\" value=\"Build Workflow | Continue\"" +
                " onclick=\"createWorkflow('" + RequestId.ToString() + "');\" class=\"csm_html_button\"/>";

                PlaceHolder dynamicButtonsContainer;
                dynamicButtonsContainer = (PlaceHolder)WebUtilities.FindControlRecursive(workflowBuilderPanel, "_dynamicButtonsContainer");
                dynamicButtonsContainer.Controls.Add(buttonLit);

                _accessTeamPanelContainer.Controls.Add(workflowBuilderPanel);
            }
			
		}

        private DataTable GetManagerInfo()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ManagerName", typeof(string));
            table.Columns.Add("ManagerID", typeof(string));

            var reqDetails = Common.Request.Details(RequestState);
            var reqDetail = reqDetails.Single(x => x.pkId.ToString() == RequestId);

            table.Rows.Add(reqDetail.managerDisplayName, ApprovalWorkflow.GetActorIdByUserId(ActorGroupType.Manager, reqDetail.managerUserId));

            return table;
        }

		private void LoadApprovingManagerPanel() 
		{
			if (RequestState != RequestState.Closed)
			{
				ApprovingManagerPanel approvingManagerPanel;
				approvingManagerPanel = LoadControl("~/Controls/ApprovingManagerPanel.ascx") as ApprovingManagerPanel;
				approvingManagerPanel.RequestId = RequestId;
				_approvingManagerPanelContainer.Controls.Add(approvingManagerPanel);
			}
			
		}
	}
}