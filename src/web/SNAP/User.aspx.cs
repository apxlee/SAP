using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SNAP
{
    public partial class User : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        [WebMethod]
        public static List<UserManagerInfo> GetNames(string name)
        {
            var result = new List<UserManagerInfo>
                             {
                                 /*
                                 new UserManagerInfo()
                                     {
                                         LoginId = "pxlee1",
                                         ManagerLoginId = "mgr1",
                                         ManagerName = "Manager 1",
                                         Name = "Pong 1"
                                     },
                                  */
                                     new UserManagerInfo()
                                     {
                                         LoginId = "pxlee2",
                                         ManagerLoginId = "mgr2",
                                         ManagerName = "Manager 2",
                                         Name = "Pong 2"

                                     }
                               
                           };
            return result;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            this.Label1.Text = UserManager1.UserName+"(" + UserManager1.UserLoginId + ") - " + UserManager1.ManagerName + "(" + UserManager1.ManagerLoginId + ")";
        }

    }

    public class UserManagerInfo
    {
        public string Name { get; set; }
        public string LoginId { get; set; }
        public string ManagerName { get; set; }
        public string ManagerLoginId { get; set; }
    }
}
