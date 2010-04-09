using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
    public partial class WorkflowBuilderPanel : System.Web.UI.UserControl
    {
		public string RequestId { get; set; }
        public RequestState RequestState { get; set; }
        public List<AccessApprover> RequestApprovers { get; set; }
        public List<AccessApprover> AvailableApprovers { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            //TODO Manager Name for the Request
			_requiredApproverFullName.Text = "Greg Belanger";

            WorkflowApprover primaryApprover;
            primaryApprover = LoadControl("~/Controls/WorkflowApprover.ascx") as WorkflowApprover;

            Literal primaryLit = new Literal();
            primaryLit.Text = "<label>Primary Approvers</label>";

            PlaceHolder primarySectionName;
            primarySectionName = (PlaceHolder)WebUtilities.FindControlRecursive(primaryApprover, "_approverSectionName");
            primarySectionName.Controls.Add(primaryLit);

            WorkflowApprover secondaryApprover;
            secondaryApprover = LoadControl("~/Controls/WorkflowApprover.ascx") as WorkflowApprover;

            Literal secondaryLit = new Literal();
            secondaryLit.Text = "<label>Secondary Approvers</label>";

            PlaceHolder secondarySectionName;
            secondarySectionName = (PlaceHolder)WebUtilities.FindControlRecursive(secondaryApprover, "_approverSectionName");
            secondarySectionName.Controls.Add(secondaryLit);

            int groupId = 0;

            foreach(AccessApprover approver in AvailableApprovers)
            {
                PlaceHolder approverCheckBoxSection = new PlaceHolder();
                PlaceHolder approverGroupMemebers = new PlaceHolder();
                Literal approverGroupLit = new Literal();
                Label approverGroupName = new Label();
                Label approverGroupDescription = new Label();
                WorkflowApprover strSection = new WorkflowApprover();
                bool isSelected = RequestApprovers.Exists(a => a.ActorId == approver.ActorId);

                switch (approver.ActorApprovalType)
                {
                    case ActorApprovalType.Team_Approver:
                        strSection = primaryApprover;
                        break;
                    case ActorApprovalType.Technical_Approver:
                        strSection = secondaryApprover;
                        break;
                }
                if (groupId != approver.GroupId)
                {
                    approverCheckBoxSection = (PlaceHolder)WebUtilities.FindControlRecursive(strSection, "_approverCheckBoxSection");
                    approverGroupName = (Label)WebUtilities.FindControlRecursive(strSection, "_approverGroupName");
                    approverGroupDescription = (Label)WebUtilities.FindControlRecursive(strSection, "_approverGroupDescription");
                    approverGroupMemebers = (PlaceHolder)WebUtilities.FindControlRecursive(strSection, "_approverGroupMemebers");
                    groupId = approver.GroupId;
                    approverGroupName.Text = approver.GroupName;
                    approverGroupDescription.Text = approver.Description;
                    approverGroupMemebers.Controls.Add(BuildRadioButtons(approver, isSelected));
                }
                else
                {
                    approverGroupMemebers = (PlaceHolder)WebUtilities.FindControlRecursive(strSection, "_approverGroupMemebers");
                    approverGroupMemebers.Controls.Add(BuildRadioButtons(approver, isSelected));
                }

                if (isSelected) { approverGroupLit.Text = "<input type=\"checkbox\" ID=\"_approverGroupCheckbox\" checked=\"checked\" onclick=\"approverGroupChecked(this,'" + RequestId.ToString() + "');\" class=\"csm_input_checkradio\" />"; }
                else { approverGroupLit.Text = "<input type=\"checkbox\" ID=\"_approverGroupCheckbox\" onclick=\"approverGroupChecked(this,'" + RequestId.ToString() + "');\" class=\"csm_input_checkradio\" />"; }
                approverCheckBoxSection.Controls.Add(approverGroupLit);
                _dynamicApproversContainer.Controls.Add(primaryApprover);
                _dynamicApproversContainer.Controls.Add(secondaryApprover);
            }
        }

        private Literal BuildRadioButtons(AccessApprover approver, bool isSelected)
        {
            Literal litRadioButton = new Literal();

            litRadioButton.Text = "<tr><td>&nbsp;</td><td colspan=\"3\">";

            if (isSelected)
            {
                if (approver.IsDefault)
                {
                    litRadioButton.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" checked=\"checked\" class=\"csm_input_checkradio\" />";
                    litRadioButton.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Default)</span>";
                }
                else
                {
                    litRadioButton.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" checked=\"checked\" class=\"csm_input_checkradio\" />";
                    litRadioButton.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Secondary)</span>";
                }
            }
            else
            {
                if (approver.IsDefault)
                {
                    litRadioButton.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" checked=\"checked\" class=\"csm_input_checkradio\" />";
                    litRadioButton.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Default)</span>";
                }
                else
                {
                    litRadioButton.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" class=\"csm_input_checkradio\" />";
                    litRadioButton.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Secondary)</span>";
                }
            }
            litRadioButton.Text += "</td></tr>";

            return litRadioButton;
        }
    }
}