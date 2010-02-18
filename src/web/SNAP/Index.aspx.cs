using System;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Apollo.CA.Logging;
using Apollo.Ultimus.CAP;
using Apollo.Ultimus.CAP.Model;
using Apollo.Ultimus.Web;

// need to alias namespace because CAP had 'Role' class also
//
using SNAPCommon = Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web
{
	public partial class Index : Page
	{
		protected void Page_Load(object sender, EventArgs e)	
		{
			// TODO: utility to find AD group and/or look into DB for existence of pending approvals to determine role
			Page.Session["SNAPUserRole"] = SNAPCommon.Role.Requestor;
		}
	}
}
