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
    public partial class RequestFormSection : System.Web.UI.UserControl
    {
        public List<usp_open_request_tabResult> RequestData;

        protected void Page_Load(object sender, EventArgs e)
        {
            var data = loadDetailRequestFormData();

            RequestFormField requestFormField;
            int rowCount = 0;
            
            foreach (var access in data)
            {
                if (access.parentId.ToString() == ParentID)
                {
                    requestFormField = LoadControl("~/Controls/RequestFormField.ascx") as RequestFormField;

                    Label innerLabel;
                    innerLabel = (Label)WebUtilities.FindControlRecursive(requestFormField, "_innerLabel");
                    innerLabel.Text = access.label;

                    Label innerDescription;
                    innerDescription = (Label)WebUtilities.FindControlRecursive(requestFormField, "_innerDescription");
                    innerDescription.Text = access.description;

                    TextBox accessDetailsFormId;
                    accessDetailsFormId = (TextBox)WebUtilities.FindControlRecursive(requestFormField, "_accessDetailsFormId");
                    accessDetailsFormId.ID = access.pkId.ToString();
                    accessDetailsFormId.Text = requestDataField(access.pkId);
                    accessDetailsFormId.TextMode = TextBoxMode.MultiLine;
                    accessDetailsFormId.Rows = 10;
                    accessDetailsFormId.CssClass = "csm_text_input";

                    innerLabel.AssociatedControlID = accessDetailsFormId.ID;

                    this._requestFormField.Controls.Add(requestFormField);
                    rowCount = rowCount + 1;
                }
            }

            if (rowCount == 0)
            {
                //<asp:TextBox ID="Access_Details_FormId" runat="server" TextMode="MultiLine" Rows="10" CssClass="csm_text_input"></asp:TextBox>
                TextBox parentTextbox = new TextBox();
                //parentTextbox.ID = "textbox_" + ParentID;
                parentTextbox.ID = ParentID;
                parentTextbox.TextMode = TextBoxMode.MultiLine;
                parentTextbox.Rows = 10;
                parentTextbox.Text = requestDataField(System.Convert.ToInt16(ParentID));
                parentTextbox.CssClass = "csm_text_input";
                this._requestFormField.Controls.Add(parentTextbox);
            }
        }

        public string ParentID { get; set; }

        /*
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
            table.Rows.Add(3, 1, "Linux/Unix Servers", "<p>Enter the exact...</p>", true, true);
            table.Rows.Add(4, 1, "Network Shares", "<p>Enter the exact...</p>", true, true);
            return table;
        }
        */

        private IEnumerable<SNAP_Access_Details_Form> loadDetailRequestFormData()
        {

            var db = new SNAPDatabaseDataContext();
            var formDetails = from form in db.SNAP_Access_Details_Forms
                                  where (form.parentId == 1) && (form.isActive == true)
                                  select form;
                
            return formDetails;
        }


        private string requestDataField(int fieldId)
        {
            if (RequestData == null)
                return "";

            if (RequestData.Count() > 0)
            {
                /*
                var item = RequestData.Single(x => x.fieldId == fieldId);
                return item.fieldText;
                 */

                var items = (from r in RequestData
                            where r.fieldId == fieldId
                            select r).ToList();

                if (items.Count() > 1)  // fields have multiple entries due to modification
                {
                    var t = (DateTime) items.Max(x => x.modifiedDate); 
                    var item = items.Single(x => x.modifiedDate == t);
                    return item.fieldText;
                }
                else
                {
                    return items[0].fieldText;
                }
            }

            return "";
        }
    }
}