using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class AccessCommentsPanel : System.Web.UI.UserControl
	{
        public string RequestId { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
            _submitComments.Attributes.Add("onclick", "return AccessComments(this,'" + RequestId + "');");
            _submitComments.Disabled = true;
		}
	}
}