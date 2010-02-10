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
	public partial class MasterRequestBlade : System.Web.UI.UserControl
	{
		public string RequestId { get; set; }
	
		protected void Page_Load(object sender, EventArgs e)
		{
			DataTable masterBladeTestTable = GetMasterBlade();
			
			foreach (DataRow userInfo in masterBladeTestTable.Rows)
			{
				//Label affectedEndUserName = (Label)WebUtilities.FindControlRecursive(requestBlade, "_affectedEndUserName");
				_affectedEndUserName.Text = userInfo["affected_end_user_name"].ToString();

				//Label overallRequestStatus = (Label)WebUtilities.FindControlRecursive(requestBlade, "_overallRequestStatus");
				_overallRequestStatus.Text = userInfo["overall_request_status"].ToString();

				//Label lastUpdatedDate = (Label)WebUtilities.FindControlRecursive(requestBlade, "_lastUpdatedDate");
				_lastUpdatedDate.Text = userInfo["last_updated_date"].ToString();

				//Label requestId = (Label)WebUtilities.FindControlRecursive(requestBlade, "_requestId");
				_requestId.Text = userInfo["request_id"].ToString();

				// pass reqId into RORV, populate repeater (add access team notes to repeater as needed, read role from session?)
				// 
				ReadOnlyRequestView readOnlyRequestView;
				PlaceHolder readOnlyRequestViewContainer;

				readOnlyRequestView = LoadControl(@"/Controls/ReadOnlyRequestView.ascx") as ReadOnlyRequestView;
				readOnlyRequestView.RequestId = userInfo["request_id"].ToString();
				//readOnlyRequestViewContainer = (PlaceHolder)WebUtilities.FindControlRecursive(this, "_readOnlyRequestViewContainer");
				_readOnlyRequestViewContainer.Controls.Add(readOnlyRequestView);

				// TODO: if role = access team, then laod AccessTeamView into placeholder

				RequestTrackingView requestTrackingView;
				PlaceHolder requestTrackingViewContainer;

				requestTrackingView = LoadControl(@"/Controls/RequestTrackingView.ascx") as RequestTrackingView;
				requestTrackingView.RequestId = userInfo["request_id"].ToString();
				//requestTrackingViewContainer = (PlaceHolder)WebUtilities.FindControlRecursive(this, "_requestTrackingViewContainer");
				_requestTrackingViewContainer.Controls.Add(requestTrackingView);

				// TODO: If role = approving manager, then load ApprovingManagerView into placeholder					
			}		
		}

		static DataTable GetMasterBlade()
		{
			DataTable table = new DataTable();
			table.Columns.Add("affected_end_user_name", typeof(string));
			table.Columns.Add("overall_request_status", typeof(string));
			table.Columns.Add("last_updated_date", typeof(string));
			table.Columns.Add("request_id", typeof(string));

			table.Rows.Add("User One", "Status One", "Feb. 12, 2010", "12345");
			table.Rows.Add("User Two", "Status Two", "Feb. 13, 2010", "54321");
			return table;
		}				
	}
}