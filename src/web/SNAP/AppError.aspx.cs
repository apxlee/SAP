﻿using System;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web
{
	public partial class AppError : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(Request.QueryString[QueryStringConstants.ERROR_REASON]))
			{
				string reason = Request.QueryString[QueryStringConstants.ERROR_REASON];
				
				switch (reason)
				{
					case "wrongRole":
						_errorMessage.Text = "<p><strong>Unauthorized View</strong><br/>"
							+ "It appears you are attempting to view a page that is not available to your role.<br/>"
							+ "Please select one of the links in the header to continue.</p>";
						_errorMessage.CssClass = "csm_error_text";
						break;
					
					default :
						break;
				}
			}

		}
	}
}