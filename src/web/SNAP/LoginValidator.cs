using System;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Collections;
using Apollo.Ultimus.CAP.Model;

namespace Apollo.AIM.SNAP.Web
{
	/// <summary>
	/// Summary description for LoginValidator.
	/// </summary>
	public static class LoginValidator
	{
		/// <summary>
		/// Verifies the login.
		/// </summary>
		/// <param name="Session">The session.</param>
		/// <param name="Request">The request.</param>
		/// <param name="Response">The response.</param>
		/// <param name="Server">The server.</param>
		/// <returns></returns>
		public static Person VerifyLogin(HttpSessionState Session, HttpRequest Request, HttpResponse Response,
			HttpServerUtility Server)
		{
			try
			{
                CapSession cs = new CapSession(Session);
				Person p = cs.LoginPerson;
			    if (p == null)
			    {
			        RedirectToLogin(Request, Response);
			    }
				else
				{
					if (Apollo.CA.ConfigValue.GetString("MaintenanceOn").ToString() == "true" )
					{
						if (!CurrentUserInRole(Session,"MaintenanceUsers"))
						{
                            cs.ClearLoginPerson();
							Response.Redirect("maintenance.html");		
							return null;
						}
						else
						{
							return p;
						}

					}
					else
					{
						return p;
					}
				}

			}
			catch (ThreadAbortException)
			{
			}
			catch (Exception )
			{
				RedirectToLogin(Request, Response);
			}
			return null;
		}

		
		/// <summary>
		/// Currents the user in role.
		/// </summary>
		/// <param name="Session">The session.</param>
		/// <param name="roleName">Name of the role.</param>
		/// <returns></returns>
		public static bool CurrentUserInRole(HttpSessionState Session, string roleName)
		{
            string userList = ConfigurationManager.AppSettings[roleName].ToString();
			return userList != null && userList.IndexOf(CurrentLogin(Session).userId) != -1;
		}

		
		/// <summary>
		/// Redirects to login.
		/// </summary>
		/// <param name="Request">The request.</param>
		/// <param name="Response">The response.</param>
		public static void RedirectToLogin(HttpRequest Request, HttpResponse Response)
		{
            
			Response.Redirect(Request.ApplicationPath + "/" + "Index.aspx" + "?" + QueryStringConstants.MessageKey + "=Your session has timed out. Please login again.",true);//ref=" + Request.ServerVariables["URL"] + "%3f" + Request.QueryString.ToString().Replace("&", "%26") );
			
		}
		
		
		/// <summary>
		/// Currents the login.
		/// </summary>
		/// <param name="Session">The session.</param>
		/// <param name="Request">The request.</param>
		/// <param name="Response">The response.</param>
		/// <returns></returns>
		public static Person CurrentLogin(HttpSessionState Session, HttpRequest Request, HttpResponse Response)
		{
            CapSession cs = new CapSession(Session);		     
            if (cs.LoginPerson == null)
			{
				RedirectToLogin(Request, Response);
				return null;
			}
			return CurrentLogin(Session);
		}
		
		/// <summary>
		/// Currents the login.
		/// </summary>
		/// <param name="Session">The session.</param>
		/// <returns></returns>
		public static Person CurrentLogin(HttpSessionState Session)
		{
            CapSession cs = new CapSession(Session);
            return cs.LoginPerson;
		}
		
		
		/// <summary>
		/// Sets the login person.
		/// </summary>
		/// <param name="Session">The session.</param>
		/// <param name="LoggedIn">The logged in.</param>
		public static void SetLoginPerson(HttpSessionState Session, Person LoggedIn)
		{
		    CapSession cs = new CapSession(Session);
		    cs.LoginPerson = LoggedIn;
		}

        /// <summary>
		/// Determines whether the user who is set as logged in via the SetLoginPerson method
		/// has the requested access to admin sections
		/// </summary>
        /// <param name="session">The user session</param>
        /// <param name="requestedAccess">The requested access from AdminAccessValues</param>
		/// <returns>
		/// 	<c>true</c> if the user is in the AD role; otherwise, <c>false</c>.
		/// </returns>
        public static bool HasAdminRole(HttpSessionState session, string[] requestedAccess)
        {
            CapSession cs = new CapSession(session);
            Person p = cs.LoginPerson;
            if (p != null)
            {
              ArrayList groups = Apollo.CA.DirectoryServices.getADAMGroups(p.userId);

              // loop through the users groups and try to find a match
              for (int i = 0; i < groups.Count; i++)
              {
                for (int j = 0; j < requestedAccess.Length; j++)
                {
                  if (groups[i].ToString().ToUpper() == requestedAccess[j].ToUpper())
                  {
                    return true;
                  }
                }
              }
              return false;
            }
            else
            {
              return false;
            }
            
        }

	}
}
