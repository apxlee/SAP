using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Apollo.AIM.SNAP.Web.Common;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Controls
{
    public partial class RequestForm : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RequestFormSection requestFormSection;

            var data = loadRequestFormData();

            //DataTable testData = GetTable();

            /*
            foreach (DataRow row in testData.Rows)
            {
                requestFormSection = LoadControl(@"/Controls/RequestFormSection.ascx") as RequestFormSection;

                requestFormSection.ParentID = row["pkId"].ToString();

                Label outerLabel;
                outerLabel = (Label)WebUtilities.FindControlRecursive(requestFormSection, "_outerLabel");
                outerLabel.Text = row["label"].ToString();

                Label outerDescription;
                outerDescription = (Label)WebUtilities.FindControlRecursive(requestFormSection, "_outerDescription");
                outerDescription.Text = row["description"].ToString();

                if ((bool)row["isActive"] == true)
                {
                    HtmlControl labelContainer;
                    labelContainer = (HtmlControl)WebUtilities.FindControlRecursive(requestFormSection, "_labelContainer");
                    labelContainer.Attributes.Add("class","csm_input_required_field");
                }

                this._requestForm.Controls.Add(requestFormSection);
            }
             */

            foreach (var access in data)
            {
                requestFormSection = LoadControl(@"/Controls/RequestFormSection.ascx") as RequestFormSection;

                requestFormSection.ParentID = access.pkId.ToString();

                Label outerLabel;
                outerLabel = (Label)WebUtilities.FindControlRecursive(requestFormSection, "_outerLabel");
                outerLabel.Text = access.label;

                Label outerDescription;
                outerDescription = (Label)WebUtilities.FindControlRecursive(requestFormSection, "_outerDescription");
                outerDescription.Text = access.description;

                if (access.isActive == true)
                {
                    HtmlControl labelContainer;
                    labelContainer = (HtmlControl)WebUtilities.FindControlRecursive(requestFormSection, "_labelContainer");
                    labelContainer.Attributes.Add("class", "csm_input_required_field");
                }

                this._requestForm.Controls.Add(requestFormSection);
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

        public string UserName
        {
            
            get { return this._requestorId.Text; }
        }

        public string UserLoginId
        {
            get { return this._requestorLoginId.Text; }
        }

        public string ManagerName
        {
            get
            {
                return this._managerName.Text;
            }
        }

        public string ManagerLoginId
        {
            get { return this._managerLoginId.Text; }
        }

        protected void _submitForm_Click(object sender, EventArgs e)
        {
            List<RequestData> newRequestList = RequestFormRequestData(_requestForm);
            string xmlInput = RequestData.ToXml(newRequestList);
        }

        public List<RequestData> RequestFormRequestData(Control controlRoot)
        {
            List<RequestData> newRequestList = new List<RequestData>();

            if (!controlRoot.HasControls()) { return null; }

            foreach (Control childControl in controlRoot.Controls)
            {
                if (childControl.HasControls()) { newRequestList.AddRange(RequestFormRequestData(childControl)); }

                if (childControl is TextBox)
                {
                    TextBox textControl = (TextBox)WebUtilities.FindControlRecursive(controlRoot, (string)childControl.ID);
                    RequestData newRequest = new RequestData();
                    newRequest.FormId = textControl.ID;
                    newRequest.UserText = textControl.Text;
                    newRequestList.Add(newRequest);
                }
            }
            return newRequestList;
        }


        private IEnumerable<SNAP_Access_Details_Form> loadRequestFormData()
        {
            var db = new SNAPDatabaseDataContext();
            var formDetails = from form in db.SNAP_Access_Details_Forms
                              where (form.parentId == 0) && (form.isActive == true)
                              select form;
            return formDetails;

        }
    }

}