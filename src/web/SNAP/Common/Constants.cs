using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apollo.AIM.SNAP.Web.Common
{
	public static class QueryStringConstants
	{
		public const string DEMONSTRATION_ROLE = "role";
		public const string REQUEST_ID = "requestId";
		//public const string REQUESTED_VIEW_INDEX = "viewIndex"; // remove
		public const string REFERRING_URL = "refurl";
		public const string ERROR_REASON = "errorReason";
	}

	public static class SessionVariables
	{
		public const string CURRENT_USER = "CurrentUser";
		public const string IS_USER_CREATED = "IsUserCreated";
		public const string SELECTED_REQUEST_ID = "SelectedRequestId";
		//public const string REQUESTED_VIEW_INDEX = "RequestedViewIndex"; // remove
		public const string IS_REQUEST_PREPOPULATED = "IsRequestPrepopulated";
		public const string REQUESTED_PAGE = "RequestedPage";
	}

	public static class PageNames
	{
		public const string ACCESS_TEAM = "AccessTeam";
		public const string APP_ERROR = "AppError";
		public const string APP_MAINTENANCE = "AppMaintenance";
		public const string DEFAULT_LOGIN = "Default";
		public const string APPROVING_MANAGER = "MyApprovals";
		public const string USER_VIEW = "MyRequests";
		public const string REQUEST_FORM = "RequestForm";
		public const string SEARCH = "Search";
		public const string SUPPORT = "Support";
	}
}
