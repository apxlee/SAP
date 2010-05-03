﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Apollo.AIM.SNAP.Web.Common;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web
{
	public partial class AccessTeam : SnapPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WebUtilities.RoleCheck(WebUtilities.GetPageName(Page));
		}
	}
}