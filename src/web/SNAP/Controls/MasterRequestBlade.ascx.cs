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
        public bool IsLastRequest { get; set; }
        public List<AccessGroup> AvailableGroups { get; set; }
        private WorkflowState AccessTeamState;
        private static int AccessTeamActorId = 1;
		
		protected void Page_Load(object sender, EventArgs e)
		{
		    try
		    {
			    PopulateUserInfo();
			    LoadReadOnlyRequestPanel();
			    LoadRequestTrackingPanel();

			    switch (Page.GetType().Name.StripUnderscoreAndExtension().ToLower())
			    {
				    case "myapprovals" :
					    LoadApprovingManagerPanel();
					    break;
                    
				    case "accessteam" :
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
				//_toggleIconContainer.CssClass = "csm_toggle_icon_up";
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
                //get the access team state.  this for building the workflow builder buttons. 
                AccessTeamState = (WorkflowState)ApprovalWorkflow.GetWorkflowState(ApprovalWorkflow.GetWorkflowId(AccessTeamActorId, Convert.ToInt32(RequestId)));

                acknowledgementPanel = LoadControl("~/Controls/AcknowledgementPanel.ascx") as AcknowledgementPanel;
                acknowledgementPanel.RequestId = RequestId.ToString();

                accessCommentsPanel = LoadControl("~/Controls/AccessCommentsPanel.ascx") as AccessCommentsPanel;
                accessCommentsPanel.RequestId = RequestId.ToString();

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
                managerInfoLit.Text += "<input id=\"_managerUserId_" + RequestId + "\" type=\"hidden\" value=\"" + row[1].ToString() + "\">";

                PlaceHolder managerInfoSection = new PlaceHolder();
                managerInfoSection = (PlaceHolder)WebUtilities.FindControlRecursive(workflowBuilderPanel, "_managerInfoSection");
                managerInfoSection.Controls.Add(managerInfoLit);

                Literal buttonLit = new Literal();
                buttonLit.Text = "<input type=\"hidden\" id=\"_selectedActors_" + RequestId.ToString() + "\" />";

                switch (AccessTeamState)
                {
                    case WorkflowState.Pending_Acknowledgement:
                        buttonLit.Text += BuildButtons("closed_cancelled_" + RequestId.ToString(), "Closed Cancelled", "actionClicked(this,'" + RequestId.ToString() + "','3');", true);
                        buttonLit.Text += BuildButtons("create_workflow_" + RequestId.ToString(), "Create Workflow", "actionClicked(this,'" + RequestId.ToString() + "','');", true);
                        break;
                    case WorkflowState.Pending_Workflow:
                        buttonLit.Text += BuildButtons("closed_cancelled_" + RequestId.ToString(), "Closed Cancelled", "actionClicked(this,'" + RequestId.ToString() + "','3');", false);
                        buttonLit.Text += BuildButtons("create_workflow_" + RequestId.ToString(), "Create Workflow", "actionClicked(this,'" + RequestId.ToString() + "','');", false);
                        break;
                    case WorkflowState.Workflow_Created:
                        buttonLit.Text += BuildButtons("closed_cancelled_" + RequestId.ToString(), "Closed Cancelled", "actionClicked(this,'" + RequestId.ToString() + "','3');", false);
                        buttonLit.Text += BuildButtons("edit_workflow_" + RequestId.ToString(), "Edit Workflow", "actionClicked(this,'" + RequestId.ToString() + "','');", false);
                        buttonLit.Text += BuildButtons("continue_workflow_" + RequestId.ToString(), "Continue Workflow", "actionClicked(this,'" + RequestId.ToString() + "','');", true);
                        break;
                    case WorkflowState.Approved:
                        buttonLit.Text += BuildButtons("closed_cancelled_" + RequestId.ToString(), "Closed Cancelled", "actionClicked(this,'" + RequestId.ToString() + "','3');", false);
                        buttonLit.Text += BuildButtons("closed_completed_" + RequestId.ToString(), "Closed Completed", "actionClicked(this,'" + RequestId.ToString() + "','6');", true);
                        buttonLit.Text += BuildButtons("create_ticket_" + RequestId.ToString(), "Create Ticket", "actionClicked(this,'" + RequestId.ToString() + "','5');", false);
                        break;
                    case WorkflowState.Pending_Provisioning:
                        buttonLit.Text += BuildButtons("closed_cancelled_" + RequestId.ToString(), "Closed Cancelled", "actionClicked(this,'" + RequestId.ToString() + "','3');", false);
                        buttonLit.Text += BuildButtons("closed_completed_" + RequestId.ToString(), "Closed Completed", "actionClicked(this,'" + RequestId.ToString() + "','6');", false);
                        break;
                }

                /*
                buttonLit.Text += BuildButtons("closed_cancelled_" + RequestId.ToString(), "Closed Cancelled", "actionClicked(this,'" + RequestId.ToString() + "','3');", false);
                buttonLit.Text += BuildButtons("closed_completed_" + RequestId.ToString(), "Closed Completed", "actionClicked(this,'" + RequestId.ToString() + "','6');", false);
                buttonLit.Text += BuildButtons("create_ticket_" + RequestId.ToString(), "Create Ticket", "actionClicked(this,'" + RequestId.ToString() + "','5');", false);
                buttonLit.Text += BuildButtons("edit_workflow_" + RequestId.ToString(), "Edit Workflow", "actionClicked(this,'" + RequestId.ToString() + "','');", false);
                buttonLit.Text += BuildButtons("create_workflow_" + RequestId.ToString(), "Create Workflow", "actionClicked(this,'" + RequestId.ToString() + "','');", false);
                buttonLit.Text += BuildButtons("continue_workflow_" + RequestId.ToString(), "Continue Workflow", "actionClicked(this,'" + RequestId.ToString() + "','');", false);
                */      

                PlaceHolder dynamicButtonsContainer;
                dynamicButtonsContainer = (PlaceHolder)WebUtilities.FindControlRecursive(workflowBuilderPanel, "_dynamicButtonsContainer");
                dynamicButtonsContainer.Controls.Add(buttonLit);

                _accessTeamPanelContainer.Controls.Add(acknowledgementPanel);
                _accessTeamPanelContainer.Controls.Add(workflowBuilderPanel);
                _accessTeamPanelContainer.Controls.Add(accessCommentsPanel);
                
            }
			
		}

        private string BuildButtons(string buttonId, string buttonName, string buttonFunction, bool isDisabled)
        {
            string strButton = "";
            string strDisabled = "";
                if(isDisabled){ strDisabled = "disabled=\"disabled\""; }
            strButton = "<input type=\"button\" id=\"" + buttonId + "\" " + strDisabled + " value=\"" + buttonName +
                "\" onclick=\"" + buttonFunction + "\" class=\"csm_html_button\"/>";

            return strButton;
        }

        private DataTable GetManagerInfo()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ManagerName", typeof(string));
            table.Columns.Add("ManagerUserID", typeof(string));
            table.Columns.Add("ManagerActorID", typeof(string));

            var reqDetails = Common.Request.Details(RequestState);
            var reqDetail = reqDetails.Single(x => x.pkId.ToString() == RequestId);

            table.Rows.Add(reqDetail.managerDisplayName, reqDetail.managerUserId, ApprovalWorkflow.GetActorIdByUserId(ActorGroupType.Manager, reqDetail.managerUserId));

            return table;
        }

		private void LoadApprovingManagerPanel() 
		{
			if (RequestState != RequestState.Closed)
			{
				ApprovingManagerPanel approvingManagerPanel;
				approvingManagerPanel = LoadControl("~/Controls/ApprovingManagerPanel.ascx") as ApprovingManagerPanel;
				approvingManagerPanel.RequestId = RequestId;
                approvingManagerPanel.IsLastRequest = IsLastRequest;
				_approvingManagerPanelContainer.Controls.Add(approvingManagerPanel);
			}
			
		}
	}
}