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
			DataTable requestTestTable = ViewBaseUtilities.GetRequests(requestState, selectedRequestId);
			Page currentPage = HttpContext.Current.Handler as Page;

			foreach (DataRow request in requestTestTable.Rows)
			{
				MasterRequestBlade requestBlade;
				requestBlade = currentPage.LoadControl("~/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
				requestBlade.RequestId = request["request_id"].ToString();
				requestBlade.RequestState = requestState;
				requestBlade.AffectedEndUserName = request["affected_end_user_name"].ToString();
				requestBlade.OverallRequestStatus = request["overall_request_status"].ToString();
				requestBlade.LastUpdatedDate = request["last_updated_date"].ToString();
				requestBlade.IsSelectedRequest = (bool)request["is_selected"];

				bladeContainer.Controls.Add(requestBlade);
			}
		}
		
		static DataTable GetRequests(RequestState requestState, string selectedRequestId)
		{
            DataTable table = new DataTable();
            table.Columns.Add("request_id", typeof(string));
			table.Columns.Add("affected_end_user_name", typeof(string));
			table.Columns.Add("overall_request_status", typeof(string));
			table.Columns.Add("last_updated_date", typeof(string));
			table.Columns.Add("is_selected", typeof(bool));

            var reqDetails = Common.Request.Details(requestState);
            foreach (usp_open_my_request_detailsResult request in reqDetails)
            {
                table.Rows.Add(
					request.pkId
					, request.userDisplayName.StripTitleFromUserName()
                    , Convert.ToString((RequestState)Enum.Parse(typeof(RequestState)
					, request.statusEnum.ToString())).StripUnderscore()
					, request.createdDate.ToString("MMM d, yyyy")
					, (request.pkId.ToString().Trim() == selectedRequestId) ? true : false
                    );
                // is this "last updated date" or "created date"?
            }


            /*
           	if (requestState == RequestState.Open)
		    {
                var reqDetails = Common.Request.Details(requestState);

			    //table.Rows.Add("12345", "User One", "Open", "Feb. 10, 2010", false);
			    //table.Rows.Add("54321", "User Two", "Open", "Jan. 3, 2010", true);

                foreach (usp_open_my_request_detailsResult list in reqDetails)
                {
                    table.Rows.Add(list.pkId, list.userDisplayName.StripTitleFromUserName()
						, Convert.ToString((WorkflowState)Enum.Parse(typeof(WorkflowState), list.statusEnum.ToString())).StripUnderscore()
						, list.createdDate.ToString("MMM d, yyyy"), false);
                    // is this "last updated date" or "created date"?
                }
		    }

			if (requestState == RequestState.Closed)
			{
                
                
                var reqDetails = Common.Request.Details(requestState);

                //table.Rows.Add("12345", "User One", "Open", "Feb. 10, 2010", false);
                //table.Rows.Add("54321", "User Two", "Open", "Jan. 3, 2010", true);

                foreach (usp_open_my_request_detailsResult list in reqDetails)
                {
                    table.Rows.Add(list.pkId, list.userDisplayName.StripTitleFromUserName()
                        , Convert.ToString((WorkflowState)Enum.Parse(typeof(WorkflowState), list.statusEnum.ToString())).StripUnderscore()
                        , list.createdDate.ToString("MMM d, yyyy"), false);
                    // is this "last updated date" or "created date"?
                }
                

				//table.Rows.Add("98544", "User One", "Closed", "Jan. 10, 2010", false);
				//table.Rows.Add("96554", "User Two", "Closed", "Jan. 3, 2010", false);
                
			}			
			*/
			return table;
		}

        public static void BuildRequests(Page page, RequestState RequestState, PlaceHolder BladeContainer, Panel nullMessage, bool IsNullRecordTest)
        {
            DataTable requestTestTable = ViewBaseUtilities.GetRequests(RequestState, null);
            List<AccessGroup> availableGroups = new List<AccessGroup>();
            
            using (var db = new SNAPDatabaseDataContext())
            {
                availableGroups = ApprovalWorkflow.GetAvailableGroups();
            }

            if (!IsNullRecordTest)
            {
                foreach (DataRow request in requestTestTable.Rows)
                {
                    MasterRequestBlade requestBlade;
                    requestBlade = page.LoadControl("~/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
                    requestBlade.RequestId = request["request_id"].ToString();
                    requestBlade.RequestState = RequestState;
                    requestBlade.AffectedEndUserName = request["affected_end_user_name"].ToString();
                    requestBlade.OverallRequestStatus = request["overall_request_status"].ToString();
                    requestBlade.LastUpdatedDate = request["last_updated_date"].ToString();
                    requestBlade.IsSelectedRequest = (bool)request["is_selected"];
                    requestBlade.AvailableGroups = availableGroups;

                    BladeContainer.Controls.Add(requestBlade);
                }
            }
            else
            {
                nullMessage.Visible = true;
            }
        }
        

    }
}
