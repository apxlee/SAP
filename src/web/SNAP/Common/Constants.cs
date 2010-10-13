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
		public const string REFERRING_URL = "refurl";
		public const string ERROR_REASON = "errorReason";
	}

	public static class SessionVariables
	{
		public const string CURRENT_USER = "CurrentUser";
		public const string IS_USER_CREATED = "IsUserCreated";
		public const string SELECTED_REQUEST_ID = "SelectedRequestId";
		public const string IS_REQUEST_PREPOPULATED = "IsRequestPrepopulated";
		public const string REQUESTED_PAGE = "RequestedPage";
		public const string USE_SELECTED_REQUEST_ID = "UseSelectedRequestId";
	}
}
