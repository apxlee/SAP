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
			// TODO: load requestId from public property, find userinfo and populate...
			//
			_affectedEndUserName.Text = "USER";
			_overallRequestStatus.Text = "STATUS";
			_lastUpdatedDate.Text = "UPDATED";
			_requestId.Text = RequestId.ToString();

			// pass reqId into RORV, populate repeater (add access team notes to repeater as needed, read role from session?)
			// 
			ReadOnlyRequestView readOnlyRequestView;

			readOnlyRequestView = LoadControl(@"/Controls/ReadOnlyRequestView.ascx") as ReadOnlyRequestView;
			readOnlyRequestView.RequestId = RequestId.ToString();
			_readOnlyRequestViewContainer.Controls.Add(readOnlyRequestView);

			// TODO: if role = access team, then load AccessTeamView into placeholder

			RequestTrackingView requestTrackingView;

			requestTrackingView = LoadControl(@"/Controls/RequestTrackingView.ascx") as RequestTrackingView;
			requestTrackingView.RequestId = RequestId.ToString();
			_requestTrackingViewContainer.Controls.Add(requestTrackingView);

			// TODO: If role = approving manager, then load ApprovingManagerView into placeholder					
		}
	}
}