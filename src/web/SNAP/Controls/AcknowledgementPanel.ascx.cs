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
            if (AccessTeamState != WorkflowState.Pending_Acknowledgement)
            {
                _acknowledge.Disabled = true;
            }
            else
            {
                _radioCancel.Disabled = true;
                _radioChange.Disabled = true;
                _radioDeny.Disabled = true;
            }

            _acknowledge.Attributes.Add("onclick", "AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowAction.Ack + "');");

            _requestChange.Attributes.Add("onclick", "AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowAction.Change + "');");
            _deny.Attributes.Add("onclick", "AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowAction.Denied + "');");
            _cancel.Attributes.Add("onclick", "AccessTeamActions(this,'" + RequestId + "','" + (byte)WorkflowAction.Cancel + "');");
                
            _radioCancel.Attributes.Add("onclick", "changeDenyCancelClick(this);");
            _radioChange.Attributes.Add("onclick", "changeDenyCancelClick(this);");
            _radioDeny.Attributes.Add("onclick", "changeDenyCancelClick(this);");

            _requestChange.Disabled = true;
            _deny.Disabled = true;
            _cancel.Disabled = true;
		}
	}
}