﻿using System;
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
		public string AffectedEndUserName { get; set; }
		public string OverallRequestStatus { get; set; }
		public string LastUpdatedDate { get; set; }
	
		protected void Page_Load(object sender, EventArgs e)
		{
			PopulateUserInfo();
			LoadReadOnlyRequestView();
			LoadRequestTrackingView();
			
			try 
			{
				switch ((Role)Page.Session["SNAPUserRole"])
				{
					case Role.ApprovingManager:
						LoadApprovingManagerView();
						break;

					case Role.AccessTeam:
						LoadAccessTeamView();
						break;

					case Role.SuperUser:
						LoadApprovingManagerView();
						LoadAccessTeamView();
						break;

					default:
						break;
				}
			} 
			catch (Exception ex)
			{
				// TODO: if session not set, redirect to login?  throw message to user?  set role to user?
			}

		}
		
		private void PopulateUserInfo()
		{
			_affectedEndUserName.Text = AffectedEndUserName;
			_overallRequestStatus.Text = OverallRequestStatus;
			_lastUpdatedDate.Text = LastUpdatedDate;
			_requestId.Text = RequestId;
		}
		
		private void LoadReadOnlyRequestView()
		{
			ReadOnlyRequestView readOnlyRequestView;

			readOnlyRequestView = LoadControl(@"/Controls/ReadOnlyRequestView.ascx") as ReadOnlyRequestView;
			readOnlyRequestView.RequestId = RequestId.ToString();
			_readOnlyRequestViewContainer.Controls.Add(readOnlyRequestView);		
		}
		
		private void LoadRequestTrackingView()
		{
			RequestTrackingView requestTrackingView;

			requestTrackingView = LoadControl(@"/Controls/RequestTrackingView.ascx") as RequestTrackingView;
			requestTrackingView.RequestId = RequestId.ToString();
			_requestTrackingViewContainer.Controls.Add(requestTrackingView);		
		}
		
		private void LoadAccessTeamView() {}
		
		private void LoadApprovingManagerView() {}
	}
}