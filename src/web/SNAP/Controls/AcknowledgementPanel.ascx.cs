using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class AcknowledgementPanel : System.Web.UI.UserControl
	{
        public string RequestId { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
            _acknowledge.Attributes.Add("onclick", "return AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowState.Pending_Workflow + "');");
            _requestChange.Attributes.Add("onclick", "AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowState.Change_Requested + "');");
            _requestChange.Disabled = true;
            _deny.Attributes.Add("onclick", "AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowState.Closed_Denied + "');");
            _deny.Disabled = true;
            _cancel.Attributes.Add("onclick", "AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowState.Closed_Cancelled + "');");
            _cancel.Disabled = true;
		}
	}
}