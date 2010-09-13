using System;
using System.Web;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Apollo.CA.Logging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace Apollo.AIM.SNAP.Model
{
	public class Utilities
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

    public static class JSONHelper
    {
        public static string ToJSONString(this object obj)
        {
            var serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                string retVal = Encoding.Default.GetString(ms.ToArray());
                return retVal;
            }

        }

        public static T FromJSONStringToObj<T> (this string jsonString)
        {
            //var serializer = new JavaScriptSerializer();
            //return serializer.Deserialize<T>(jsonString);

            var serializer = new DataContractJsonSerializer(typeof (T));
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                var obj = (T) serializer.ReadObject(ms);
                return obj;
            }

        }

    }

    [DataContract]
    public class WebMethodResponse
    {
        public WebMethodResponse() { }
        /*
        public static string SerializeResponse(WebMethodResponse response)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(response.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, response);
                string retVal = Encoding.Default.GetString(ms.ToArray());
                return retVal;
            }
        }
        public static WebMethodResponse DeserializeResponse(string response)
        {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WebMethodResponse));
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(response)))
            {
                WebMethodResponse responseObject = (WebMethodResponse)serializer.ReadObject(ms);
                return responseObject;
            }
        }
         */
        public WebMethodResponse(bool success, string title, string message)
        {
            this.Success = success;
            this.Title = title;
            this.Message = message;
        }
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}
