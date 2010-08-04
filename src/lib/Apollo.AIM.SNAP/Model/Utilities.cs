using System;
using System.Web;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Model
{
	class Utilities
	{
		public static string WebRootUrl
		{
			// TODO: make this dynamic
			get { return ConfigurationManager.AppSettings["EnvironmentPath"]; }
		}

		public static string ImagePath
		{
			get { return ConfigurationManager.AppSettings["ImagePath"]; }
		}

		public static string AbsolutePath
		{
			get
			{
				try
				{
                    //if (HttpContext.Current != null)
					    return HttpRuntime.AppDomainAppPath;  // C:\Apollo\idm\snap\trunk\release\build\src\web\SNAP\EmailTemplate

				    //return string.Empty;
				}
				catch (Exception ex)
				{
                    //Logger.Fatal("Utilities - AbsolutePath, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
					return string.Empty;
				}
			}
		}		

	}
}
