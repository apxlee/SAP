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
    
        protected void Page_Load(object sender, EventArgs e)
        {
			_requiredApproverFullName.Text = "Greg Belanger";
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