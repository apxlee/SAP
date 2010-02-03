﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
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

        [WebMethod]
        public static List<UserManagerInfo> GetNames(string name)
        {
            var result = new List<UserManagerInfo>
                             {
                                 
                                 new UserManagerInfo()
                                     {
                                         LoginId = "pxlee1",
                                         ManagerLoginId = "mgr1",
                                         ManagerName = "Manager 1",
                                         Name = "Pong 1"
                                     },
                                 
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

    }

}
