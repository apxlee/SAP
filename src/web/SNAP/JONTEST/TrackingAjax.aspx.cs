using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web.JONTEST
{
	public partial class TrackingAjax : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		[WebMethod]
		public static List<string> GetAllTrackingData(string requestId)
		{
			return TrackingUI.GetTrackingBlades(requestId);
		}
	}
}
