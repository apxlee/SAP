using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apollo.AIM.SNAP.Model
{
    public partial class SNAPDatabaseDataContext
    {
        public IEnumerable<SNAP_Access_User_Text> RetrieveRequest(int requestId)
        {
            //using (var db = new SNAPDatabaseDataContext())
            //{
                var data = from userText in SNAP_Access_User_Texts
                           where (userText.requestId == requestId)
                           group userText by userText.access_details_formId into g
                           let latest = g.Max(t => t.modifiedDate)
                           select new { newUserText = g.Where(t => t.modifiedDate == latest) };


                foreach (var x in data)
                {
                    foreach (var y in x.newUserText)
                    {
                        yield return y;
                    }
                }
            //}

        }
    }
}
