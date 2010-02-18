using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class ReadOnlyRequestPanel : System.Web.UI.UserControl
	{
		// TODO: if no reqId, will raise error when attempting to 'get'
		//
		public string RequestId { get; set; }
			
		protected void Page_Load(object sender, EventArgs e)
		{
			// build details section
			//
			// TODO: make 'AD Manager Name' red, add seperator between heading info and details
			//
			DataTable requestTestTable = GetDetails();
			_readOnlyRequestDetails.DataSource = requestTestTable;
			_readOnlyRequestDetails.DataBind();
			
			BuildAccessComments(Role.Requestor);
		}
		
		private void BuildAccessComments(Role UserRole)
		{
			DataTable accessCommentsTable = GetAccessComments(UserRole);
			StringBuilder accessComments = new StringBuilder();
			
			foreach (DataRow comment in accessCommentsTable.Rows)
			{
				// TODO: move string to config file?
				//
				accessComments.AppendFormat("<p><u>{0}&nbsp;for&nbsp;{1}</u><br />{2}</p>"
					, comment["comment_date"].ToString()
					, comment["audience"].ToString()
					, comment["comment"].ToString());
			}

			_accessNotes.Text = accessComments.ToString();
			_accessNotesContainer.Visible = true;
		}

		static DataTable GetDetails()
		{
			DataTable table = new DataTable();
			table.Columns.Add("label", typeof(string));
			table.Columns.Add("value", typeof(string));

			table.Rows.Add("Title", "Network Engineer II");
			table.Rows.Add("Manager Name", "Bob Jones");
			table.Rows.Add("AD Manager Name", "Sally Kirkland");
			table.Rows.Add("Requestor", "Larry Lutein");
			table.Rows.Add("Windows Servers", "Empty");
			table.Rows.Add("Linux/Unix Servers", "Empty");
			table.Rows.Add("Network Shares", "//MSTGG01/Enrollment");
			table.Rows.Add("Justification", "I am required to have access to complete my tasks under EC Level 4. I am required to have access to complete my tasks under EC Level 4. I am required to have access to complete my tasks under EC Level 4.");

			return table;
		}
		
		static DataTable GetAccessComments(Role UserRole)
		{
			// NOTE: data request returns 'friendly' name for the audience, not the type
			//
			DataTable table = new DataTable();
			table.Columns.Add("audience", typeof(string));
			table.Columns.Add("comment_date", typeof(string));
			table.Columns.Add("comment", typeof(string));

		
				
			switch (UserRole)
			{
				case Role.AccessTeam:
				case Role.SuperUser:
					// where (no filter, can see everything)
					//
					table.Rows.Add("Requestor", "Feb. 16, 2010", "Comments one and this could be html.");
					table.Rows.Add("Requestor", "Feb. 12, 2010", "More comments about something.");
					table.Rows.Add("Access Team", "Feb. 11, 2010", "I have alot of comments to make about this subject.");
					table.Rows.Add("Approving Manager", "Feb. 16, 2010", "Approving manager only.");
					break;
				
				case Role.ApprovingManager :
					// where (UserRole == Role.Requestor || UserRole == Role.ApprovingManager)
					//
					table.Rows.Add("Requestor", "Feb. 16, 2010", "Comments one and this could be html.");
					table.Rows.Add("Requestor", "Feb. 12, 2010", "More comments about something.");
					table.Rows.Add("Requestor", "Feb. 11, 2010", "I have alot of comments to make about this subject.");
					table.Rows.Add("Approving Manager", "Feb. 16, 2010", "Approving manager only.");
					break;
				
				case Role.Requestor :
				default :
					// where (UserRole == Role.Requestor)
					//
					table.Rows.Add("Requestor", "Feb. 16, 2010", "Comments one and this could be html.");
					table.Rows.Add("Requestor", "Feb. 12, 2010", "More comments about something.");
					table.Rows.Add("Requestor", "Feb. 11, 2010", "I have alot of comments to make about this subject.");
					break;
			}		

			return table;
		}

		//public void _readOnlyRequestDetails_OnItemDataBound(object Sender, RepeaterItemEventArgs e)
		//{
		//    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
		//    {
		//        DataRowView myRow = (DataRowView)e.Item.DataItem;

		//        if (myRow.Row["comment_type"].ToString() == CommentsType.Access_Notes_ApprovingManager.ToString())
		//        {
		//            e.Item.Visible = false;
		//        }
		//    }
		//}		
	}
}
