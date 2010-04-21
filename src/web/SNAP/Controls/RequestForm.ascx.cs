using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Data.Linq;
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
        private List<usp_open_request_tabResult> _requestFormData;

        protected void Page_Load(object sender, EventArgs e)
        {
			if (WebUtilities.CurrentViewIndex == ViewIndex.request_form || Page.IsPostBack)
			{
				BuildRequestForm();
			}
        }
        
        private void BuildRequestForm()
        {
			if (SnapSession.IsRequestPrePopulated)
			{
				_requestorId.Text = SnapSession.CurrentUser.FullName;
				_managerName.Text = SnapSession.CurrentUser.ManagerName;
			}
        
            _requestFormData = loadRequestFormData();

            if (!brandNewRequest())
            {
                this.UserName = _requestFormData[0].userDisplayName;
                this.UserLoginId = _requestFormData[0].userId;
                this.ManagerName = _requestFormData[0].managerDisplayName;
                this.ManagerLoginId = _requestFormData[0].managerUserId;
            }

            RequestFormSection requestFormSection=null;

            var data = loadRequestFormLabelDescriptionData();

            foreach (var access in data)
            {
                requestFormSection = LoadControl("~/Controls/RequestFormSection.ascx") as RequestFormSection;

                requestFormSection.ParentID = access.pkId.ToString();

                if (!brandNewRequest())
                    requestFormSection.RequestData = _requestFormData;

                Label outerDescription;
                outerDescription = (Label)WebUtilities.FindControlRecursive(requestFormSection, "_outerDescription");
                outerDescription.Text = access.description;

				//Label outerLabel;
				//outerLabel = (Label)WebUtilities.FindControlRecursive(requestFormSection, "_outerLabel");
				//outerLabel.Text = access.label;
				
				Literal htmlLabelText = new Literal();
				htmlLabelText.Text = access.label;

				HtmlControl labelContainer;
				labelContainer = (HtmlControl)WebUtilities.FindControlRecursive(requestFormSection, "_labelContainer");
				labelContainer.Controls.Add(htmlLabelText);

                if (access.isActive == true)
                {
                    labelContainer.Attributes.Add("class", "csm_input_required_field");
                }

                this._requestForm.Controls.Add(requestFormSection);
            }
        }

        public string UserName
        {
            get { return this._requestorId.Text; }
            set { _requestorId.Text = value;  }
        }

        public string UserLoginId
        {
            get { return this._requestorLoginId.Text; }
            set { _requestorLoginId.Text = value; }
        }

        public string ManagerName
        {
            get
            {
                return this._managerName.Text;
            }
            set { _managerName.Text = value; }
        }

        public string ManagerLoginId
        {
            get { return this._managerLoginId.Text; }
            set { _managerLoginId.Text = value; }
        }

        protected void _submitForm_Click(object sender, EventArgs e)
        {
            int requestID;
            try
            {
                List<RequestData> newRequestDataList = RequestFormRequestData(_requestForm);

                if (brandNewRequest())
                {
                    var xmlInput = RequestData.ToXml(newRequestDataList);

                    ADUserDetail detail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(UserLoginId);

                    using (var db = new SNAPDatabaseDataContext())
                    {
                        requestID = db.usp_insert_request_xml(xmlInput, SnapSession.CurrentUser.LoginId, UserLoginId, UserName, detail.Title, ManagerLoginId, ManagerName);
                        if (requestID > 0)
                        {
                            //TODO set Session variable to requestID
                            
                        }
                        else { Logger.Fatal("SNAP: Request Form -  Submit failure", "Unknown Error"); }
                    }
                }
                else
                {
                    RequestData.UpdateRequestData(newRequestDataList, _requestFormData);
                    requestID = System.Convert.ToInt32(Request.QueryString["RequestId"]);
                    var accessReq = new AccessRequest(requestID);
                    accessReq.RequestChanged();
                }

                Email.UpdateRequesterStatus(SnapSession.CurrentUser.LoginId, UserName, requestID, WorkflowState.Pending_Acknowlegement, string.Empty);
                Email.TaskAssignToApprover(ConfigurationManager.AppSettings["AIM-DG"], "Access Team", requestID, UserName);

            }
            catch (Exception ex)
            {
                Logger.Fatal("SNAP: Request Form -  Submit failure", ex);
            }
        }

        private List<RequestData> RequestFormRequestData(Control controlRoot)
        {
            List<RequestData> newRequestDataList = new List<RequestData>();

            if (!controlRoot.HasControls()) { return null; }

            foreach (Control childControl in controlRoot.Controls)
            {
                if (childControl.HasControls()) { newRequestDataList.AddRange(RequestFormRequestData(childControl)); }

                if (childControl is TextBox)
                {
                    TextBox textControl = (TextBox)WebUtilities.FindControlRecursive(controlRoot, (string)childControl.ID);
                    RequestData newRequest = new RequestData();
                    newRequest.FormId = textControl.ID;
                    newRequest.UserText = textControl.Text;
                    newRequestDataList.Add(newRequest);
                }
            }
            return newRequestDataList;
        }

        private IEnumerable<SNAP_Access_Details_Form> loadRequestFormLabelDescriptionData()
        {
            var db = new SNAPDatabaseDataContext();
            var formDetails = from form in db.SNAP_Access_Details_Forms
                                  where (form.parentId == 0) && (form.isActive == true)
                                  select form;
            return formDetails;
        }

        private List<usp_open_request_tabResult> loadRequestFormData()
        {
			if (!string.IsNullOrEmpty(Request.QueryString["RequestId"]))
			{
				var requestId = System.Convert.ToInt32(Request.QueryString["RequestId"]);
				var db = new SNAPDatabaseDataContext();
				var formData = db.usp_open_request_tab(WebUtilities.CurrentLoginUserId, requestId);
				// formData contain history of all data fields, we are only interested in the latest

				return formData.ToList();
			}
			else
			{
				return new List<usp_open_request_tabResult>();
			}

			//var requestId = Request.QueryString["RequestId"];
			//int reqId = 0;
			//// 1003
			//try
			//{
			//    if (requestId != null)
			//        reqId = System.Convert.ToInt32(requestId);
			//}
			//catch (Exception ex)
			//{
			//    Logger.Error("SNAP: Request Form - Request id convert error", ex);
			//}


			//if (reqId != 0)
			//{
			//    var db = new SNAPDatabaseDataContext();
			//    var formData = db.usp_open_request_tab(WebUtilities.CurrentLoginUserId, reqId);
			//    // formData contain history of all data fields, we are only interested in the latest

			//    return formData.ToList();
			//}
			//else
			//{
			//    return new List<usp_open_request_tabResult>();
			//}
        }

        private bool brandNewRequest()
        {
            return _requestFormData == null ||_requestFormData.Count() == 0;
        }
    }
}