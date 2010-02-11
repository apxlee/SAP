using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class UserView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			// TODO: move to index, utility to find AD group, look into DB for approving rows to determine role
			Page.Session["SNAPUserRole"] = Role.User;
		
			// build dataset of all master blade rows
			
			BuildRequests(RequestState.Open, _openRequestsContainer);
			BuildRequests(RequestState.Closed, _closedRequestsContainer);
		}
		
		private void BuildRequests(RequestState RequestState, PlaceHolder BladeContainer)
		{
			DataTable requestTestTable = GetRequests(RequestState);
			foreach (DataRow request in requestTestTable.Rows)
			{
				MasterRequestBlade requestBlade;
				requestBlade = LoadControl(@"/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
				requestBlade.RequestId = request["request_id"].ToString();
				// TODO: expand blade if specific requestId in url

				BladeContainer.Controls.Add(requestBlade);
			}			
		}

		static DataTable GetRequests(RequestState RequestState)
		{
			DataTable table = new DataTable();
			table.Columns.Add("request_id", typeof(string));
			table.Columns.Add("affected_end_user_name", typeof(string));
			table.Columns.Add("overall_request_status", typeof(string));
			table.Columns.Add("last_updated_date", typeof(string));
			
			if (RequestState == RequestState.Open)
			{
				table.Rows.Add("12345", "User One", "Open", "Feb. 10, 2010");
				table.Rows.Add("54321", "User Two", "Open", "Jan. 3, 2010");
			}

			if (RequestState == RequestState.Closed)
			{
				table.Rows.Add("98544", "User One", "Closed", "Jan. 10, 2010");
				table.Rows.Add("96554", "User Two", "Closed", "Jan. 3, 2010");
			}			
			
			return table;
		}
	}
}