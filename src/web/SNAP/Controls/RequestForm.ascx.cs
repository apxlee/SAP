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
using System.Threading;
using MyRequest = Apollo.AIM.SNAP.Web.Common.Request;

namespace Apollo.AIM.SNAP.Web.Controls
{
    public partial class RequestForm : System.Web.UI.UserControl
    {
        private List<usp_open_request_tabResult> _requestFormData;

		#region Public Properties // TODO: remove these are redundant

		//public string UserName
		//{
		//    get { return this._requestorId.Text; }
		//    set { _requestorId.Text = value; }
		//}

		//public string UserLoginId
		//{
		//    get { return this._requestorLoginId.Text; }
		//    set { _requestorLoginId.Text = value; }
		//}

		//public string ManagerName
		//{
		//    get { return this._managerName.Text; }
		//    set { _managerName.Text = value; }
		//}

		//public string ManagerLoginId
		//{
		//    get { return this._managerLoginId.Text; }
		//    set { _managerLoginId.Text = value; }
		//}
		
		#endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            BuildRequestForm();
        }
        
        private void BuildRequestForm()
        {
			if (SnapSession.IsRequestPrePopulated && !Page.IsPostBack)
			{
				_requestorId.Text = SnapSession.CurrentUser.FullName;
				_managerName.Text = SnapSession.CurrentUser.ManagerName;
				_managerLoginId.Text = SnapSession.CurrentUser.ManagerLoginId;
				_requestorLoginId.Text = SnapSession.CurrentUser.LoginId;
			}
        
            _requestFormData =loadRequestFormData();

            if (!BrandNewRequest() && !Page.IsPostBack)
            {
				_requestorId.Text = _requestFormData[0].userDisplayName;
				_requestorLoginId.Text = _requestFormData[0].userId;
				_managerName.Text = _requestFormData[0].managerDisplayName;
				_managerLoginId.Text = _requestFormData[0].managerUserId;
                
				LoadChangeComments();
            }

            RequestFormSection requestFormSection=null;

            var data = loadRequestFormLabelDescriptionData();

            foreach (var access in data)
            {
                requestFormSection = LoadControl("~/Controls/RequestFormSection.ascx") as RequestFormSection;

                requestFormSection.ParentID = access.pkId.ToString();

                if (!BrandNewRequest() && !Page.IsPostBack)
                    requestFormSection.RequestData = _requestFormData;

                Label outerDescription;
                outerDescription = (Label)WebUtilities.FindControlRecursive(requestFormSection, "_outerDescription");
                outerDescription.Text = access.description;

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


            if (_requestFormData.Count == 0 && !string.IsNullOrEmpty(SnapSession.SelectedRequestId))
            {
                _changeComments.Text = "Request Status Changed! At this time, you cannot perform modification or Not Your Request. ";
                SnapSession.SelectedRequestId = string.Empty;
            }

        }

        protected void _submitForm_Click(object sender, EventArgs e)
        {
            int requestID;
            var sendEmail = true;
            try
            {
                List<RequestData> newRequestDataList = RequestFormRequestData(_requestForm);

                if (BrandNewRequest())
                {
                    var xmlInput = RequestData.ToXml(newRequestDataList);

					ADUserDetail detail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(_requestorLoginId.Text);

                    using (var db = new SNAPDatabaseDataContext())
                    {
						requestID = db.usp_insert_request_xml(xmlInput, SnapSession.CurrentUser.LoginId, _requestorLoginId.Text, _requestorId.Text, detail.Title, _managerLoginId.Text, _managerName.Text);
                        if (requestID > 0)
                        {
                            SnapSession.SelectedRequestId = requestID.ToString();
                        }
                        else
                        {
                            Logger.Fatal("SNAP: Request Form -  Submit failure", "Unknown Error");
                            sendEmail = false;
                        }
                    }
                }
                else
                {
                    requestID = System.Convert.ToInt32(SnapSession.SelectedRequestId);
					updateRequestUsrInfo(requestID, SnapSession.CurrentUser.LoginId, _requestorLoginId.Text, _requestorId.Text, _managerLoginId.Text, _managerName.Text);
                    RequestData.UpdateRequestData(newRequestDataList, _requestFormData);

                    var accessReq = new AccessRequest(requestID);
                    if (!accessReq.RequestChanged().Success)
                    {
                        sendEmail = false;
                    }
                }

                if (sendEmail)
                {
					if (SnapSession.CurrentUser.LoginId != _requestorLoginId.Text)
					{
						Email.SendTaskEmail(EmailTaskType.ProxyForAffectedEndUser, SnapSession.CurrentUser.LoginId + "@apollogrp.edu", SnapSession.CurrentUser.FullName, requestID, _requestorId.Text);
					}
					Email.SendTaskEmail(EmailTaskType.AccessTeamAcknowledge, ConfigurationManager.AppSettings["AIM-DG"], null, requestID, _requestorId.Text);
					Email.SendTaskEmail(EmailTaskType.UpdateRequester, _requestorLoginId.Text + "@apollogrp.edu", _requestorId.Text, requestID, _requestorId.Text, WorkflowState.Pending_Acknowledgement, null);
					// TODO: UserLoginId concatenates with @apollogrp, but that may not be the addy.
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal("RequestForm - _submitForm_Click, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
            }

			WebUtilities.Redirect(PageNames.USER_VIEW);
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
            if (!string.IsNullOrEmpty(SnapSession.SelectedRequestId))
			{
                var requestId = System.Convert.ToInt32(SnapSession.SelectedRequestId);
				var db = new SNAPDatabaseDataContext();
				var formData = db.usp_open_request_tab(SnapSession.CurrentUser.LoginId, requestId);
				// formData contain history of all data fields, we are only interested in the latest

				return formData.ToList();
			}
			
			return new List<usp_open_request_tabResult>();
        }

        private void LoadChangeComments()
        {
            DataTable changeComments = MyRequest.GetChangeComments(Convert.ToInt32(SnapSession.SelectedRequestId));

            foreach (DataRow comment in changeComments.Rows)
            {
                _changeComments.Text += string.Format("<span>{2} - {0} has requested a change:</span><p>{1}</p>", comment[0], comment[1].ToString().Replace("\n","<br />"), Convert.ToDateTime(comment[2]).ToString("MMM d, yyyy"));
            }
        }

        private bool BrandNewRequest()
        {
            return _requestFormData == null || _requestFormData.Count() == 0;
        }

        private void updateRequestUsrInfo(long requestId, string submitterId, string userId, string userName, string mgrId, string mgrName)
        {
            var change = false;
            ADUserDetail usrDetail = null;

            using (var db = new SNAPDatabaseDataContext())
            {
                var req = db.SNAP_Requests.Single(x => x.pkId == requestId);

                // we want to keep submitter the same, eventhough aeu can modify the form after requestchange
                /*
                if (req.submittedBy != submitterId)
                {
                    req.submittedBy = submitterId;
                    change = true;
                }
                 */

                if (req.userId != userId)
                {
                    req.userId = userId;
                    usrDetail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(userId);
                    req.userTitle = usrDetail.Title;
                    change = true;
                }

                if (req.userDisplayName != userName)
                {
                    req.userDisplayName = userName;
                    change = true;
                }

                if (req.managerUserId != mgrId)
                {
                    req.managerUserId = mgrId;
                    change = true;
                }

                if (req.managerDisplayName != mgrName)
                {
                    req.managerDisplayName = mgrName;
                    change = true;
                }

                if (change)
                {
                    req.lastModifiedDate = DateTime.Now;
                    db.SubmitChanges();
                }
            }
        }
    }
}