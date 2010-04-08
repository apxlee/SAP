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
        public List<AccessApprover> AvailableApprovers { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
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

            PlaceHolder approverCheckBoxSection;
            Label approverGroupName;
            Label approverGroupDescription;
            int groupId = 0;

            foreach(AccessApprover approver in AvailableApprovers)
            {
                switch (approver.ActorApprovalType)
                {
                    case ActorApprovalType.Team_Approver:
                        if (groupId != approver.GroupId)
                        {
                            Literal approverGroupLit = new Literal();
                            approverGroupLit.Text = "<input type=\"checkbox\" ID=\"_approverGroupCheckbox\" onclick=\"approverGroupChecked(this,'" + RequestId.ToString() + "');\" class=\"csm_input_checkradio\" />";

                            approverCheckBoxSection = (PlaceHolder)WebUtilities.FindControlRecursive(primaryApprover, "_approverCheckBoxSection");
                            approverCheckBoxSection.Controls.Add(approverGroupLit);

                            approverGroupName = (Label)WebUtilities.FindControlRecursive(primaryApprover, "_approverGroupName");
                            approverGroupName.Text = approver.GroupName;

                            approverGroupDescription = (Label)WebUtilities.FindControlRecursive(primaryApprover, "_approverGroupDescription");
                            approverGroupDescription.Text = approver.Description;

                            PlaceHolder approverGroupMemebers;
                            approverGroupMemebers = (PlaceHolder)WebUtilities.FindControlRecursive(primaryApprover, "_approverGroupMemebers");

                            Literal selectMember = new Literal();

                            selectMember.Text = "<tr><td>&nbsp;</td><td colspan=\"3\">";
                            if (approver.IsDefault)
                            {
                                selectMember.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" checked=\"checked\" class=\"csm_input_checkradio\" />";
                                selectMember.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Default)</span>";
                            }
                            else
                            {
                                selectMember.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" class=\"csm_input_checkradio\" />";
                                selectMember.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Secondary)</span>";
                            }
                            selectMember.Text += "</td></tr>";

                            approverGroupMemebers.Controls.Add(selectMember);

                            groupId = approver.GroupId;
                        }
                        else
                        {
                            PlaceHolder approverGroupMemebers;
                            approverGroupMemebers = (PlaceHolder)WebUtilities.FindControlRecursive(primaryApprover, "_approverGroupMemebers");

                            Literal selectMember = new Literal();

                            selectMember.Text = "<tr><td>&nbsp;</td><td colspan=\"3\">";
                            if (approver.IsDefault)
                            {
                                selectMember.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" checked=\"checked\" class=\"csm_input_checkradio\" />";
                                selectMember.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Default)</span>";
                            }
                            else
                            {
                                selectMember.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" class=\"csm_input_checkradio\" />";
                                selectMember.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Secondary)</span>";
                            }
                            selectMember.Text += "</td></tr>";

                            approverGroupMemebers.Controls.Add(selectMember);
                        }
                        break;
                    case ActorApprovalType.Technical_Approver:
                        if (groupId != approver.GroupId)
                        {
                            Literal approverGroupLit = new Literal();
                            approverGroupLit.Text = "<input type=\"checkbox\" ID=\"_approverGroupCheckbox\" onclick=\"approverGroupChecked(this,'" + RequestId.ToString() + "');\" class=\"csm_input_checkradio\" />";

                            approverCheckBoxSection = (PlaceHolder)WebUtilities.FindControlRecursive(secondaryApprover, "_approverCheckBoxSection");
                            approverCheckBoxSection.Controls.Add(approverGroupLit);

                            approverGroupName = (Label)WebUtilities.FindControlRecursive(secondaryApprover, "_approverGroupName");
                            approverGroupName.Text = approver.GroupName;

                            approverGroupDescription = (Label)WebUtilities.FindControlRecursive(secondaryApprover, "_approverGroupDescription");
                            approverGroupDescription.Text = approver.Description;
                            
                            PlaceHolder approverGroupMemebers;
                            approverGroupMemebers = (PlaceHolder)WebUtilities.FindControlRecursive(secondaryApprover, "_approverGroupMemebers");

                            Literal selectMember = new Literal();

                            selectMember.Text = "<tr><td>&nbsp;</td><td colspan=\"3\">";
                            if (approver.IsDefault)
                            {
                                selectMember.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" checked=\"checked\" class=\"csm_input_checkradio\" />";
                                selectMember.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Default)</span>";
                            }
                            else
                            {
                                selectMember.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" class=\"csm_input_checkradio\" />";
                                selectMember.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Secondary)</span>";
                            }
                            selectMember.Text += "</td></tr>";

                            approverGroupMemebers.Controls.Add(selectMember);

                            groupId = approver.GroupId;
                        }
                        else
                        {
                            PlaceHolder approverGroupMemebers;
                            approverGroupMemebers = (PlaceHolder)WebUtilities.FindControlRecursive(secondaryApprover, "_approverGroupMemebers");

                            Literal selectMember = new Literal();

                            selectMember.Text = "<tr><td>&nbsp;</td><td colspan=\"3\">";
                            if (approver.IsDefault)
                            {
                                selectMember.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" checked=\"checked\" class=\"csm_input_checkradio\" />";
                                selectMember.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Default)</span>";
                            }
                            else
                            {
                                selectMember.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" name=\"radio" + RequestId + "_" + approver.GroupId + "\" class=\"csm_input_checkradio\" />";
                                selectMember.Text += "<span class=\"csm_inline\">" + approver.DisplayName + " (Secondary)</span>";
                            }
                            selectMember.Text += "</td></tr>";

                            approverGroupMemebers.Controls.Add(selectMember);
                        }
                    break;     
                }

                _dynamicApproversContainer.Controls.Add(primaryApprover);
                _dynamicApproversContainer.Controls.Add(secondaryApprover);
            }
        }
    }
}