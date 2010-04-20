﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class ApprovingManagerPanel : System.Web.UI.UserControl
	{
		public string RequestId { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)
		{
            _approve.Attributes.Add("onclick","return ApproverActions('" + _approve.ClientID + "','" + RequestId + "','" + (byte)WorkflowState.Approved + "');");
            _requestChange.Attributes.Add("onclick","ApproverActions('" + _requestChange.ClientID + "','" + RequestId + "','" + (byte)WorkflowState.Change_Requested + "');");
            _requestChange.Disabled = true;
            _deny.Attributes.Add("onclick", "ApproverActions('" + _deny.ClientID + "','" + RequestId + "','" + (byte)WorkflowState.Closed_Denied + "');");
            _deny.Disabled = true;
        }
	}
}