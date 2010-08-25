using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Controls;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Web.Common
{
    public class ViewBaseUtilities
    {
        public static void BuildApproverRequests(Page page, RequestState requestState, PlaceHolder bladeContainer)
		{
            
            string requestApprover = SnapSession.CurrentUser.DistributionGroup != null ? SnapSession.CurrentUser.DistributionGroup : SnapSession.CurrentUser.LoginId;              

            Page currentPage = HttpContext.Current.Handler as Page;
            DataTable requestTable = ViewBaseUtilities.GetRequests(requestState, SnapSession.SelectedRequestId);
            WorkflowState ApproverState = WorkflowState.Not_Active;

            PlaceHolder pendingContainer = new PlaceHolder();
            pendingContainer = (PlaceHolder)WebUtilities.FindControlRecursive(page, "_pendingApprovalsContainer");

            Panel pendingNullData = new Panel();
            pendingNullData = (Panel)WebUtilities.FindControlRecursive(page, "_nullDataMessage_PendingApprovals");

            PlaceHolder openContainer = new PlaceHolder();
            openContainer = (PlaceHolder)WebUtilities.FindControlRecursive(page, "_openRequestsContainer");

            Panel openNullData = new Panel();
            openNullData = (Panel)WebUtilities.FindControlRecursive(page, "_nullDataMessage_OpenRequests");

            string[] userIds = SnapSession.CurrentUser.DistributionGroup !=
                null ? new string[] { SnapSession.CurrentUser.DistributionGroup, SnapSession.CurrentUser.LoginId }
                : new string[] { SnapSession.CurrentUser.LoginId };
            int approvalCount = Request.ApprovalCount(userIds);
            RequestState OverAllState = new RequestState();
            int lastRow = 1;
            bool IsLastRow = false;

            if (approvalCount == 0) { pendingNullData.Visible = true; }
            if ((requestTable.Rows.Count - approvalCount) < 1) { openNullData.Visible = true; }

            foreach (DataRow request in requestTable.Rows)
            {
                List<AccessApprover> requestApprovers = new List<AccessApprover>();
                int requestId = Convert.ToInt32(request["request_id"].ToString());
                requestApprovers = ApprovalWorkflow.GetRequestApprovers(requestId);
                foreach (AccessApprover approver in requestApprovers)
                {
                    if (userIds.Contains(approver.UserId))
                    {
                        ApproverState = (WorkflowState)ApprovalWorkflow.GetWorkflowState(ApprovalWorkflow.GetWorkflowId(approver.ActorId, requestId));
                        if (ApproverState == WorkflowState.Pending_Approval) { break; }
                    }
                }
                OverAllState = (RequestState)Enum.Parse(typeof(RequestState), request["overall_request_status"].ToString());
                if (ApproverState == WorkflowState.Pending_Approval && OverAllState == RequestState.Pending)
                {
                    if (lastRow == approvalCount) { IsLastRow = true; }
                    pendingContainer.Controls.Add(BuildMasterBlade(request, currentPage, requestState, null, true, IsLastRow));
                    lastRow++;
                }
                else
                {
                    openContainer.Controls.Add(BuildMasterBlade(request, currentPage, requestState, null, false, false));
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
                        detailsTable.Rows.Add(row.fieldId, row.fieldLabel, modifiedFieldText.Replace("\n", "<br />"));
                    }
                }
            }
            
            return detailsTable;
        }

        public static void BuildRequests(Page page, RequestState requestState, PlaceHolder bladeContainer, Panel nullMessage)
        {
            DataTable requestTable = ViewBaseUtilities.GetRequests(requestState, SnapSession.SelectedRequestId);
            List<AccessGroup> availableGroups = ApprovalWorkflow.GetAvailableGroups();
            
			if (requestTable.Rows.Count > 0)
			{
				foreach (DataRow request in requestTable.Rows)
				{
                    bladeContainer.Controls.Add(BuildMasterBlade(request, page, requestState, availableGroups, false, false));
                }
			}
			else { nullMessage.Visible = true; }
        }

		static DataTable GetRequests(RequestState requestState, string selectedRequestId)
		{
			DataTable requestTable = new DataTable();
			requestTable.Columns.Add("request_id", typeof(string));
			requestTable.Columns.Add("affected_end_user_name", typeof(string));
			requestTable.Columns.Add("overall_request_status", typeof(RequestState));
			requestTable.Columns.Add("last_updated_date", typeof(string));
			requestTable.Columns.Add("is_selected", typeof(bool));

			var reqDetails = Common.Request.Details(requestState);
			foreach (usp_open_my_request_detailsResult request in reqDetails)
			{
				requestTable.Rows.Add(
					request.pkId
					, request.userDisplayName.StripTitleFromUserName()
					, request.statusEnum.ToString()
					, request.lastModifiedDate.ToString("MMM d, yyyy")
					, (request.pkId.ToString().Trim() == selectedRequestId) ? true : false
					);
				// is this "last updated date" or "created date"?
			}

			return requestTable;
		}

        public static void SetGroupMembership()
        {
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var result = (from sr in db.SNAP_Requests
                                     join sw in db.SNAP_Workflows on sr.pkId equals sw.requestId
                                     join sws in db.SNAP_Workflow_States on sw.pkId equals sws.workflowId
                                     join sa in db.SNAP_Actors on sw.actorId equals sa.pkId
                                     where sa.isActive == true
                                     && sa.isGroup == true
                                     && SnapSession.CurrentUser.MemberOf.Contains(sa.userId)
                                     && sws.workflowStatusEnum == 7
                                     && sws.completedDate == null
                                     && sr.statusEnum == 2
                                     select new { sa.userId });

                    if (result.Count() > 0)
                    {
                        SnapSession.CurrentUser.DistributionGroup = result.First().userId;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ViewBasedUtilities - SetGroupMembership, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
            }
        }

        private static MasterRequestBlade BuildMasterBlade(DataRow request, Page page, RequestState RequestState, List<AccessGroup> AvailableGroups,bool IsPendingApproval, bool IsLastRequest)
        {
            MasterRequestBlade requestBlade;
            requestBlade = page.LoadControl("~/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
            requestBlade.RequestId = request["request_id"].ToString();
            requestBlade.RequestState = RequestState;
            requestBlade.AffectedEndUserName = request["affected_end_user_name"].ToString();
            requestBlade.OverallRequestStatus = Convert.ToString((RequestState)Enum.Parse(typeof(RequestState)
            , request["overall_request_status"].ToString())).StripUnderscore();
            requestBlade.LastUpdatedDate = request["last_updated_date"].ToString();
            requestBlade.IsSelectedRequest = (bool)request["is_selected"];
            requestBlade.AvailableGroups = AvailableGroups;
            requestBlade.IsPendingApproval = IsPendingApproval;
            requestBlade.IsLastRequest = IsLastRequest;

            return requestBlade;
        }
    }
}
