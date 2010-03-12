﻿using System;
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
			if (WebUtilities.CurrentViewIndex == ViewIndex.request_form)
			{
				BuildUserView();
			}
		}
		
		private void BuildUserView()
		{
            var requesterLoader = new Common.MyRequestLooder();
		    requesterLoader.Load();

            ViewBaseUtilities.BuildRequests(this.Page, RequestState.Open, _openRequestsContainer, _nullDataMessage_ClosedRequests, false);
            //ViewBaseUtilities.BuildRequests(this.Page, RequestState.Closed, _closedRequestsContainer, _nullDataMessage_ClosedRequests, true);
            ViewBaseUtilities.BuildRequests(this.Page, RequestState.Closed, _closedRequestsContainer, _nullDataMessage_ClosedRequests, false);
		}
		
	}

}