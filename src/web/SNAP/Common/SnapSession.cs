using System.Web;
using System.Web.UI;
using Apollo.CA.Logging;

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
		
		public static void ClearCurrentUser()
		{
			Page currentPage = HttpContext.Current.Handler as Page;
			currentPage.Session.Remove(SessionVariables.CURRENT_USER);
			currentPage.Session.Remove(SessionVariables.IS_USER_CREATED);
			currentPage.Session.Remove(SessionVariables.IS_REQUEST_PREPOPULATED);
		}

		public static bool IsUserCreated 
		{
			get
			{
				Page currentPage = HttpContext.Current.Handler as Page;

				if (currentPage.Session[SessionVariables.IS_USER_CREATED] != null) 
				{
					Logger.Info("SnapSession > IsUserCreated (41): true/n"); //TODO REMOVE
					return (bool)currentPage.Session[SessionVariables.IS_USER_CREATED];
				}
				else
				{
					Logger.Info("SnapSession > IsUserCreated (46): false/n"); // TODO REMOVE
					return false;
				}
			}
		}
		
		public static bool IsRequestPrePopulated
		{
			get
			{
				Page currentPage = HttpContext.Current.Handler as Page;

				if (currentPage.Session[SessionVariables.IS_REQUEST_PREPOPULATED] != null)
				{
					return (bool)currentPage.Session[SessionVariables.IS_REQUEST_PREPOPULATED];
				}
				else
				{
					return false;
				}
			}
			set
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				currentPage.Session[SessionVariables.IS_REQUEST_PREPOPULATED] = value;
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
		
		public static string RequestedPage
		{
			get
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				Logger.Info("SnapSession > RequestedPage GET (93):" + (string)currentPage.Session[SessionVariables.REQUESTED_PAGE] + "/n"); //TODO REMOVE
				return (string)currentPage.Session[SessionVariables.REQUESTED_PAGE];
			}
			set
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				currentPage.Session[SessionVariables.REQUESTED_PAGE] = value;
			}
		}
	}
}
