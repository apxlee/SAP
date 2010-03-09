using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Controls;

namespace Apollo.AIM.SNAP.Web.Common
{
    public class ViewBaseUtilities
    {

		public static DataTable GetRequests(RequestState RequestState)
		{
            DataTable table = new DataTable();
            table.Columns.Add("request_id", typeof(string));
			table.Columns.Add("affected_end_user_name", typeof(string));
			table.Columns.Add("overall_request_status", typeof(string));
			table.Columns.Add("last_updated_date", typeof(string));
			table.Columns.Add("is_selected", typeof(bool));

            

           	if (RequestState == RequestState.Open)
		    {
                var reqDetails = Common.Request.Details;

			    //table.Rows.Add("12345", "User One", "Open", "Feb. 10, 2010", false);
			    //table.Rows.Add("54321", "User Two", "Open", "Jan. 3, 2010", true);

                foreach (usp_open_my_request_detailsResult list in reqDetails)
                {
                    table.Rows.Add(list.pkId, list.userDisplayName.StripTitleFromUserName()
						, Convert.ToString((WorkflowState)Enum.Parse(typeof(WorkflowState), list.statusEnum.ToString())).StripUnderscore()
						, list.createdDate.ToString("MMM d, yyyy"), false);
                    // is this "last updated date" or "created date"?
                }
		    }

			if (RequestState == RequestState.Closed)
			{
				//table.Rows.Add("98544", "User One", "Closed", "Jan. 10, 2010", false);
				//table.Rows.Add("96554", "User Two", "Closed", "Jan. 3, 2010", false);
			}			
			
			return table;
		}
    }
}
