using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class WorkflowBlade : System.Web.UI.UserControl
	{
		public string ActorName { get; set; }
		public string Status { get; set; }
		public string DueDate { get; set; }
		public string CompletedDate { get; set; }
		public string AlternatingCss { get; set; }	
		
		protected void Page_Load(object sender, EventArgs e)
		{

		}
	}
}