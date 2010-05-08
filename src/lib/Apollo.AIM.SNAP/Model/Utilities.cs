using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
	class Utilities
	{
		public static string WebRootUrl
		{
			get { return @"http://" + Environment.MachineName + Environment.UserDomainName + ".edu/snap"; }
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
