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
            deleteApprover.Visible = false;

            HtmlContainerControl editApprover;
            editApprover = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_editApprover");
            editApprover.Attributes.Add("onclick", "alert('Throw Modal');");

            this._requiredApprover.Controls.Add(workflowApprover);
        }

        private void LoadApprovers()
        {
            DataTable testData = GetTable();
            PlaceHolder defaultApprover;
            Literal litApprover = new Literal();
            string groupName = "";

            foreach (DataRow row in testData.Rows)
            {
                WorkflowApprover workflowApprover;
                workflowApprover = LoadControl(@"/Controls/WorkflowApprover.ascx") as WorkflowApprover;

                if (row["typeName"].ToString() == "Team Approver")
                {
                    if (groupName != row["groupName"].ToString())
                    {
                        if (litApprover != null)
                        {
                            defaultApprover = (PlaceHolder)WebUtilities.FindControlRecursive(workflowApprover, "_defaultApprover");
                            defaultApprover.Controls.Add(litApprover);
                        }

                        HtmlInputCheckBox approverCheck;
                        approverCheck = (HtmlInputCheckBox)WebUtilities.FindControlRecursive(workflowApprover, "_approverCheck");
                        approverCheck.Checked = false;

                        HtmlContainerControl approverTitle;
                        approverTitle = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_approverTitle");
                        approverTitle.InnerHtml = row["groupName"].ToString();
                        groupName = row["groupName"].ToString();

                        HtmlContainerControl approverDescription;
                        approverDescription = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_approverDescription");
                        approverDescription.InnerHtml = row["description"].ToString();

                        HtmlContainerControl deleteApprover;
                        deleteApprover = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_deleteApprover");
                        deleteApprover.Attributes.Add("onclick", "alert('Throw Modal');");

                        HtmlContainerControl editApprover;
                        editApprover = (HtmlContainerControl)WebUtilities.FindControlRecursive(workflowApprover, "_editApprover");
                        editApprover.Attributes.Add("onclick", "alert('Throw Modal');");

                        this._teamApprover.Controls.Add(workflowApprover);
                    }

                    if (groupName == row["groupName"].ToString())
                    {
                        litApprover.Text += "<tr><td>&nbsp;</td><td colspan=\"3\">";
                        if ((bool)row["isDefault"] == true)
                        {
                            litApprover.Text += "<input type=\"radio\" checked=\"checked\" class=\"csm_input_checkradio\" />";
                            litApprover.Text += "<span class=\"csm_inline\">" + row["displayName"].ToString() + "(Default)</span>";
                        }
                        else
                        {
                            litApprover.Text += "<input type=\"radio\" class=\"csm_input_checkradio\" />";
                            litApprover.Text += "<span class=\"csm_inline\">" + row["displayName"].ToString() + "(Secondary)</span>";
                        }
                        litApprover.Text += "</td></tr>";
                    }                    
                }
            }
        }

        static DataTable GetTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("displayName", typeof(string));
            table.Columns.Add("groupName", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("typeName", typeof(string));
            table.Columns.Add("isDefault", typeof(bool));

            table.Rows.Add("Chris Schwimmer", "Software Group", "The Software Group includes all the people who make the softwares.", "Team Approver", true);
            table.Rows.Add("Mark Garrigan", "Software Group", "The Software Group includes all the people who make the softwares.", "Team Approver", false);
            table.Rows.Add("Pong Lee", @"Unix\Linux", "Technical Group Description", "Technical Approver", true);
            table.Rows.Add("Mark Garrigan", @"Unix\Linux", "Technical Group Description", "Technical Approver", false);
            table.Rows.Add("Jonathan Steele", "Windows","Exchange, Windows Network Shares, Collaboration", "Technical Approver", true);
            table.Rows.Add("Chris Schwimmer", "Windows", "Exchange, Windows Network Shares, Collaboration", "Technical Approver", false);
            return table;
        }
    }
}