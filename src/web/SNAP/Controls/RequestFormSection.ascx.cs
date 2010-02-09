using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace Apollo.AIM.SNAP.Web.Controls
{
    public partial class RequestFormSection : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            loadFields();
        }

        private void loadFields()
        {
            RequestFormField srff;
            int rowCount = 0;
            DataTable testData = GetTable();

            foreach (DataRow row in testData.Rows)
            {
                if (row["parentId"].ToString() == ParentId.Text)
                {
                    srff = LoadControl(@"/Controls/RequestFormField.ascx") as Apollo.AIM.SNAP.Web.Controls.RequestFormField;
                    srff.FieldLabel = row["label"].ToString();
                    srff.FieldDescription = row["description"].ToString();
                    srff.FieldId = row["pkId"].ToString();
                    this.phrRequestFormField.Controls.Add(srff);
                    rowCount = rowCount + 1;
                }
            }

            if (rowCount == 0)
            {
                //<asp:TextBox ID="Access_Details_FormId" runat="server" TextMode="MultiLine" Rows="10" CssClass="csm_text_input"></asp:TextBox>
                TextBox parentTextbox = new TextBox();
                parentTextbox.ID = "textbox_" + ParentId.Text;
                parentTextbox.TextMode = TextBoxMode.MultiLine;
                parentTextbox.Rows = 10;
                parentTextbox.CssClass = "csm_text_input";
                this.phrRequestFormField.Controls.Add(parentTextbox);
            }
        }

        public string FieldLabel
        {
            set
            {
                OuterLabel.Text = value;
            }
        }

        public string LabelContainerCSS
        {
            set
            {
                LabelContainer.Attributes.Add("class", value);
            }
        }

        public string FieldDescription
        {
            set
            {
                OuterDescription.Text = value;
            }
        }

        public string FieldId
        {
            set
            {
                ParentId.Text = value;
            }
        }

        static DataTable GetTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("pkId", typeof(int));
            table.Columns.Add("parentId", typeof(int));
            table.Columns.Add("label", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("isActive", typeof(bool));
            table.Columns.Add("isRequired", typeof(bool));

            table.Rows.Add(2, 1, "Windows Servers", "<p>Enter the <strong>exact</strong> servers you need access to...</p>", true, true);
            table.Rows.Add(3, 1, "Linux/Unix Servers", "Enter the exact...", true, true);
            table.Rows.Add(4, 1, "Network Shares", "Enter the exact...", true, true);
            return table;
        }
    }
}