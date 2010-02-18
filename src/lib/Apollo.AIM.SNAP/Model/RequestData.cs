using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
    public class RequestData
    {
        public string FormId { get; set; } 
        public string UserText { get; set; }
        public static XElement ToXml(List<RequestData> data)
        {
            var root = new XElement("ROOT");
            foreach (var list in data)
            {
                var requestData = new XElement("request_data", new XAttribute("access_details_formId", list.FormId), new XAttribute("userText", list.UserText));
                root.Add(requestData);
            }

            return root;
        }
    }
}
