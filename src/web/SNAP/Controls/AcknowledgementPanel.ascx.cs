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
        public WorkflowState AccessTeamState { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
            _acknowledge.Attributes.Add("onclick", "AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowAction.Ack + "');");
            _requestChange.Attributes.Add("onclick", "AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowAction.Change + "');");
            _deny.Attributes.Add("onclick", "AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowAction.Denied + "');");
            _cancel.Attributes.Add("onclick", "AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowAction.Cancel + "');");
            _radioCancel.Attributes.Add("onclick", "changeDenyCancelClick(this);");
            _radioChange.Attributes.Add("onclick", "changeDenyCancelClick(this);");
            _radioDeny.Attributes.Add("onclick", "changeDenyCancelClick(this);");

            switch (AccessTeamState)
            {
                case WorkflowState.Pending_Acknowledgement:
                    _radioCancel.Disabled = true;
                    _radioChange.Disabled = true;
                    _radioDeny.Disabled = true;
                    break;
                case WorkflowState.Pending_Workflow:
                    _acknowledge.Disabled = true;
                    break;
                default:
                    _acknowledge.Disabled = true;
                    _radioChange.Disabled = true;
                    _requestChange.Disabled = true;
                    _deny.Disabled = true;
                    _cancel.Disabled = true;
                    break;
            }
		}
	}
}