using System.Web;
using System.Web.UI;

namespace Apollo.AIM.SNAP.Web.Common
{
	class SnapSession
	{
		public static SnapUser CurrentUser
		{
			get
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				// TODO: test to make sure snapuser exists, return empty user?
				return (SnapUser)currentPage.Session[SessionVariables.CURRENT_USER];
			} 
			set
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				currentPage.Session[SessionVariables.CURRENT_USER] = value;
				currentPage.Session[SessionVariables.IS_USER_CREATED] = true;
			}
		}
		
		public static bool IsUserCreated 
		{
			get
			{
				Page currentPage = HttpContext.Current.Handler as Page;

				if (currentPage.Session[SessionVariables.IS_USER_CREATED] != null) 
				{
					return (bool)currentPage.Session[SessionVariables.IS_USER_CREATED];
				}
				else
				{
					return false;
				}
			}
		}
		
		public static string SelectedRequestId
		{
			get
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				return (string)currentPage.Session[SessionVariables.SELECTED_REQUEST_ID];
			}
			set
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				currentPage.Session[SessionVariables.SELECTED_REQUEST_ID] = value;
			}
		}
	}
}
