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

		public static string AbsolutePath
		{
			get
			{
				try
				{
					return HttpRuntime.AppDomainAppPath;  // C:\Apollo\idm\snap\trunk\release\build\src\web\SNAP\EmailTemplate
				}
				catch (Exception ex)
				{
					Logger.Error("Utilities > AbsolutePath", ex);
					return string.Empty;
				}
			}
		}		

		//private static void ConfigPerEnvironment(long requestId, string pageName)
		//{
		//    _imageUrl = @"http://";
		//    _followLinkUrl = @"http://";

		//    if (Environment.UserDomainName.ToUpper().Contains("DEV"))
		//    {
		//        _imageUrl += Environment.MachineName + ".devapollogrp.edu/snap/images";
		//        _followLinkUrl += (Environment.MachineName + ".devapollogrp.edu/snap/" + pageName + ".aspx?RequestId=" + requestId);
		//    }
		//    else if (Environment.UserDomainName.ToUpper().Contains("QA"))
		//    {
		//        _imageUrl += "access.qaapollogrp.edu/snap/images";
		//        _followLinkUrl += ("access.qaapollogrp.edu/snap/" + pageName + ".aspx?RequestId=" + requestId);
		//    }
		//    else
		//    {
		//        _imageUrl += "access.apollogrp.edu/snap/images";
		//        _followLinkUrl += ("access.apollogrp.edu/snap/" + pageName + ".aspx?RequestId=" + requestId);
		//    }

		//    _imageUrl = "http://dwaxulbp001.devapollogrp.edu/snap/images";
		//    _followLinkUrl = "http://dwaxulbp001.devapollogrp.edu/snap/" + pageName + ".aspx?RequestId=" + requestId;
		//}		
	}
}
