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

        public static List<RequestData> UpdateRequestList(List<RequestData> newRequestList, List<usp_open_request_tabResult> requestData)
        {
            var result = new List<RequestData>();
            foreach (var newData in newRequestList)
            {
                var data = requestData.Single(x => x.fieldId == System.Convert.ToInt16(newData.FormId));
                if (data.fieldText != newData.UserText)
                {
                    result.Add(new RequestData() { FormId = newData.FormId, UserText = newData.UserText});
                }
            }
            return result;
        }

        public static void UpdateRequestData(List<RequestData> newRequestList, List<usp_open_request_tabResult> requestData)
        {
            var result = UpdateRequestList(newRequestList, requestData);
            if (result.Count > 0)
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    var reqId = requestData[0].requestId;
                    foreach(var data in result)
                    {
                        var accessUserText = new SNAP_Access_User_Text()
                                                 {
                                                     requestId = reqId,
                                                     access_details_formId = Convert.ToInt16(data.FormId),
                                                     userText = data.UserText,
                                                     modifiedDate = DateTime.Now
                                                 };
                        db.SNAP_Access_User_Texts.InsertOnSubmit(accessUserText);
                    }
                    db.SubmitChanges();
                }
            }
        }
    }
}
