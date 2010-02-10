using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using System.DirectoryServices;
using System.Configuration;
using Apollo.AIM.SNAP.Model;
using Apollo.CA;

namespace Apollo.AIM.SNAP.CA
{
    //http://www.dotnetspark.com/kb/1217-all-operations-on-windows-active-directory.aspx
    public class DirectoryServices
    {
        private static bool _initialized = false;
        private static LDAPConnection _ldapconn;
        private static NameValueCollection _configDN;

        static DirectoryServices()
        {
            _ldapconn =
                new LDAPConnection(
                    (NameValueCollection)ConfigurationManager.GetSection("Apollo.CA.DirectoryServices/LDAP.Connection"));
            _configDN = (NameValueCollection)ConfigurationManager.GetSection("Apollo.CA.DirectoryServices/AD.knownDNs");

            _initialized = (_ldapconn != null && _ldapconn.Host.Length != 0 && _ldapconn.Username.Length != 0 &&
                            _ldapconn.UserSearchBase.Length != 0);
            if (!_initialized)
            {
                throw new ConfigurationErrorsException(
                    "Could not initialize LDAP connection in static class DirectoryServices. Check web.config file settings.");
            }
        }


        public static ADUserDetail GetUserByLoginName(String userName)
        {
            try
            {
                var directorySearch = new DirectorySearcher(_ldapconn.UserSearchBase);
                directorySearch.Filter = "(&(objectClass=user)(SAMAccountName=" + userName + "))";
                SearchResult results = directorySearch.FindOne();

                if (results != null)
                {
                    var user = new DirectoryEntry(results.Path, _ldapconn.Username, _ldapconn.Password); // ActiveDirectoryHelper.LDAPUser, ActiveDirectoryHelper.LDAPPassword);
                    return ADUserDetail.GetUser(user);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static ADUserDetail GetUserByFullName(String userName)
        {
            try
            {
                var directorySearch = new DirectorySearcher(_ldapconn.UserSearchBase); // ActiveDirectoryHelper.SearchRoot);
                directorySearch.Filter = "(&(objectClass=user)(cn=" + userName + "))";
                SearchResult results = directorySearch.FindOne();

                if (results != null)
                {
                    DirectoryEntry user = new DirectoryEntry(results.Path, _ldapconn.Username, _ldapconn.Password); //ActiveDirectoryHelper.LDAPUser, ActiveDirectoryHelper.LDAPPassword);
                    return ADUserDetail.GetUser(user);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<string> GetAllUserByFullName(String userName)
        {
            try
            {
                var directorySearch = new DirectorySearcher(_ldapconn.UserSearchBase); // ActiveDirectoryHelper.SearchRoot);
                directorySearch.Filter = "(&(objectClass=user)(cn=" + userName + "*))";
                SearchResultCollection results = directorySearch.FindAll();

                if (results != null)
                {
                    // result.path is in this format - LDAP://CN=John Smith (Enrollment Counselor),OU=Users,OU=UOP-PA-Pittsburgh2,OU=C-UOP-Pittsburgh,OU=UOP,OU=Sites,DC=devapollogrp,DC=edu
                    // result.path can be LDAP://CN=Administrator,CN=Users,DC=devapollogrp,DC=edu
                    var names = new List<string>();
                    foreach (SearchResult result in results)
                    {
                        string[] x = result.Path.Split(new string[] { "CN=" }, StringSplitOptions.RemoveEmptyEntries);

                        // part different path in the result to return user name 
                        //names.Add(x[1].Split(',')[0].Replace("\\", ""));
                        var tmp = x[1].Split(new string[] {",OU="}, StringSplitOptions.RemoveEmptyEntries)[0].Replace("\\", "");
                        // remove traing ,
                        if (tmp[tmp.Length - 1] == ',')
                            tmp = tmp.Substring(0, tmp.Length - 1);
                        names.Add(tmp);
                    }
                    return names;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<UserManagerInfo> GetUserManagerInfo(string fullName)
        {
            var result = new List<UserManagerInfo>();
            //if (fullName != string.Empty)
            //{
                var users = GetAllUserByFullName(fullName);
                users.ForEach(delegate(string x)
                                  {
                                      Console.WriteLine(x);
                                      var u = GetUserByFullName(x);
                                      if (u != null)
                                      {
                                          Console.WriteLine(">>>" + u.LoginName);
                                          result.Add(new UserManagerInfo
                                                         {
                                                             LoginId = u.LoginName ?? "unknown",
                                                             ManagerLoginId = u.Manager != null ? u.Manager.LoginName : "unknown",
                                                             ManagerName = u.ManagerName ?? "unknown",
                                                             Name = x
                                                         });
                                          //DisplayDetails(u);
                                      }
                                      else
                                      {
                                          Console.WriteLine("*** Can't find: " + x);
                                      }
                                  });


            //}
            return result;
        }

        public static void Main()
        {
            
            List<string> users = DirectoryServices.GetAllUserByFullName("Jason Smith");
            users.ForEach(delegate(string x)
                              {
                                  var u = DirectoryServices.GetUserByFullName(x);
                                  DisplayDetails(u);
                              });
            
            ADUserDetail detail = DirectoryServices.GetUserByLoginName("pxlee");
            DisplayDetails(detail);
            detail = DirectoryServices.GetUserByLoginName("gjbelang");
            DisplayDetails(detail);
            Console.ReadLine();
        }

        public static void DisplayDetails(ADUserDetail detail)
        {
            Console.WriteLine("Name: " + detail.LoginNameWithDomain);
            var t = detail.EmailAddress ?? "unknown";
            Console.WriteLine("Email: " + detail.EmailAddress);
            t = detail.ManagerName ?? "unknown";
            Console.WriteLine("Manager Name: " + t);
            t = detail.Manager != null ? detail.Manager.LoginName : "unknown";
            Console.WriteLine("Manager login name: " + t);
            Console.WriteLine("Memberof: " + detail.MemberOf);
            Console.WriteLine("================================");
            Console.WriteLine("");
        }
    }

    #region ADUserDetail
    public class ADUserDetail
    {
        private String _firstName;
        private String _middleName;
        private String _lastName;
        private String _loginName;
        private String _loginNameWithDomain;
        private String _streetAddress;
        private String _city;
        private String _state;
        private String _postalCode;
        private String _country;
        private String _homePhone;
        private String _extension;
        private String _mobile;
        private String _fax;
        private String _emailAddress;
        private String _title;
        private String _company;
        private String _manager;
        private String _managerName;
        private String _department;
        private String _memberOf;

        public String Department
        {
            get { return _department; }
        }

        public String FirstName
        {
            get { return _firstName; }
        }

        public String MiddleName
        {
            get { return _middleName; }
        }

        public String LastName
        {
            get { return _lastName; }
        }

        public String LoginName
        {
            get { return _loginName; }
        }

        public String LoginNameWithDomain
        {
            get { return _loginNameWithDomain; }
        }

        public String StreetAddress
        {
            get { return _streetAddress; }
        }

        public String City
        {
            get { return _city; }
        }

        public String State
        {
            get { return _state; }
        }

        public String PostalCode
        {
            get { return _postalCode; }
        }

        public String Country
        {
            get { return _country; }
        }

        public String HomePhone
        {
            get { return _homePhone; }
        }

        public String Extension
        {
            get { return _extension; }
        }

        public String Mobile
        {
            get { return _mobile; }
        }

        public String Fax
        {
            get { return _fax; }
        }

        public String EmailAddress
        {
            get { return _emailAddress; }
        }

        public String Title
        {
            get { return _title; }
        }

        public String Company
        {
            get { return _company; }
        }

        public String MemberOf
        {
            get
            {
                // assuming the _memberOf in this format:
                // CN=SP C-APO-Enterprise,OU=SharePoint Security Groups,OU=_Enterprise Groups,OU=Sites,DC=devapollogrp,DC=edu|CN=IT014-Senior Software Developer,OU=Job Codes,OU=_Enterprise Groups,OU=Sites,DC=devapollogrp,DC=edu|CN=APO-AZ-Phoenix2,OU=Groups,OU=C-APO-Corporate,OU=APO,OU=Sites,DC=devapollogrp,DC=edu|CN=C-APO-Corporate,OU=Groups,OU=C-APO-Corporate,OU=APO,OU=Sites,DC=devapollogrp,DC=edu|CN=VPN Access - Two Factor Users,OU=VPN Access,OU=_Enterprise Groups,OU=Sites,DC=devapollogrp,DC=edu
                var sb = new StringBuilder();
                string[] members = _memberOf.Split('|');
                foreach (var m in members)
                {
                    var x = m.Split(new string[] { "CN=" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    sb.Append(x.Split(',')[0] + ";");
                }
                return sb.ToString();
            }
        }
        public ADUserDetail Manager
        {
            get
            {
                if (!String.IsNullOrEmpty(_managerName))
                {
                    return DirectoryServices.GetUserByFullName(_managerName);
                }
                return null;
            }
        }

        public String ManagerName
        {
            get { return _managerName; }
        }


        private ADUserDetail(DirectoryEntry directoryUser)
        {

            String domainAddress;
            String domainName;
            _firstName = GetProperty(directoryUser, ADProperties.FIRSTNAME);
            _middleName = GetProperty(directoryUser, ADProperties.MIDDLENAME);
            _lastName = GetProperty(directoryUser, ADProperties.LASTNAME);
            _loginName = GetProperty(directoryUser, ADProperties.LOGINNAME);
            String userPrincipalName = GetProperty(directoryUser, ADProperties.USERPRINCIPALNAME);
            if (!string.IsNullOrEmpty(userPrincipalName))
            {
                domainAddress = userPrincipalName.Split('@')[1];
            }
            else
            {
                domainAddress = String.Empty;
            }

            if (!string.IsNullOrEmpty(domainAddress))
            {
                domainName = domainAddress.Split('.').First();
            }
            else
            {
                domainName = String.Empty;
            }
            _loginNameWithDomain = String.Format(@"{0}\{1}", domainName, _loginName);
            _memberOf = GetProperty(directoryUser, ADProperties.MEMBEROF);
            _streetAddress = GetProperty(directoryUser, ADProperties.STREETADDRESS);
            _city = GetProperty(directoryUser, ADProperties.CITY);
            _state = GetProperty(directoryUser, ADProperties.STATE);
            _postalCode = GetProperty(directoryUser, ADProperties.POSTALCODE);
            _country = GetProperty(directoryUser, ADProperties.COUNTRY);
            _company = GetProperty(directoryUser, ADProperties.COMPANY);
            _department = GetProperty(directoryUser, ADProperties.DEPARTMENT);
            _homePhone = GetProperty(directoryUser, ADProperties.HOMEPHONE);
            _extension = GetProperty(directoryUser, ADProperties.EXTENSION);
            _mobile = GetProperty(directoryUser, ADProperties.MOBILE);
            _fax = GetProperty(directoryUser, ADProperties.FAX);
            _emailAddress = GetProperty(directoryUser, ADProperties.EMAILADDRESS);
            _title = GetProperty(directoryUser, ADProperties.TITLE);
            _manager = GetProperty(directoryUser, ADProperties.MANAGER);
            if (!String.IsNullOrEmpty(_manager))
            {
                String[] managerArray = _manager.Split(',');
                _managerName = managerArray[0].Replace("CN=", "");
            }
        }


        private static String GetProperty(DirectoryEntry userDetail, String propertyName)
        {
            // if there are multiple items return, we concat then with '|' as a separator
            var sb = new StringBuilder();
            if (userDetail.Properties.Contains(propertyName))
            {
                for (int i = 0; i < userDetail.Properties[propertyName].Count; i++)
                    sb.Append(userDetail.Properties[propertyName][i] + "|");
                var x = sb.ToString();
                return x.Substring(0, x.Length - 1); // remove the last '|'
            }
            else
            {
                return string.Empty;
            }
        }

        public static ADUserDetail GetUser(DirectoryEntry directoryUser)
        {
            return new ADUserDetail(directoryUser);
        }
    }
    #endregion

    #region ADProperties
    public static class ADProperties
    {
        public const String OBJECTCLASS = "objectClass";
        public const String CONTAINERNAME = "cn";
        public const String LASTNAME = "sn";
        public const String COUNTRYNOTATION = "c";
        public const String CITY = "l";
        public const String STATE = "st";
        public const String TITLE = "title";
        public const String POSTALCODE = "postalCode";
        public const String PHYSICALDELIVERYOFFICENAME = "physicalDeliveryOfficeName";
        public const String FIRSTNAME = "givenName";
        public const String MIDDLENAME = "initials";
        public const String DISTINGUISHEDNAME = "distinguishedName";
        public const String INSTANCETYPE = "instanceType";
        public const String WHENCREATED = "whenCreated";
        public const String WHENCHANGED = "whenChanged";
        public const String DISPLAYNAME = "displayName";
        public const String USNCREATED = "uSNCreated";
        public const String MEMBEROF = "memberOf";
        public const String USNCHANGED = "uSNChanged";
        public const String COUNTRY = "co";
        public const String DEPARTMENT = "department";
        public const String COMPANY = "company";
        public const String PROXYADDRESSES = "proxyAddresses";
        public const String STREETADDRESS = "streetAddress";
        public const String DIRECTREPORTS = "directReports";
        public const String NAME = "name";
        public const String OBJECTGUID = "objectGUID";
        public const String USERACCOUNTCONTROL = "userAccountControl";
        public const String BADPWDCOUNT = "badPwdCount";
        public const String CODEPAGE = "codePage";
        public const String COUNTRYCODE = "countryCode";
        public const String BADPASSWORDTIME = "badPasswordTime";
        public const String LASTLOGOFF = "lastLogoff";
        public const String LASTLOGON = "lastLogon";
        public const String PWDLASTSET = "pwdLastSet";
        public const String PRIMARYGROUPID = "primaryGroupID";
        public const String OBJECTSID = "objectSid";
        public const String ADMINCOUNT = "adminCount";
        public const String ACCOUNTEXPIRES = "accountExpires";
        public const String LOGONCOUNT = "logonCount";
        public const String LOGINNAME = "sAMAccountName";
        public const String SAMACCOUNTTYPE = "sAMAccountType";
        public const String SHOWINADDRESSBOOK = "showInAddressBook";
        public const String LEGACYEXCHANGEDN = "legacyExchangeDN";
        public const String USERPRINCIPALNAME = "userPrincipalName";
        public const String EXTENSION = "ipPhone";
        public const String SERVICEPRINCIPALNAME = "servicePrincipalName";
        public const String OBJECTCATEGORY = "objectCategory";
        public const String DSCOREPROPAGATIONDATA = "dSCorePropagationData";
        public const String LASTLOGONTIMESTAMP = "lastLogonTimestamp";
        public const String EMAILADDRESS = "mail";
        public const String MANAGER = "manager";
        public const String MOBILE = "mobile";
        public const String PAGER = "pager";
        public const String FAX = "facsimileTelephoneNumber";
        public const String HOMEPHONE = "homePhone";
        public const String MSEXCHUSERACCOUNTCONTROL = "msExchUserAccountControl";
        public const String MDBUSEDEFAULTS = "mDBUseDefaults";
        public const String MSEXCHMAILBOXSECURITYDESCRIPTOR = "msExchMailboxSecurityDescriptor";
        public const String HOMEMDB = "homeMDB";
        public const String MSEXCHPOLICIESINCLUDED = "msExchPoliciesIncluded";
        public const String HOMEMTA = "homeMTA";
        public const String MSEXCHRECIPIENTTYPEDETAILS = "msExchRecipientTypeDetails";
        public const String MAILNICKNAME = "mailNickname";
        public const String MSEXCHHOMESERVERNAME = "msExchHomeServerName";
        public const String MSEXCHVERSION = "msExchVersion";
        public const String MSEXCHRECIPIENTDISPLAYTYPE = "msExchRecipientDisplayType";
        public const String MSEXCHMAILBOXGUID = "msExchMailboxGuid";
        public const String NTSECURITYDESCRIPTOR = "nTSecurityDescriptor";
    }
    #endregion

    #region ActiveDirectoryHelper
    class ActiveDirectoryHelper
    {
        public static DirectoryEntry SearchRoot
        {
            get
            {
                var directoryEntry = new DirectoryEntry(LDAPPath, LDAPUser, LDAPPassword, AuthenticationTypes.Secure);
                return directoryEntry;
            }
        }

        public static String LDAPPath
        {
            get
            {
                return ConfigurationManager.AppSettings["LDAPPath"];
            }
        }

        public static String LDAPUser
        {
            get
            {
                return ConfigurationManager.AppSettings["LDAPUser"];
            }
        }

        public static String LDAPPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["LDAPPassword"];
            }
        }

        public static String LDAPDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["LDAPDomain"];
            }
        }
    }
    #endregion
}