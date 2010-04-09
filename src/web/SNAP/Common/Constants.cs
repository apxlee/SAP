﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apollo.AIM.SNAP.Web.Common
{
	public static class QueryStringConstants
	{
		public const string DEMONSTRATION_ROLE = "role";
		public const string REQUEST_ID = "requestId";
		public const string REQUESTED_VIEW_INDEX = "viewIndex";
		public const string REFERRING_URL = "refurl";
		public const string ERROR_REASON = "errorReason";
	}

	public static class SessionVariables
	{
		public const string CURRENT_USER = "CurrentUser";
		public const string IS_USER_CREATED = "IsUserCreated";
	}	
}
