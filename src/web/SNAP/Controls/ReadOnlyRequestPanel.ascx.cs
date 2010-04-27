﻿using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.CA;
using Apollo.AIM.SNAP.Web.Common;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class ReadOnlyRequestPanel : System.Web.UI.UserControl
	{
		// TODO: if no reqId, will raise error when attempting to 'get'
		//
		public string RequestId { get; set; }
        public RequestState RequestState { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
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

		private DataTable GetDetails()
		{
			DataTable table = new DataTable();
            table.Columns.Add("label", typeof (string));
            table.Columns.Add("value", typeof (string));

            var reqDetails = Common.Request.Details(RequestState);
            var reqUserTexts = Common.Request.UserTexts(RequestState);

            var reqDetail = reqDetails.Single(x => x.pkId.ToString() == RequestId);
            var reqUserText = reqUserTexts.Where(x => x.requestId.ToString() == RequestId).ToList();

            _affectedEndUserTitle.Text = reqDetail.userTitle;
            _managerName.Text = reqDetail.managerDisplayName.ToString();
            _adManagerName.Text = CompareManagerName(reqDetail.userId, reqDetail.managerUserId);
			_requestorName.Text = GetFullNameFromAD(reqDetail.submittedBy);
            
			// TODO: get request form info and match to request details 
            
            table.Rows.Add("Windows Servers", userText(reqUserText, 2));
                //reqUserText.Single(x => x.access_details_formId == 2).userText); ;
            table.Rows.Add("Linux/Unix Servers", userText(reqUserText, 3));
                //reqUserText.Single(x => x.access_details_formId == 3).userText);
            table.Rows.Add("Network Shares", userText(reqUserText, 4));
                //reqUserText.Single(x => x.access_details_formId == 4).userText);
            table.Rows.Add("Justification", userText(reqUserText, 5));
			//reqUserText.Single(x => x.access_details_formId == 5).userText);

			#region Mockup Stuff
			/*
		    table.Rows.Add("Title", "Network Engineer II");
			table.Rows.Add("Manager Name", "Bob Jones");
			table.Rows.Add("AD Manager Name", "Sally Kirkland");
			table.Rows.Add("Requestor", "Larry Lutein");
			table.Rows.Add("Windows Servers", "Empty");
			table.Rows.Add("Linux/Unix Servers", "Empty");
			table.Rows.Add("Network Shares", "//MSTGG01/Enrollment");
			table.Rows.Add("Justification", "I am required to have access to complete my tasks under EC Level 4. I am required to have access to complete my tasks under EC Level 4. I am required to have access to complete my tasks under EC Level 4.");
             */
			#endregion

			return table;
		}

		private string GetFullNameFromAD(string userId)
		// TODO: should there ever be a record without 'Requestor'?  Log this as error?
		{
			if (string.IsNullOrEmpty(userId)) { return "<span class=\"csm_error_text\">Unknown</strong></span>"; }
			else
			{
				ADUserDetail userDetail = CA.DirectoryServices.GetUserByLoginName(userId);
				return userDetail.FirstName + " " + userDetail.LastName;
			}
		}

        string CompareManagerName(string userId, string mgrUserId)
        {
			try
			{
				ADUserDetail userDetail = Apollo.AIM.SNAP.CA.DirectoryServices.GetUserByLoginName(userId);
				if (mgrUserId != userDetail.Manager.LoginName)
				{
					return "<span class=\"csm_error_text\">[Active Directory:&nbsp;" + userDetail.ManagerName + "]</span>";
				}
			}
			catch (Exception ex)
			{
				// TODO: Logger.Error("ReadOnlyRequestPanel > CompareManagerName", ex);
			}

			return string.Empty;
        }

        string userText(List<SNAP_Access_User_Text> list, int formId)
        {
            string txt = string.Empty;
            try
            {
                txt = list.Single(x => x.access_details_formId == formId).userText;
            }
            catch
            {
            }

            return txt;
        }

		DataTable GetAccessComments(Role UserRole)
		{
			// NOTE: data request returns 'friendly' name for the audience, not the type
			//
			DataTable table = new DataTable();
			table.Columns.Add("audience", typeof(string));
			table.Columns.Add("comment_date", typeof(string));
			table.Columns.Add("comment", typeof(string));

   
            var reqComments = Common.Request.Comments(RequestState);
            var comments = reqComments.Where(x => x.requestId.ToString() == RequestId).ToList();
            foreach (usp_open_my_request_commentsResult result in comments)
                table.Rows.Add(result.commentTypeEnum, result.createdDate, result.commentText);



            // TODO
            /* will filter by on row later
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
            */

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
