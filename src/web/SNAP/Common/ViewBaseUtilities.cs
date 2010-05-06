using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Controls;

namespace Apollo.AIM.SNAP.Web.Common
{
    public class ViewBaseUtilities
    {
        public static void BuildApproverRequests(Page page, RequestState requestState, PlaceHolder bladeContainer, Panel nullMessage)
		{
			Page currentPage = HttpContext.Current.Handler as Page;
            DataTable requestTable = ViewBaseUtilities.GetRequests(requestState, null);
            WorkflowState ApproverState = WorkflowState.Not_Active;

            PlaceHolder pendingContainer = new PlaceHolder();
            pendingContainer = (PlaceHolder)WebUtilities.FindControlRecursive(page, "_pendingApprovalsContainer");

            PlaceHolder openContainer = new PlaceHolder();
            openContainer = (PlaceHolder)WebUtilities.FindControlRecursive(page, "_openRequestsContainer");

            if (requestTable.Rows.Count < 1)
            {
                nullMessage.Visible = true;
            }

            foreach (DataRow request in requestTable.Rows)
            {
                List<AccessApprover> requestApprovers = new List<AccessApprover>();
                int requestId = Convert.ToInt32(request["request_id"].ToString());
                requestApprovers = ApprovalWorkflow.GetRequestApprovers(requestId);
                foreach (AccessApprover approver in requestApprovers)
                {
                    if (approver.UserId == SnapSession.CurrentUser.LoginId)
                    {
                        ApproverState = (WorkflowState)ApprovalWorkflow.GetWorkflowState(ApprovalWorkflow.GetWorkflowId(approver.ActorId, requestId));
                    }
                }

                if (ApproverState == WorkflowState.Pending_Approval)
                {
                    pendingContainer.Controls.Add(BuildMasterBlade(request, currentPage, requestState, null));
                }
                else
                {
                    openContainer.Controls.Add(BuildMasterBlade(request, currentPage, requestState, null));
                }
            }
		}
		
        public static DataTable GetRequestDetails(int requestId)
        {
            DataTable detailsTable = new DataTable();
            detailsTable.Columns.Add("fieldId", typeof(int));
            detailsTable.Columns.Add("fieldLabel", typeof(string));
            detailsTable.Columns.Add("fieldText", typeof(string));

            using (var db = new SNAPDatabaseDataContext())
            {
                var result = db.usp_request_details(requestId);

                if (result != null)
                {
                    foreach (var row in result)
                    {
						string modifiedFieldText = (string.IsNullOrEmpty(row.fieldText)) ? "<span class=\"aim_dark_text\">[Empty]</span>" : row.fieldText;
						detailsTable.Rows.Add(row.fieldId, row.fieldLabel, modifiedFieldText);
                    }
                }
            }
            
            return detailsTable;
        }

        public static void BuildRequests(Page page, RequestState requestState, PlaceHolder bladeContainer, Panel nullMessage)
        {
			//string selectedRequestId = SnapSession
            DataTable requestTable = ViewBaseUtilities.GetRequests(requestState, SnapSession.SelectedRequestId);
            //only use it once the clear requestId
            SnapSession.SelectedRequestId = "";
            List<AccessGroup> availableGroups = ApprovalWorkflow.GetAvailableGroups();

			if (requestTable.Rows.Count > 0)
			{
				foreach (DataRow request in requestTable.Rows)
				{
					bladeContainer.Controls.Add(BuildMasterBlade(request, page, requestState, availableGroups));
				}
			}
			else { nullMessage.Visible = true; }
        }

		static DataTable GetRequests(RequestState requestState, string selectedRequestId)
		{
			DataTable requestTable = new DataTable();
			requestTable.Columns.Add("request_id", typeof(string));
			requestTable.Columns.Add("affected_end_user_name", typeof(string));
			requestTable.Columns.Add("overall_request_status", typeof(string));
			requestTable.Columns.Add("last_updated_date", typeof(string));
			requestTable.Columns.Add("is_selected", typeof(bool));

			var reqDetails = Common.Request.Details(requestState);
			foreach (usp_open_my_request_detailsResult request in reqDetails)
			{
				requestTable.Rows.Add(
					request.pkId
					, request.userDisplayName.StripTitleFromUserName()
					, Convert.ToString((RequestState)Enum.Parse(typeof(RequestState)
					, request.statusEnum.ToString())).StripUnderscore()
					, request.lastModifiedDate.ToString("MMM d, yyyy")
					, (request.pkId.ToString().Trim() == selectedRequestId) ? true : false
					);
				// is this "last updated date" or "created date"?
			}

			return requestTable;
		}        

        private static MasterRequestBlade BuildMasterBlade(DataRow request, Page page, RequestState RequestState, List<AccessGroup> AvailableGroups)
        {
            MasterRequestBlade requestBlade;
            requestBlade = page.LoadControl("~/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
            requestBlade.RequestId = request["request_id"].ToString();
            requestBlade.RequestState = RequestState;
            requestBlade.AffectedEndUserName = request["affected_end_user_name"].ToString();
            requestBlade.OverallRequestStatus = request["overall_request_status"].ToString();
            requestBlade.LastUpdatedDate = request["last_updated_date"].ToString();
            requestBlade.IsSelectedRequest = (bool)request["is_selected"];
            requestBlade.AvailableGroups = AvailableGroups;

            return requestBlade;
        }
    }
}
