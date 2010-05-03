using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.CA;

namespace Apollo.AIM.SNAP.Web.Common
{
	public class SnapUser
	{
		public string LoginId { get; set; }
		public string FullName { get; set; }
		public string ManagerName { get; set; }
		public string ManagerLoginId { get; set; }
		public Role CurrentRole { get; set; }
		//public ViewIndex DefaultView { get; private set; } // TODO: remove
		public string DefaultPage { get; private set; }

		private bool _isAccessTeam = false;

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
				if (userDetail.MemberOf.ToLower().Trim().Contains("access"))
				{
					_isAccessTeam = true;
				}
				FullName = userDetail.FirstName + " " + userDetail.LastName;
				ManagerName = userDetail.ManagerName;
				ManagerLoginId = userDetail.Manager.LoginName; // TODO: is this id? what is full name?
			}
			catch (Exception ex)
			{
				FullName = "NOT FOUND";
				ManagerName = "NOT FOUND";
				ManagerLoginId = "NOT FOUND";
				// TODO: Logger.Error("SnapUser > SetADProperties", ex);
			}
		}
		
		private void SetRole()
		{
#if DEBUG
	CurrentRole = Role.Requestor;
	return;
#endif

			using (var snapDb = new SNAPDatabaseDataContext())
			try
			{
				if (_isAccessTeam)
				{
					var rolecheck = snapDb.SNAP_Actors.Where(a => a.actor_groupId == (int)ActorGroupType.Workflow_Admin && a.userId == LoginId && a.isActive == true);
					// TODO: we need to think about this... 
					// if they aren't in the "Heather" subgroup, but are AIM members, should they really be superuser?
					// could move the superuser list to web.config
					// superUser list is the same as maintenanceUser?
					if (rolecheck.Count() > 0) {CurrentRole = Role.SuperUser;}
					else {CurrentRole = Role.AccessTeam;}
				}
				else
				{
                    var rolecheck = snapDb.SNAP_Actors.Where(a => a.actor_groupId != (int)ActorGroupType.Workflow_Admin && a.userId == LoginId && a.isActive == true);
					if (rolecheck.Count() > 0) { CurrentRole = Role.ApprovingManager; }
					else { CurrentRole = Role.Requestor; }
				}
			}
			catch (Exception ex)
			{
				// TODO: Logger.Error("SnapUser > SetRole", ex);
				CurrentRole = Role.NotAuthorized;
			}
		}
		
		private void SetDefaultPage()
		{
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
		
		//private void SetDefaultView() // TODO: remove
		//{
		//    //switch (CurrentRole)
		//    //{
		//    //    case Role.ApprovingManager:
		//    //        DefaultView = ViewIndex.my_approvals;
		//    //        break;

		//    //    case Role.AccessTeam:
		//    //        DefaultView = ViewIndex.access_team;
		//    //        break;

		//    //    case Role.SuperUser:
		//    //        DefaultView = ViewIndex.my_requests;
		//    //        break;

		//    //    case Role.Requestor:
		//    //        DefaultView = ViewIndex.my_requests;
		//    //        break;

		//    //    case Role.NotAuthorized:
		//    //    default:
		//    //        DefaultView = ViewIndex.login;
		//    //        break;
		//    //}
		//}
	}
}
