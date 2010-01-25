using System.Collections;
using System.Reflection;
using System.Text;
using Apollo.Ultimus.CAP.Model;

namespace OOSPA.includes
{
	using System;
	using System.Configuration;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for ucFooter.
	/// </summary>
	public partial class ucFooter : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
			//lblAssemblyInfo.Text = Global.DLLInfo();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
