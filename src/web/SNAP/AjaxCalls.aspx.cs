﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.Ultimus.CAP.Model;
using Apollo.AIM.SNAP.CA;
using System.Text;
using System.Text.RegularExpressions;

namespace Apollo.AIM.SNAP.Web
{
    public partial class _AjaxCalls : CapPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*
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
            */
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            LoginValidator.SetLoginPerson(Session, null);
            Response.Redirect("Index.aspx");
        }

        [WebMethod]
        public static List<UserManagerInfo> GetNames(string name)
        {
            List<UserManagerInfo> singleUser = new List<UserManagerInfo>();

            if (name == string.Empty)
            {
                return new List<UserManagerInfo>();
            }

            if (name.Length <= 8)
            {
                UserManagerInfo userManagerInfo = new UserManagerInfo();
                ADUserDetail userDetail;
                userDetail = DirectoryServices.GetUserByLoginName(name);

                if (userDetail != null)
                {
                    if (userDetail.LoginName != null && userDetail.ManagerName != null && userDetail.FirstName != null && userDetail.LastName != null)
                    {
                        userManagerInfo.LoginId = userDetail.LoginName;
                        userManagerInfo.ManagerLoginId = DirectoryServices.GetUserByFullName(userDetail.ManagerName).LoginName;
                        userManagerInfo.ManagerName = userDetail.ManagerName;
                        userManagerInfo.Name = userDetail.FirstName + " " + userDetail.LastName;
                        singleUser.Add(userManagerInfo);
                    }
                }
            }

            if (singleUser.Count == 0) { return DirectoryServices.GetSimplifiedUserManagerInfo(name); }
            else { return singleUser; }
        }


        [WebMethod]
        public static UserManagerInfo GetUserManagerInfoByFullName(string fullName)
        {
            return DirectoryServices.GetUserManagerInfoByFullName(fullName);
        }

        #region

        [WebMethod]
        public static bool CreateWorkflow(int requestId, string actorIds)
        {
            var accessReq = new AccessRequest(requestId);
            string[] split = null;
            char[] splitter = { '[' };
            split = actorIds.Split(splitter);

            List<int> actorsList = new List<int>();

            foreach (string actor in split)
            {
                if (actor.Length > 1) { actorsList.Add(Convert.ToInt32(actor.Replace("[", "").Replace("]", ""))); }
            }

            accessReq.Ack();
            return accessReq.CreateWorkflow(actorsList);
        }

        #endregion

    }

}