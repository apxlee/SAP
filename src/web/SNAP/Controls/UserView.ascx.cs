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
	public partial class UserView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			// select case of user who should be viewing to determine headings
			// build dataset of all master blade rows
			// build master blades
			
			DataTable masterBladeTestTable = GetMasterBlade();
			
			foreach (DataRow userInfo in masterBladeTestTable.Rows)
			{
				MasterRequestBlade requestBlade;

				requestBlade = LoadControl(@"/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
				
				Label affectedEndUserName = (Label)WebUtilities.FindControlRecursive(requestBlade, "_affectedEndUserName");
				affectedEndUserName.Text = userInfo["affected_end_user_name"].ToString();
				
				Label overallRequestStatus = (Label)WebUtilities.FindControlRecursive(requestBlade, "_overallRequestStatus");
				overallRequestStatus.Text = userInfo["overall_request_status"].ToString();
				
				Label lastUpdatedDate = (Label)WebUtilities.FindControlRecursive(requestBlade, "_lastUpdatedDate");
				lastUpdatedDate.Text = userInfo["last_updated_date"].ToString();
				
				Label requestId = (Label)WebUtilities.FindControlRecursive(requestBlade, "_requestId");
				requestId.Text = userInfo["request_id"].ToString();
				
				// find readonlyrequestview placeholder, load rorv into it
				// populate repeater (add access team notes to repeater as needed)
				// 
				ReadOnlyRequestView readOnlyRequestView; 
				PlaceHolder readOnlyRequestViewContainer;

				readOnlyRequestView = LoadControl(@"/Controls/ReadOnlyRequestView.ascx") as ReadOnlyRequestView;
				readOnlyRequestViewContainer = (PlaceHolder)WebUtilities.FindControlRecursive(requestBlade, "_readOnlyRequestView");
				readOnlyRequestViewContainer.Controls.Add(readOnlyRequestView);
				
				this.Controls.Add(requestBlade);	
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