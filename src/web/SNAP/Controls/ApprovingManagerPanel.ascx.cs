using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class ApprovingManagerPanel : System.Web.UI.UserControl
	{
		public string RequestId { get; set; }
		
		protected void Page_Load(object sender, EventArgs e)
		{
			//Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "key", ApproveScript());
			
			_approve.OnClientClick = "return ApproveRequest(\'" + RequestId + "\');";
		}
	}
}