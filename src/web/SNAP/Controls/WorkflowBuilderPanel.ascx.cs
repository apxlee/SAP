using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Apollo.AIM.SNAP.CA;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
    public partial class WorkflowBuilderPanel : System.Web.UI.UserControl
    {
		public string RequestId { get; set; }
        public RequestState RequestState { get; set; }
        public List<AccessApprover> RequestApprovers { get; set; }
        public List<AccessGroup> AvailableGroups { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            List<AccessGroup> UpdatedGroups = new List<AccessGroup>();
            string strGroupType = "";
            //make a copy of available groups
            foreach (AccessGroup group in AvailableGroups)
            {
                UpdatedGroups.Add(group.CreateDeepCopy(group));
            }

            //update groups with selected actors  
            foreach (AccessGroup group in UpdatedGroups)
            {
                bool isOneSelected = false;
                foreach (AccessApprover approver in group.AvailableApprovers)
                {
                    bool approverSelected = RequestApprovers.Exists(a => a.ActorId == approver.ActorId);
                    if (approverSelected) 
                    {
                        isOneSelected = true;
                        approver.IsSelected = true;
                        group.IsSelected = true;
                        switch (approver.WorkflowState)
                        {
                            case WorkflowState.Approved:
                                group.IsDisabled = true;
                                break;
                            case WorkflowState.Closed_Cancelled:
                                group.IsDisabled = true;
                                break;
                            case WorkflowState.Closed_Completed:
                                group.IsDisabled = true;
                                break;
                            case WorkflowState.Closed_Denied:
                                group.IsDisabled = true;
                                break;
                            default:
                                group.IsDisabled = false;
                                break;
                        }
                    }
                }
                if (!isOneSelected)
                {
                    foreach (AccessApprover approver in group.AvailableApprovers)
                    {
                        if (approver.IsDefault){ approver.IsSelected = true;}
                    }
                }
            }

            foreach (AccessGroup group in UpdatedGroups)
            {
                PlaceHolder approverCheckBoxSection = new PlaceHolder();
                PlaceHolder approverGroupMemebers = new PlaceHolder();
                Literal approverGroupLit = new Literal();
                Label approverGroupName = new Label();
                Label approverGroupDescription = new Label();
                WorkflowApprover strSection = new WorkflowApprover();

                WorkflowApprover primaryApprover;
                primaryApprover = LoadControl("~/Controls/WorkflowApprover.ascx") as WorkflowApprover;

                WorkflowApprover secondaryApprover;
                secondaryApprover = LoadControl("~/Controls/WorkflowApprover.ascx") as WorkflowApprover;

                if (group.ActorGroupType == ActorGroupType.Team_Approver) 
                { 
                    strSection = primaryApprover;
                    if (strGroupType != group.ActorGroupType.ToString())
                    {
                        strGroupType = group.ActorGroupType.ToString();
                        Literal primaryLit = new Literal();
                        primaryLit.Text = "<label>Primary Approvers</label>";

                        PlaceHolder primarySectionName;
                        primarySectionName = (PlaceHolder)WebUtilities.FindControlRecursive(primaryApprover, "_approverSectionName");
                        primarySectionName.Controls.Add(primaryLit);
                    }
                }
                else 
                { 
                    strSection = secondaryApprover;
                    if (strGroupType != group.ActorGroupType.ToString())
                    {
                        strGroupType = group.ActorGroupType.ToString();
                        Literal secondaryLit = new Literal();
                        secondaryLit.Text = "<label>Secondary Approvers</label>";

                        PlaceHolder secondarySectionName;
                        secondarySectionName = (PlaceHolder)WebUtilities.FindControlRecursive(secondaryApprover, "_approverSectionName");
                        secondarySectionName.Controls.Add(secondaryLit);
                    }
                }

                approverCheckBoxSection = (PlaceHolder)WebUtilities.FindControlRecursive(strSection, "_approverCheckBoxSection");
                approverGroupName = (Label)WebUtilities.FindControlRecursive(strSection, "_approverGroupName");
                approverGroupDescription = (Label)WebUtilities.FindControlRecursive(strSection, "_approverGroupDescription");
                approverGroupMemebers = (PlaceHolder)WebUtilities.FindControlRecursive(strSection, "_approverGroupMemebers");
                approverGroupName.Text = group.GroupName;
                approverGroupDescription.Text = group.Description;
                
                string strChecked = "";
                if (group.IsSelected) { strChecked = "checked=\"checked\""; }

                string strDisabled = "";
                if (group.IsDisabled) { strDisabled = "disabled=\"disabled\""; }

                approverGroupLit.Text = "<input type=\"checkbox\" ID=\"_approverGroupCheckbox\"" + strDisabled + " " + strChecked + " onclick=\"approverGroupChecked(this,'" + RequestId.ToString() + "');\" class=\"csm_input_checkradio\" />";
                approverCheckBoxSection.Controls.Add(approverGroupLit);

                foreach (AccessApprover approver in group.AvailableApprovers)
                {
                    approverGroupMemebers.Controls.Add(BuildRadioButtons(approver, group, strDisabled));
                }

                _dynamicApproversContainer.Controls.Add(strSection);
            }
        }

        private Literal BuildRadioButtons(AccessApprover approver, AccessGroup group, string strDisabled)
        {
            string strChecked = "";
            string strDisplayName = "";

            if (approver.IsSelected) { strChecked = "checked=\"checked\""; }

            if (approver.IsDefault){strDisplayName = approver.DisplayName + " (Default)";}
            else{strDisplayName = approver.DisplayName + " (Secondary)";}

            Literal litRadioButton = new Literal();
            litRadioButton.Text = "<tr><td>&nbsp;</td><td colspan=\"3\">";
            litRadioButton.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" disabled=\"disabled\" name=\"radio" + RequestId + "_" + group.GroupId + "\" " + strDisabled + " " + strChecked + " class=\"csm_input_checkradio\" />";
            litRadioButton.Text += "<span class=\"csm_inline\">" + strDisplayName + "</span>";
            litRadioButton.Text += "</td></tr>";

            return litRadioButton;
        }
    }
}