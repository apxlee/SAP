using System;
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
			PopulateRequestDemographics();
			
			DataTable requestTestTable = ViewBaseUtilities.GetRequestDetails(Convert.ToInt32(RequestId));
			_readOnlyRequestDetails.DataSource = requestTestTable;
			_readOnlyRequestDetails.DataBind();
			
			BuildAccessComments(SnapSession.CurrentUser.CurrentRole);
		}

		private void PopulateRequestDemographics()
		{
			var requestDemographics = Common.Request.Details(RequestState).Single(x => x.pkId.ToString() == RequestId);

			_affectedEndUserTitle.Text = requestDemographics.userTitle;
			_managerName.Text = requestDemographics.managerDisplayName;
			_adManagerName.Text = CompareManagerName(requestDemographics.userId, requestDemographics.managerUserId);
			//_requestorName.Text = GetFullNameFromAD(requestDemographics.submittedBy);
		    _requestorName.Text = GetFullNameFromAD(requestDemographics.submittedBy);
		}

		private string GetFullNameFromAD(string userId)
		{
			// TODO: should there ever be a record without 'Requestor'?  Log this as error?
			if (string.IsNullOrEmpty(userId)) { return "<span class=\"csm_error_text\">Unknown</strong></span>"; }
			else
			{
				try
				{
					ADUserDetail userDetail = CA.DirectoryServices.GetUserByLoginName(userId);
					return userDetail.FirstName + " " + userDetail.LastName;
				}
				catch (Exception ex)
				{
					return "<span class=\"csm_error_text\">Unknown (Active Directory Error)</strong></span>";
					// TODO: Logger.Error("ReadOnlyRequestPanel > GetFullNameFromAD", ex);
				}
			}
		}

        private string CompareManagerName(string userId, string mgrUserId)
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

		private void BuildAccessComments(Role userRole)
		{
			DataTable accessCommentsTable = GetAccessComments(userRole);

			if (accessCommentsTable.Rows.Count > 0)
			{
				DataRow[] filteredRows;
				StringBuilder accessComments = new StringBuilder();
				string commentFilter = string.Empty;

				switch (userRole)
				{
					case Role.AccessTeam:
					case Role.SuperUser:
						commentFilter = string.Format("IN ({0},{1},{2})"
							, (int)CommentsType.Access_Notes_Requestor
							, (int)CommentsType.Access_Notes_ApprovingManager
							, (int)CommentsType.Access_Notes_AccessTeam);
						break;

					case Role.ApprovingManager:
						commentFilter = string.Format("IN ({0},{1})"
							, (int)CommentsType.Access_Notes_Requestor
							, (int)CommentsType.Access_Notes_ApprovingManager);
						break;

					case Role.Requestor:
					default:
						commentFilter = string.Format("IN ({0})"
							, (int)CommentsType.Access_Notes_Requestor);
						break;
				}
				
				filteredRows = accessCommentsTable.Select("audience " + commentFilter);
				foreach (DataRow comment in filteredRows)
				{
					accessComments.AppendFormat("<p><u>{0}&nbsp;at&nbsp;{1}&nbsp;for&nbsp;{2}</u><br />{3}</p>"
						, Convert.ToDateTime(comment["comment_date"].ToString()).ToString("MMM d, yyyy")
						, Convert.ToDateTime(comment["comment_date"].ToString()).ToString("h:mm tt")
						, MakeFriendlyCommentAudience(  (CommentsType)Convert.ToInt32(comment["audience"].ToString()) )
						, comment["comment"].ToString());
				}

				_accessNotes.Text = accessComments.ToString();
				_accessNotesContainer.Visible = true;
			}
		}
		
		private string MakeFriendlyCommentAudience(CommentsType commentType)
		{
			string audienceName = string.Empty;
			
			switch (commentType)
			{
				case CommentsType.Access_Notes_AccessTeam:
					audienceName = "Access & Identity Management";
					break;
					
				case CommentsType.Access_Notes_ApprovingManager:
					audienceName = "Approving Managers";
					break;
				
				case CommentsType.Access_Notes_SuperUser:
				case CommentsType.Access_Notes_Requestor:
				default:
					audienceName = "Requestor";
					break;
			}
			
			return audienceName;
		}

		private DataTable GetAccessComments(Role userRole)
		{
			DataTable table = new DataTable();
			table.Columns.Add("audience", typeof(string));
			table.Columns.Add("comment_date", typeof(string));
			table.Columns.Add("comment", typeof(string));

            var reqComments = Common.Request.Comments(RequestState);
            var comments = reqComments.Where(x => x.requestId.ToString() == RequestId).ToList();
            
            foreach (usp_open_my_request_commentsResult result in comments)
            {
				table.Rows.Add(result.commentTypeEnum, result.createdDate, result.commentText);
			}

			return table;
		}
	}
}
