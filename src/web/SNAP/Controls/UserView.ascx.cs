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
			// select case of user who should be viewing to determine headings
			// build dataset of all master blade rows
			// build master blades
			// expand blade if specific requestId in url
			// Request Type Headings: OPEN, CLOSED, FILTERED

			DataTable requestTestTable = GetRequests();

			foreach (DataRow request in requestTestTable.Rows)
			{
				MasterRequestBlade requestBlade;
				requestBlade = LoadControl(@"/Controls/MasterRequestBlade.ascx") as MasterRequestBlade;
				requestBlade.RequestId = request["request_id"].ToString();

				this.Controls.Add(requestBlade);	
			}	
		}

		static DataTable GetRequests()
		{
			DataTable table = new DataTable();
			table.Columns.Add("request_id", typeof(string));

			table.Rows.Add("12345");
			table.Rows.Add("54321");
			return table;
		}		
	}
}