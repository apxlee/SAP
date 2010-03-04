using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class AccessTeamView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			BuildRequests(RequestState.Open, _openRequestsContainer, false);
			BuildRequests(RequestState.Closed, _closedRequestsContainer, true);
		}

		private void BuildRequests(RequestState RequestState, PlaceHolder BladeContainer, bool IsNullRecordTest)
		{
			DataTable requestTestTable = GetRequests(RequestState);

			if (!IsNullRecordTest)
			{
				foreach (DataRow request in requestTestTable.Rows)
				{
					MasterRequestBlade requestBlade;
					requestBlade = LoadControl("~/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
					requestBlade.RequestId = request["request_id"].ToString();
					requestBlade.AffectedEndUserName = request["affected_end_user_name"].ToString();
					requestBlade.OverallRequestStatus = request["overall_request_status"].ToString();
					requestBlade.LastUpdatedDate = request["last_updated_date"].ToString();
					requestBlade.IsSelectedRequest = (bool)request["is_selected"];

					BladeContainer.Controls.Add(requestBlade);
				}
			}
			else
			{
				_nullDataMessage_ClosedRequests.Visible = true;
			}
		}

		static DataTable GetRequests(RequestState RequestState)
		{
			DataTable table = new DataTable();
			table.Columns.Add("request_id", typeof(string));
			table.Columns.Add("affected_end_user_name", typeof(string));
			table.Columns.Add("overall_request_status", typeof(string));
			table.Columns.Add("last_updated_date", typeof(string));
			table.Columns.Add("is_selected", typeof(bool));

			if (RequestState == RequestState.Open)
			{
				//table.Rows.Add("12345", "User One", "Open", "Feb. 10, 2010", false);
				//table.Rows.Add("54321", "User Two", "Open", "Jan. 3, 2010", true);
			}

			if (RequestState == RequestState.Closed)
			{
				//table.Rows.Add("98544", "User One", "Closed", "Jan. 10, 2010", false);
				//table.Rows.Add("96554", "User Two", "Closed", "Jan. 3, 2010", false);
			}

			return table;
		}
	}
}