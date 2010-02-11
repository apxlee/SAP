using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class ReadOnlyRequestView : System.Web.UI.UserControl
	{
		// pass reqId into RORV, populate repeater (add access team notes to repeater as needed, read role from session?)
		// 

	
		// TODO: if no reqId, will raise error when attempting to 'get'
		public string RequestId { get; set; }
	
		protected void Page_Load(object sender, EventArgs e)
		{
		}
	}
}