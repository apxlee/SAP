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

		public static void ClearSelectedRequestId()
		{
			Page currentPage = HttpContext.Current.Handler as Page;
			currentPage.Session.Remove(SessionVariables.SELECTED_REQUEST_ID);
			currentPage.Session.Remove(SessionVariables.USE_SELECTED_REQUEST_ID);
		}

		public static bool IsUserCreated 
		{
			get
			{
				Page currentPage = HttpContext.Current.Handler as Page;

				if (currentPage.Session[SessionVariables.IS_USER_CREATED] != null) 
				{
					//TODO REMOVE Logger.Info(WebUtilities.GetTimestamp() + "SnapSession > IsUserCreated (41): true\r\n"); 
					return (bool)currentPage.Session[SessionVariables.IS_USER_CREATED];
				}
				else
				{
					// TODO REMOVE Logger.Info(WebUtilities.GetTimestamp() + "SnapSession > IsUserCreated (46): false\r\n"); 
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

        //public static bool UseSelectedRequestId { get; set; }
		public static bool UseSelectedRequestId
		{
			get
			{
				Page currentPage = HttpContext.Current.Handler as Page;

				if (currentPage.Session[SessionVariables.USE_SELECTED_REQUEST_ID] != null)
				{
					return (bool)currentPage.Session[SessionVariables.USE_SELECTED_REQUEST_ID];
				}
				else
				{
					return false;
				}
			}
			set
			{
				Page currentPage = HttpContext.Current.Handler as Page;
				currentPage.Session[SessionVariables.USE_SELECTED_REQUEST_ID] = value;
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
				//TODO REMOVE Logger.Info(WebUtilities.GetTimestamp() + "SnapSession > RequestedPage (getter:93): " 
				//	+ (string)currentPage.Session[SessionVariables.REQUESTED_PAGE] + "\r\n"); 
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
