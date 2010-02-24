using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Apollo.AIM.SNAP.CA;
using Apollo.AIM.SNAP.Web.Common;
using Apollo.AIM.SNAP.Model;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Web.Controls
{
    public partial class RequestForm : System.Web.UI.UserControl
    {
        private List<usp_open_request_tabResult> _requestData;

        protected void Page_Load(object sender, EventArgs e)
        {
            _requestData = loadRequestData();

            if (!brandNewRequest())
            {
                 //_requestData[0].userDisplayName
                // display user name, mgr name ...etc
            }

            RequestFormSection requestFormSection=null;

            var data = loadRequestFormData();

            foreach (var access in data)
            {
                requestFormSection = LoadControl(@"/Controls/RequestFormSection.ascx") as RequestFormSection;

                requestFormSection.ParentID = access.pkId.ToString();

                if (!brandNewRequest())
                    requestFormSection.RequestData = _requestData;

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
            try
            {
                List<RequestData> newRequestList = RequestFormRequestData(_requestForm);

                if (brandNewRequest())
                {
                    var xmlInput = RequestData.ToXml(newRequestList);

                    ADUserDetail detail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(UserLoginId);

                    using (var db = new SNAPDatabaseDataContext())
                    {
                        db.usp_insert_request_xml(xmlInput, browseUser, UserLoginId, UserName, detail.Title);
                    }

                }
                else
                {
                    RequestData.UpdateRequestData(newRequestList, _requestData);
                }

            }
            catch (Exception ex)
            {
                Logger.Fatal("SNAP: Request Form -  Submit failure", ex);
            }
        }



        private List<RequestData> RequestFormRequestData(Control controlRoot)
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

        private List<usp_open_request_tabResult> loadRequestData()
        {
            var requestId = Request.QueryString["RequestId"];
            int reqId = 0;
            // 1003
            try
            {
                if (requestId != null)
                    reqId = System.Convert.ToInt32(requestId);
            }
            catch (Exception ex)
            {
                Logger.Error("SNAP: Request Form - Request id convert error", ex);
            }
            if (reqId != 0)
            {
                var db = new SNAPDatabaseDataContext();
                var formData = db.usp_open_request_tab(browseUser, reqId);
                return formData.ToList();
            }
            else
            {
                return new List<usp_open_request_tabResult>();
            }
        }

        private bool brandNewRequest()
        {
            return _requestData == null ||_requestData.Count() == 0;
        }

        private string browseUser
        {
            get
            {
                // To-do: Should use CAP login user object here
                var x = Request.ServerVariables["AUTH_USER"].Split('\\')[1]; // remove domain name

                return "clschwim";
                //return x;
            }
        }
    }

}