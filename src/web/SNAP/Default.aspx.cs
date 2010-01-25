using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Apollo.Ultimus.CAP.Model;

namespace SNAP
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
            userDetail.init(p.Oid.ToString());
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            LoginValidator.SetLoginPerson(Session, null);
            Response.Redirect("Index.aspx");
        }
    }

}
