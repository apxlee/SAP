using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.CA;
using Apollo.AIM.SNAP.Web.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace Apollo.AIM.SNAP.Web
{
    public partial class _AjaxCalls : SnapPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        //[WebMethod]
        //public static List<UserManagerInfo> GetNames(string name)
        //{
        //    List<UserManagerInfo> singleUser = new List<UserManagerInfo>();

        //    if (name == string.Empty)
        //    {
        //        return new List<UserManagerInfo>();
        //    }

        //    if (name.Length <= 8)
        //    {
        //        UserManagerInfo userManagerInfo = new UserManagerInfo();
        //        ADUserDetail userDetail;
        //        userDetail = DirectoryServices.GetUserByLoginName(name);

        //        if (userDetail != null)
        //        {
        //            if (userDetail.LoginName != null && userDetail.ManagerName != null && userDetail.FirstName != null && userDetail.LastName != null)
        //            {
        //                userManagerInfo.LoginId = userDetail.LoginName;
        //                userManagerInfo.ManagerLoginId = DirectoryServices.GetUserByFullName(userDetail.ManagerName).LoginName;
        //                userManagerInfo.ManagerName = userDetail.ManagerName;
        //                userManagerInfo.Name = userDetail.FirstName + " " + userDetail.LastName;
        //                singleUser.Add(userManagerInfo);
        //            }
        //        }
        //    }

        //    if (singleUser.Count == 0) { return DirectoryServices.GetSimplifiedUserManagerInfo(name); }
        //    else { return singleUser; }
        //}


        //[WebMethod]
        //public static UserManagerInfo GetUserManagerInfoByFullName(string fullName)
        //{
        //    return DirectoryServices.GetUserManagerInfoByFullName(fullName);
        //}


        //[WebMethod]
        //public static int GetActorId(string userId, int groupId)
        //{
        //    int actorId = 0;
        //    actorId = ApprovalWorkflow.GetActorIdByUserIdAndGroupId(userId, groupId);
        //    return actorId;
        //}

    }

}
