using System;
using System.Security.Cryptography;
using System.Threading;
using Apollo.AIM.SNAP.Web.Common;

namespace Apollo.AIM.SNAP.Web
{
	public partial class AppError : SnapPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(Request.QueryString[QueryStringConstants.ERROR_REASON]))
			{
				string reason = Request.QueryString[QueryStringConstants.ERROR_REASON];
				
				switch (reason) // TODO: make these cases enum or constants
				{
					case "wrongRole":
						_errorMessage.Text = "<p><strong>Unauthorized View</strong><br/>"
							+ "It appears you are attempting to view a page that is not available to your role.<br/>"
							+ "Please select one of the links in the header to continue.</p>";
						break;

					case "badRequestId":
						_errorMessage.Text = "<p><strong>Bad Request Id</strong><br/>"
							+ "It appears the Request Id you have linked to is no longer in the system.<br/>"
							+ "Please select one of the links in the header to continue.</p>";
						break;
					
					case "unhandled":
					default :
						_errorMessage.Text = "<p><strong>Unhandled Exception</strong><br/>"
							+ "The application reported an error that was not handled.<br/>"
							+ "Please select one of the links in the header to continue.</p>";
						break;
				}

				SnapSession.ClearSelectedRequestId();
			}
            else
			{

                //http://weblogs.asp.net/scottgu/archive/2010/09/18/important-asp-net-security-vulnerability.aspx

                byte[] delay = new byte[1];
                RandomNumberGenerator prng = new RNGCryptoServiceProvider();

                prng.GetBytes(delay);
                Thread.Sleep((int)delay[0]);

                IDisposable disposable = prng as IDisposable;
                if (disposable != null) { disposable.Dispose(); }

			}

		}
	}
}
