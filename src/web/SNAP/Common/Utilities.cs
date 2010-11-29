using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using Apollo.AIM.SNAP.CA;
using Apollo.AIM.SNAP.Model;
using Apollo.CA.Logging;
using Microsoft.SharePoint;
using System.Net;
using System.Xml;

namespace Apollo.AIM.SNAP.Web.Common
{
    public class Utilities
    {
        #region Sharepoint

        [DataContract]
        public class ReportListItem
        {
            public ReportListItem() { }
            public ReportListItem(string title, string link, string smallImage, string largeImage)
            {
                this.Title = title;
                this.Link = link;
                this.SmallImage = SmallImage;
                this.LargeImage = LargeImage;
            }

            [DataMember]
            public string Title { get; set; }
            [DataMember]
            public string Link { get; set; }
            [DataMember]
            public string SmallImage { get; set; }
            [DataMember]
            public string LargeImage { get; set; }
        }

        #region WebMethod Calls

        public static List<ReportListItem> GetBottleneckApproversListItems(string SitePath, string UserName, string Password, string Domain)
        {
            List<ReportListItem> reportList = new List<ReportListItem>();
            try
            {
                its_sharepoint_lists.Lists ListsService = new Apollo.AIM.SNAP.Web.its_sharepoint_lists.Lists();

                // the user credentials to use
                ListsService.Credentials = new NetworkCredential(UserName, Password, Domain);
                ListsService.Url = SitePath + "_vti_bin/Lists.asmx";

                // gets the file versions
                XmlNode Result = ListsService.GetListItems("Bottleneck Approvers", "", null, null, "", null, null);
                
                foreach (XmlNode node in Result)
                {
                    if (node.Name == "rs:data")
                    {
                        for (int i = 0; i < node.ChildNodes.Count; i++)
                        {
                            ReportListItem report = new ReportListItem();
                            if (node.ChildNodes[i].Name == "z:row")
                            {
                                string[] docLink = node.ChildNodes[i].Attributes["ows_Link"].Value.Split(',');
                                string[] smallLink = node.ChildNodes[i].Attributes["ows_SmallImage"].Value.Split(',');
                                string[] largeLink = node.ChildNodes[i].Attributes["ows_LargeImage"].Value.Split(',');
                                report.Title = node.ChildNodes[i].Attributes["ows_Title"].Value;
                                report.Link = docLink[0];
                                report.SmallImage = smallLink[0];
                                report.LargeImage = largeLink[0];
                                reportList.Add(report);
                            }
                        }
                    }
                }

                // dispose the web service object
                ListsService.Dispose();
                reportList.Reverse();
                return reportList;
            }
            catch (Exception ex)
            {
                Logger.Error("Ajax Utilites > GetReportListItems [\r\nMessage" + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                return null;
            }
        }

        #endregion

        #endregion
    }
}
