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
	public partial class UserView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			// TODO: utility to find AD group, look into DB for approving rows to determine role
			Page.Session["SNAPUserRole"] = Role.SuperUser;
		
			// select case of user who should be viewing to determine headings
			// build dataset of all master blade rows
			// build master blades
			// expand blade if specific requestId in url
			// Request Type Headings: [users] OPEN, CLOSED / [access team] FILTERED / [approvers] PENDING, APPROVED
			
			// if clicked on myRequests view, then load open/closed, else look to see if 

			try
			{
				switch ((Role)Page.Session["SNAPUserRole"])
				{
					case Role.ApprovingManager:
						LoadApprovingManagerHeadings();
						break;

					case Role.AccessTeam:
						//LoadAccessTeamFilter();
						//LoadUserHeadings();
						break;

					case Role.SuperUser:
						//LoadApprovingManagerView();
						//LoadAccessTeamView();
						break;

					default:
						break;
				}
			}
			catch (Exception ex)
			{
				// TODO: if session not set, redirect to login?  throw message to user?  set role to user?
			}
			

			DataTable requestTestTable = GetRequests();
			foreach (DataRow request in requestTestTable.Rows)
			{
				MasterRequestBlade requestBlade;
				requestBlade = LoadControl(@"/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
				requestBlade.RequestId = request["request_id"].ToString();

				this.Controls.Add(requestBlade);	
			}	
		}
		
		private void LoadApprovingManagerHeadings()
		{
			// add pending h1
			// query db for pending requests
			// build masterblades
			
			// add closed 
		}

		static DataTable GetRequests()
		{
			DataTable table = new DataTable();
			table.Columns.Add("request_id", typeof(string));
			table.Columns.Add("affected_end_user_name", typeof(string));
			table.Columns.Add("overall_request_status", typeof(string));
			table.Columns.Add("last_updated_date", typeof(string));

			table.Rows.Add("12345", "User One", "Open", "Feb. 10, 2010");
			table.Rows.Add("54321", "User Two", "Closed", "Jan. 3, 2010");
			return table;
		}		
	}
}