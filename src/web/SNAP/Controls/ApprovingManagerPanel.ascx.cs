using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class ApprovingManagerPanel : System.Web.UI.UserControl
	{
		public string RequestId { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)
		{
            _approve.Attributes.Add("onclick","return ApproverActions(this,'" + RequestId + "','" + (byte)WorkflowAction.Approved + "');");
            _requestChange.Attributes.Add("onclick", "ApproverActions(this,'" + RequestId + "','" + (byte)WorkflowAction.Change + "');");
            _requestChange.Disabled = true;
            _deny.Attributes.Add("onclick", "ApproverActions(this,'" + RequestId + "','" + (byte)WorkflowAction.Denied + "');");
            _deny.Disabled = true;
            _currentUserDisplayName.Value = SnapSession.CurrentUser.FullName;
            _currentUserId.Value = SnapSession.CurrentUser.LoginId;
        }
	}
}