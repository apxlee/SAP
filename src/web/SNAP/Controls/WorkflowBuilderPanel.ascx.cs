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
        public WorkflowState AccessTeamState { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            bool builderDisabled = AccessTeamState != WorkflowState.Pending_Workflow ? true : false;
            bool isOneSelected = false;
            bool approverSelected;
            List<AccessGroup> UpdatedGroups = new List<AccessGroup>();
            List<AccessGroup> UpdatedLargeGroups = new List<AccessGroup>();
            string strGroupType = "";

            //make a copy of available groups
            foreach (AccessGroup group in AvailableGroups)
            {
                if (group.IsLargeGroup){ UpdatedLargeGroups.Add(group.CreateDeepCopy(group)); }
                else { UpdatedGroups.Add(group.CreateDeepCopy(group)); }
            }

            #region Normal Groups
            //update groups with selected actors  
            
            foreach (AccessGroup group in UpdatedGroups)
            {
                
                foreach (AccessApprover approver in group.AvailableApprovers)
                {
                    approverSelected = RequestApprovers.Exists(a => a.ActorId == approver.ActorId);
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
                        primaryLit.Text = "<label>Team Approvers</label>";

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
                        secondaryLit.Text = "<label>Technical Approvers</label>";

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
                if (group.IsDisabled || builderDisabled) { strDisabled = "disabled=\"disabled\""; }

                approverGroupLit.Text = "<input type=\"checkbox\" ID=\"_approverGroupCheckbox\"" + strDisabled + " " + strChecked + " onclick=\"approverGroupChecked(this,'" + RequestId.ToString() + "');\" class=\"csm_input_checkradio\" />";
                approverCheckBoxSection.Controls.Add(approverGroupLit);

                foreach (AccessApprover approver in group.AvailableApprovers)
                {
                    approverGroupMemebers.Controls.Add(BuildRadioButtons(approver, group, strDisabled));
                }

                _dynamicApproversContainer.Controls.Add(strSection);
            }
            #endregion

            #region Large Groups

            foreach (AccessGroup group in UpdatedLargeGroups)
            {
                DataTable approverTable = new DataTable();
                approverTable.Columns.Add("DisplayName", typeof(string));
                approverTable.Columns.Add("ActorId", typeof(string));
                
                WorkflowLargeGroup largeGroup = new WorkflowLargeGroup();
                largeGroup = LoadControl("~/Controls/WorkflowLargeGroup.ascx") as WorkflowLargeGroup;

                Literal largeGroupNameLit = new Literal();
                largeGroupNameLit.Text = "<label>" + group.GroupName + "</label>";

                PlaceHolder largeGroupSectionName;
                largeGroupSectionName = (PlaceHolder)WebUtilities.FindControlRecursive(largeGroup, "_largeGroupSectionName");
                largeGroupSectionName.Controls.Add(largeGroupNameLit);

                DropDownList dropdownActors;
                dropdownActors = (DropDownList)WebUtilities.FindControlRecursive(largeGroup, "_dropdownActors");
                dropdownActors.Attributes.Add("onchange", "ActorSelected(this);");
                if (builderDisabled) { dropdownActors.Attributes.Add("disabled", "disabled"); ;}

                foreach (AccessApprover approver in group.AvailableApprovers)
                {
                    approverSelected = RequestApprovers.Exists(a => a.ActorId == approver.ActorId);
                    if (approverSelected)
                    {
                        //add approver to selected approvers
                        approverTable.Rows.Add(approver.DisplayName, approver.ActorId);
                    }
                    else
                    {
                        //add approver to approver selection drop down
                        ListItem approverItem = new ListItem();
                        approverItem.Text = approver.DisplayName;
                        approverItem.Value = approver.ActorId.ToString();
                        dropdownActors.Items.Add(approverItem);
                    }
                }

                HtmlInputControl actorDisplayName;
                actorDisplayName = (HtmlInputControl)WebUtilities.FindControlRecursive(largeGroup, "_actorDisplayName");
                actorDisplayName.Attributes.Add("onkeyup", "ActorChanged(this);");
                if (builderDisabled) { actorDisplayName.Attributes.Add("disabled", "disabled"); ;}

                HtmlInputHidden actorGroupId;
                actorGroupId = (HtmlInputHidden)WebUtilities.FindControlRecursive(largeGroup, "_actorGroupId");
                actorGroupId.Value = group.GroupId.ToString();

                HtmlInputButton checkActor;
                checkActor = (HtmlInputButton)WebUtilities.FindControlRecursive(largeGroup, "_checkActor");
                checkActor.Attributes.Add("onclick", "ActorCheck(this);");
                if (builderDisabled) { checkActor.Attributes.Add("disabled", "disabled"); ;}

                HtmlInputButton addActor;
                addActor = (HtmlInputButton)WebUtilities.FindControlRecursive(largeGroup, "_addActor");
                addActor.Attributes.Add("onclick", "ActorAdd(this,'" + RequestId.ToString() + "');");
                if (builderDisabled) { addActor.Attributes.Add("disabled", "disabled"); ;}

                if (approverTable.Rows.Count > 0)
                {
                    ListView actorListView;
                    actorListView = (ListView)WebUtilities.FindControlRecursive(largeGroup, "_actorListView");
                    
                    Literal largeGroupListItemLit = new Literal();
                    largeGroupListItemLit.Text = "<table class='listview_table'>";

                    string strDisabled = builderDisabled ? "disabled='disabled'" : "";

                    foreach(DataRow row in approverTable.Rows)
                    {
                        largeGroupListItemLit.Text += "<tr class='listview_tr'>";
                        largeGroupListItemLit.Text += "<td class='listview_td'>" + row[0].ToString() + "</td>";
                        largeGroupListItemLit.Text += "<td style='display:none;'>" + row[1].ToString() + "</td>";
                        largeGroupListItemLit.Text += "<td class='listview_button'><input type='button' " + strDisabled + " onclick='RemoveActor(this);' value='Remove' class='csm_html_button'/></td>";
                        largeGroupListItemLit.Text += "</tr>";
                    }

                    largeGroupListItemLit.Text += "</table>";

                    PlaceHolder largeGroupSelectedList;
                    largeGroupSelectedList = (PlaceHolder)WebUtilities.FindControlRecursive(largeGroup, "_largeGroupSelectedList");
                    largeGroupSelectedList.Controls.Add(largeGroupListItemLit);
                }
                                
                _dynamicLargeGroupContainer.Controls.Add(largeGroup);
            }

            #endregion
        }

        private Literal BuildRadioButtons(AccessApprover approver, AccessGroup group, string strDisabled)
        {
            string strChecked = "";
            string strDisplayName = "";

            if (approver.IsSelected) { strChecked = "checked=\"checked\""; }

            if (approver.IsDefault){strDisplayName = approver.DisplayName + " (Default)";}
            else{strDisplayName = approver.DisplayName;}

            Literal litRadioButton = new Literal();
            litRadioButton.Text = "<tr><td>&nbsp;</td><td colspan=\"3\">";
            litRadioButton.Text += "<input type=\"radio\" id=\"" + approver.ActorId + "\" disabled=\"disabled\" name=\"radio" + RequestId + "_" + group.GroupId + "\" " + strDisabled + " " + strChecked + " class=\"csm_input_checkradio\" />";
            litRadioButton.Text += "<span class=\"csm_inline\">" + strDisplayName + "</span>";
            litRadioButton.Text += "</td></tr>";

            return litRadioButton;
        }
    }
}