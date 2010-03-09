using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class UserView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            var loader = new Common.MyOpenRequestLooder();
		    loader.Load();

			BuildRequests(RequestState.Open, _openRequestsContainer, false);
			BuildRequests(RequestState.Closed, _closedRequestsContainer, true);
		}
		
		private void BuildRequests(RequestState RequestState, PlaceHolder BladeContainer, bool IsNullRecordTest)
		{
			DataTable requestTestTable = ViewBaseUtilities.GetRequests(RequestState);

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

	}
}