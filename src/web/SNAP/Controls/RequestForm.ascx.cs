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
    public partial class RequestForm : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            loadSection();
        }

        private void loadSection()
        {
            RequestFormSection srfs;

            DataTable testData = GetTable();

            foreach (DataRow row in testData.Rows)
            {
                srfs = LoadControl(@"/Controls/RequestFormSection.ascx") as Apollo.AIM.SNAP.Web.Controls.RequestFormSection;
                srfs.FieldLabel = row["label"].ToString();
                srfs.FieldDescription = row["description"].ToString();
                srfs.FieldId = row["pkId"].ToString();
                if ((bool)row["isActive"] == true)
                {
                    srfs.LabelContainerCSS = "csm_input_required_field";
                }
                this._requestFormSection.Controls.Add(srfs);
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

            table.Rows.Add(1, 0, "Access Details","Please complete one or more of the following sections...",true,true);
            table.Rows.Add(5, 0, "Justification", "Enter the reason for requesting...", true, true);
            return table;
        }
    }

}