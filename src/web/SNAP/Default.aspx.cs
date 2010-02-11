using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.Ultimus.CAP.Model;

namespace Apollo.AIM.SNAP.Web
{
    public partial class _Default : CapPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Person p = CurrentSession.LoginPerson;
            ArrayList groups = Apollo.CA.DirectoryServices.getADAMGroups(p.userId);
            var groupInfo = "Groups:\n";
            foreach (var list in groups)
            {
                groupInfo += list + "\n";
            }
            txtUsrInfo.Text = "Manager: " + p.ManagerLink.userId + "\n" + "User Info: " + p.ToStringAllProperties();
            txtGroupInfo.Text = groupInfo;
            this.ucUserDetails1.init(p.Oid.ToString());
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            LoginValidator.SetLoginPerson(Session, null);
            Response.Redirect("Index.aspx");
        }

        [WebMethod]
        public static List<UserManagerInfo> GetNames(string name)
        {
            //System.Threading.Thread.Sleep(3000);
            //return CA.DirectoryServices.GetUserManagerInfo(name);
            return CA.DirectoryServices.GetSimplifiedUserManagerInfo(name);
        }

        /*
        [WebMethod]
        public static List<string> GetUserNames(string name)
        {
            //System.Threading.Thread.Sleep(3000);
            return CA.DirectoryServices.GetAllUserByFullName(name);
        }
         */

        [WebMethod]
        public static UserManagerInfo GetUserManagerInfoByFullName(string fullName)
        {
            return CA.DirectoryServices.GetUserManagerInfoByFullName(fullName);
        }

    }

}
