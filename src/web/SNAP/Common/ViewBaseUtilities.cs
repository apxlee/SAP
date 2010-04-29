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
		public static void BuildRequestBlades(RequestState requestState, PlaceHolder bladeContainer, Panel nullMessage, string selectedRequestId)
		{
			Page currentPage = HttpContext.Current.Handler as Page;
			DataTable requestTable = ViewBaseUtilities.GetRequests(requestState, selectedRequestId);
			
			if (requestTable.Rows.Count < 1)
			{
				nullMessage.Visible = true;
			}
			else
			{
                /*
				List<AccessGroup> availableGroups = new List<AccessGroup>();
				using (var db = new SNAPDatabaseDataContext())
				{
					availableGroups = ApprovalWorkflow.GetAvailableGroups();
				}
				*/
                 

				foreach (DataRow request in requestTable.Rows)
				{
                    /*
					MasterRequestBlade requestBlade;
					requestBlade = currentPage.LoadControl("~/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
					requestBlade.RequestId = request["request_id"].ToString();
					requestBlade.RequestState = requestState;
					requestBlade.AffectedEndUserName = request["affected_end_user_name"].ToString();
					requestBlade.OverallRequestStatus = request["overall_request_status"].ToString();
					requestBlade.LastUpdatedDate = request["last_updated_date"].ToString();
					requestBlade.IsSelectedRequest = (bool)request["is_selected"];
					requestBlade.AvailableGroups = availableGroups;
                    */

                    bladeContainer.Controls.Add(buildMasterBlade(request, currentPage,requestState));
				}
			}
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
					, request.createdDate.ToString("MMM d, yyyy")
					, (request.pkId.ToString().Trim() == selectedRequestId) ? true : false
                    );
                // is this "last updated date" or "created date"?
            }

			return requestTable;
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

        public static void BuildRequests(Page page, RequestState RequestState, PlaceHolder BladeContainer, Panel nullMessage, bool IsNullRecordTest)
        {
            DataTable requestTestTable = ViewBaseUtilities.GetRequests(RequestState, null);

            /*
            List<AccessGroup> availableGroups = new List<AccessGroup>();
            
            using (var db = new SNAPDatabaseDataContext())
            {
                availableGroups = ApprovalWorkflow.GetAvailableGroups();
            }
            */

            if (!IsNullRecordTest)
            {
                foreach (DataRow request in requestTestTable.Rows)
                {
                    /*
                    MasterRequestBlade requestBlade;
                    requestBlade = page.LoadControl("~/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
                    requestBlade.RequestId = request["request_id"].ToString();
                    requestBlade.RequestState = RequestState;
                    requestBlade.AffectedEndUserName = request["affected_end_user_name"].ToString();
                    requestBlade.OverallRequestStatus = request["overall_request_status"].ToString();
                    requestBlade.LastUpdatedDate = request["last_updated_date"].ToString();
                    requestBlade.IsSelectedRequest = (bool)request["is_selected"];
                    requestBlade.AvailableGroups = availableGroups;
                    */

                    BladeContainer.Controls.Add(buildMasterBlade(request,page,RequestState));

                }
            }
            else
            {
                nullMessage.Visible = true;
            }
        }
        

        static MasterRequestBlade buildMasterBlade(DataRow request, Page page, RequestState RequestState)
        {
            var availableGroups = ApprovalWorkflow.GetAvailableGroups();
            MasterRequestBlade requestBlade;
            requestBlade = page.LoadControl("~/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
            requestBlade.RequestId = request["request_id"].ToString();
            requestBlade.RequestState = RequestState;
            requestBlade.AffectedEndUserName = request["affected_end_user_name"].ToString();
            requestBlade.OverallRequestStatus = request["overall_request_status"].ToString();
            requestBlade.LastUpdatedDate = request["last_updated_date"].ToString();
            requestBlade.IsSelectedRequest = (bool)request["is_selected"];
            requestBlade.AvailableGroups = availableGroups;

            return requestBlade;
        }
    }
}
