using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.CA;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Web.Common
{
	public class SnapUser
	{
		public string LoginId { get; set; }
		public string FullName { get; set; }
		public string ManagerName { get; set; }
		public string ManagerLoginId { get; set; }
        public string DistributionGroup { get; set; }
        public string[] MemberOf { get; set; }
		public Role CurrentRole { get; set; }
		public string DefaultPage { get; private set; }

		public SnapUser(string networkId)
		{
			LoginId = networkId;
			SetADProperties();
			
			if (WebUtilities.IsSuperUser(LoginId))
			{
				CurrentRole = Role.SuperUser;
			}
			else
			{
				SetRole();
			}
			
			SetDefaultPage();
		}
		
		private void SetADProperties()
		{
			try
			{
				ADUserDetail userDetail = CA.DirectoryServices.GetUserByLoginName(LoginId);

				if (String.IsNullOrEmpty(userDetail.FirstName) || String.IsNullOrEmpty(userDetail.LastName)){ FullName = "NOT FOUND"; }
				else { FullName = userDetail.FirstName + " " + userDetail.LastName; }

				if (String.IsNullOrEmpty(userDetail.ManagerName)) { ManagerName = "NOT FOUND"; }
				else { ManagerName = userDetail.ManagerName; }

				if (String.IsNullOrEmpty(userDetail.Manager.LoginName)) { ManagerLoginId = String.Empty; }
				else { ManagerLoginId = userDetail.Manager.LoginName; } // TODO: is this id? what is full name?

				MemberOf = userDetail.MemberOf.Split(';');
            }
			catch (Exception ex)
			{
				MemberOf = null;
                Logger.Error("SnapUser > SetADProperties\r\nMessage: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
			}
		}
		
		private void SetRole()
		{
//#if DEBUG
//    CurrentRole = Role.Requestor;
//    return;
//#endif
			CurrentRole = Role.Requestor;
			using (var db = new SNAPDatabaseDataContext())
			try
			{
				int approverCount = (from sa in db.SNAP_Actors
					join sag in db.SNAP_Actor_Groups on sa.actor_groupId equals sag.pkId
					where sa.userId == LoginId
					&& (sag.actorGroupType == 0 || sag.actorGroupType == 1 || sag.actorGroupType == 2)
					select new { sa.pkId }).Count();
							 
				int accessTeamCount = (from sa in db.SNAP_Actors
					join sag in db.SNAP_Actor_Groups on sa.actor_groupId equals sag.pkId
					where sa.userId == LoginId
					&& sag.actorGroupType == 3
					select new { sa.pkId }).Count();
				
				if (approverCount > 0 || (IsGroupMember())) { CurrentRole = Role.ApprovingManager; }
				if (accessTeamCount > 0) { CurrentRole = Role.AccessTeam; }
				if ((approverCount > 0) && (accessTeamCount > 0)) { CurrentRole = Role.SuperUser; }
			}
			catch (Exception ex)
			{
                Logger.Error("SnapUser - SetRole, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
			}
		}
		
		private void SetDefaultPage()
		{
			//TODO REMOVE Logger.Info(WebUtilities.GetTimestamp() + "SnapUser > SetDefaultPage > switch(CurrentRole) (93):" + CurrentRole + "\r\n"); 
			switch (CurrentRole)
			{
				case Role.ApprovingManager:
					DefaultPage = PageNames.APPROVING_MANAGER;
					break;

				case Role.AccessTeam:
					DefaultPage = PageNames.ACCESS_TEAM;
					break;

				case Role.SuperUser:
				case Role.Requestor:
					DefaultPage = PageNames.USER_VIEW;
					break;

				case Role.NotAuthorized:
				default:
					DefaultPage = PageNames.DEFAULT_LOGIN;
					break;			
			}
		}

        private bool IsGroupMember()
        {
            try
            {
                using (var db = new SNAPDatabaseDataContext())
                {
                    int result = db.SNAP_Actors
                                    .Where(w => w.isGroup == true
                                            && w.isActive == true
                                            && MemberOf.Contains(w.userId))
                                    .Select(s => s.pkId)
                                    .Count();
                    if (result > 0) { return true; }
                    else { return false; }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SnapUser - IsGroupMember, Message:" + ex.Message + ", StackTrace: " + ex.StackTrace);
            }
            return false;
        }
	}
}
