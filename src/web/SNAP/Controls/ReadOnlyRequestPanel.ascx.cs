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
		public string RequestId { get; set; }
			
		protected void Page_Load(object sender, EventArgs e)
		{
			DataTable requestTestTable = GetDetails();
			_readOnlyRequestDetails.DataSource = requestTestTable;
			_readOnlyRequestDetails.DataBind();
			
			BuildAccessComments(Role.User);
		}
		
		private void BuildAccessComments(Role UserRole)
		{
			DataTable accessCommentsTable = GetAccessComments( (Role)Page.Session["SNAPUserRole"] );
			StringBuilder accessComments = new StringBuilder();
			
			foreach (DataRow comment in accessCommentsTable.Rows)
			{
				switch ((Role)comment["audience"])
				{
					case Role.AccessTeam:
					case Role.SuperUser:
						// can see everything
						break;
					
					case Role.ApprovingManager :
						// can see user + approving mgr
						break;
					
					case Role.User :
					default :
						break;
				}
				
				accessComments.AppendFormat("<p><u>{0}&nbsp;for&nbsp;{1}</u><br />{2}</p>"
					, comment["comment_date"].ToString()
					, comment["comment"].ToString() );
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
			DataTable table = new DataTable();
			table.Columns.Add("audience", typeof(CommentsType));
			table.Columns.Add("comment_date", typeof(string));
			table.Columns.Add("comment", typeof(string));
			
			table.Rows.Add(CommentsType.Access_Notes_User, "Feb. 16, 2010", "Comments one and this could be html.");
			table.Rows.Add(CommentsType.Access_Notes_User, "Feb. 12, 2010", "More comments about something.");
			table.Rows.Add(CommentsType.Access_Notes_User, "Feb. 11, 2010", "I have alot of comments to make about this subject.");
			table.Rows.Add(CommentsType.Access_Notes_ApprovingManager, "Feb. 16, 2010", "Approving manager only.");
			
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
