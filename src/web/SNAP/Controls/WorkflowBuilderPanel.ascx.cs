using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Apollo.AIM.SNAP.Web.Common;
namespace Apollo.AIM.SNAP.Web.Controls
{
    public partial class WorkflowBuilderPanel : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadRequiredApprover();
            LoadApprovers();
        }

        private void LoadRequiredApprover()
        {
            WorkflowApprover workflowApprover;
            workflowApprover = LoadControl(@"/Controls/WorkflowApprover.ascx") as WorkflowApprover;

            HtmlInputCheckBox approverCheck;
            approverCheck = (HtmlInputCheckBox)WebUtilities.FindControlRecursive(workflowApprover, "_approverCheck");
            approverCheck.Checked = true;
            approverCheck.Disabled = true;

            HtmlContainerControl approverTitle;
            approverTitle = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_approverTitle");
            approverTitle.InnerHtml = "Manager - Greg Belanger";

            HtmlContainerControl deleteApprover;
            deleteApprover = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_deleteApprover");
            //deleteApprover.Visible = false;

            HtmlContainerControl editApprover;
            editApprover = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_editApprover");
            editApprover.Attributes.Add("onclick", "alert('Throw Modal');");

            this._requiredApprover.Controls.Add(workflowApprover);
        }

        private void LoadApprovers()
        {
            DataTable teamData = GetTable("Team Groups");
            DataTable technicalData = GetTable("Technical Groups");

            WorkflowApprover workflowApprover;
            workflowApprover = LoadControl(@"/Controls/WorkflowApprover.ascx") as WorkflowApprover;

            string groupName = "";
            string description = "";
            bool alternating = false;

            foreach (DataRow team in teamData.Rows)
            {
                groupName = team["groupName"].ToString();
                description = team["description"].ToString();

                DataTable approverData = GetTable(groupName);
                Literal litApprover = new Literal();
                foreach (DataRow approver in approverData.Rows)
                {
                    litApprover.Text += "<tr><td>&nbsp;</td><td colspan=\"3\">";
                        if ((bool)approver["isDefault"] == true)
                        {
                            litApprover.Text += "<input type=\"radio\" checked=\"checked\" class=\"csm_input_checkradio\" />";
                            litApprover.Text += "<span class=\"csm_inline\">" + approver["displayName"].ToString() + "(Default)</span>";
                        }
                        else
                        {
                            litApprover.Text += "<input type=\"radio\" class=\"csm_input_checkradio\" />";
                            litApprover.Text += "<span class=\"csm_inline\">" + approver["displayName"].ToString() + "(Secondary)</span>";
                        }
                    litApprover.Text += "</td></tr>";
                }

                //Panel approverContainer;
                //approverContainer = (Panel)WebUtilities.FindControlRecursive(workflowApprover, "_approverContainer");
                
                //if (alternating)
                //{
                //    approverContainer.CssClass = "oospa_workflow_builder_row csm_alternating_bg";
                //    alternating = false;
                //}
                //else
                //{
                //    approverContainer.CssClass = "oospa_workflow_builder_row";
                //    alternating = true;
                //}

                HtmlInputCheckBox approverCheck;
                approverCheck = (HtmlInputCheckBox)WebUtilities.FindControlRecursive(workflowApprover, "_approverCheck");
                approverCheck.Checked = false;

                HtmlContainerControl approverTitle;
                approverTitle = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_approverTitle");
                approverTitle.InnerHtml = groupName;

                HtmlContainerControl approverDescription;
                approverDescription = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_approverDescription");
                approverDescription.InnerHtml = description;

                HtmlContainerControl deleteApprover;
                deleteApprover = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_deleteApprover");
                deleteApprover.Attributes.Add("onclick", "alert('Throw Modal');");

                HtmlContainerControl editApprover;
                editApprover = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_editApprover");
                editApprover.Attributes.Add("onclick", "alert('Throw Modal');");
                
                PlaceHolder defaultApprover;
                defaultApprover = (PlaceHolder)WebUtilities.FindControlRecursive(workflowApprover, "_defaultApprover");
                defaultApprover.Controls.Add(litApprover);

                this._teamApprover.Controls.Add(workflowApprover);
            }

            Literal teamButton = new Literal();
            teamButton.Text = "<hr />" +
                              "<div class=\"csm_input_buttons_container\">" +
                              "<input type=\"button\" value=\"Add New Primary Group\" class=\"csm_html_button\" onclick=\"throwModifyGroupModal();\"/>" +
                              "</div>";
            this._teamButton.Controls.Add(teamButton);

            foreach (DataRow technical in technicalData.Rows)
            {
                groupName = technical["groupName"].ToString();
                description = technical["description"].ToString();
                workflowApprover = new WorkflowApprover();
                workflowApprover = LoadControl(@"/Controls/WorkflowApprover.ascx") as WorkflowApprover;
                DataTable approverData = GetTable(groupName);
                Literal litApprover = new Literal();
                foreach (DataRow approver in approverData.Rows)
                {
                    litApprover.Text += "<tr><td>&nbsp;</td><td colspan=\"3\">";
                    if ((bool)approver["isDefault"] == true)
                    {
                        litApprover.Text += "<input type=\"radio\" checked=\"checked\" class=\"csm_input_checkradio\" />";
                        litApprover.Text += "<span class=\"csm_inline\">" + approver["displayName"].ToString() + "(Default)</span>";
                    }
                    else
                    {
                        litApprover.Text += "<input type=\"radio\" class=\"csm_input_checkradio\" />";
                        litApprover.Text += "<span class=\"csm_inline\">" + approver["displayName"].ToString() + "(Secondary)</span>";
                    }
                    litApprover.Text += "</td></tr>";
                }

                HtmlInputCheckBox approverCheck;
                approverCheck = (HtmlInputCheckBox)WebUtilities.FindControlRecursive(workflowApprover, "_approverCheck");
                approverCheck.Checked = false;

                HtmlContainerControl approverTitle;
                approverTitle = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_approverTitle");
                approverTitle.InnerHtml = groupName;

                HtmlContainerControl approverDescription;
                approverDescription = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_approverDescription");
                approverDescription.InnerHtml = description;

                HtmlContainerControl deleteApprover;
                deleteApprover = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_deleteApprover");
                deleteApprover.Attributes.Add("onclick", "alert('Throw Modal');");

                HtmlContainerControl editApprover;
                editApprover = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_editApprover");
                editApprover.Attributes.Add("onclick", "alert('Throw Modal');");

                PlaceHolder defaultApprover;
                defaultApprover = (PlaceHolder)WebUtilities.FindControlRecursive(workflowApprover, "_defaultApprover");
                defaultApprover.Controls.Add(litApprover);

                this._technicalApprover.Controls.Add(workflowApprover);
            }

            Literal technicalButton = new Literal();
            technicalButton.Text = "<hr />" +
                              "<div class=\"csm_input_buttons_container\">" +
                              "<input type=\"button\" value=\"Add New Primary Group\" class=\"csm_html_button\" onclick=\"throwModifyGroupModal();\"/>" +
                              "</div>";
            this._technicalButton.Controls.Add(technicalButton);

        }

        static DataTable GetTable(string dataCollection)
        {
            
            DataTable groupTable = new DataTable();
            groupTable.Columns.Add("groupName", typeof(string));
            groupTable.Columns.Add("description", typeof(string));
            groupTable.Columns.Add("typeName", typeof(string));

            DataTable approverTable = new DataTable();
            approverTable.Columns.Add("groupName", typeof(string));
            approverTable.Columns.Add("displayName", typeof(string));
            approverTable.Columns.Add("isDefault", typeof(bool));

            switch(dataCollection)
            {
                case "Team Groups":
                    groupTable.Rows.Add("Software Group", "The Software Group includes all the people who make the softwares.", "Team Approver");
                    return groupTable;
                case "Technical Groups":
                    groupTable.Rows.Add("UnixLinux", "Technical Group Description", "Technical Approver");
                    groupTable.Rows.Add("Windows", "Exchange, Windows Network Shares, Collaboration", "Technical Approver");
                    return groupTable;
                case "Software Group":
                    approverTable.Rows.Add("Software Group", "Pong Lee", false);
                    approverTable.Rows.Add("Software Group", "Mark Garrigan", true);
                    return approverTable;
                case "UnixLinux":
                    approverTable.Rows.Add("UnixLinux", "Pong Lee", true);
                    approverTable.Rows.Add("UnixLinux", "Chris Schwimmer", false);
                    return approverTable;
                case "Windows":
                    approverTable.Rows.Add("Windows", "Jonathan Steele", true);
                    approverTable.Rows.Add("Windows", "Chris Schwimmer", false);
                    return approverTable;
                default:
                    return null;
            }
        }
    }
}