namespace Apollo.AIM.SNAP.Web
{
  public static class QueryStringConstants
  {
    public const string MessageKey = "m";
    public const string EAFIdKey = "eafId";
    public const string EmployeeIdKey = "employeeId";
    public const string TaskIdKey = "TaskID";
    public const string UserKey = "user";
    public const string IncidentIdKey = "IncidentID";
    public const string PersonOidKey = "personoid";
    public const string ViewKey = "view";
    public const string TypeKey = "type";
    public const string FoundUserOidKey = "FoundUserOid";
    public const string RefreshKey = "Refresh";
    public const string CancelUrlKey = "cancelurl";
    public const string ContinueUrlKey = "continueurl";
    public const string MessageTypeKey = "MessageType";
    public const string ServiceKey = "service";
    public const string RefKey = "ref";
    public const string GUID = "guid";
    public const string IncidentType = "incidentType";

  }

  public static class SessionVariables
  {
    public const string TaskVariables = "TaskVariables";
    public const string TaskList = "TaskList";
    public const string UserIncidentsList = "ListOfUserIncidents";
    public const string RefreshIncidentList = "RefreshIncidentList";
    public const string IncidentNumbers = "IncidentNumbers";
    public const string SingleVars = "SingleVars";
    public const string MultiVars = "MultiVars";
    public const string Approval = "approval";
    public const string Approvals = "approvals";
    public const string SortDir = "SortDir";
    public const string SortOrder = "SortOrder";
    public const string AuditList = "auditlist";
    public const string AuditSortDirection = "AuditSortDirection";
    public const string AuditSortExpression = "AuditSortExpression";
    public const string Audits = "audits";
    public const string VPNRationale = "vpnRationale";
    public const string Submitted = "Submitted";
    public const string OriginalLocation = "OriginalLocation";
    public const string EafUser = "eafUser";
    public const string EafManager = "eafManager";
    public const string RolesReviewed = "RolesReviewed";
    public const string ResetRoles = "ResetRoles";
    public const string CampusListItems = "CampusesListItems";
    public const string CostCodeListItems = "CostCodesListItems";
    public const string JobCodeListItems = "JobCodesListItems";
    public const string OrgListItems = "OrgListItems";
    public const string SiteListItems = "SitesListItems";
    public const string Err1Message = "Err1Message";
    public const string NewPersonCreated = "NewPersonCreated";
    public const string LoginPerson = "LoginPerson";
    public const string CurrentPerson = "CurrentPerson";
    public const string OriginalRoles = "OriginalRoles";
    public const string NewRoles = "NewRoles";
    public const string CurrentIncidentPrefix = "_CurrentIncident_";

    //AA Constants
    public const string PolicyName = "PolicyName";
    public const string PolicyJobs = "PolicyJobs";
    public const string PolicyRanges = "PolicyRanges";
    public const string PolicyExclusions = "PolicyExclusions";
    public const string PolicyDefaults = "PolicyDefaults";

    public const string Ranges = "Ranges";

    public const string XMLBatchProvInfo = "XMLBATCHPROVINFO";

    public const string SearchParams = "SearchParams";
    public const string SavedPerson = "SavedPerson";

    public const string AppovalMatrix = "ApprovalMatrix";
    public const string DeletedApprovalMatrix = "DeletedApprovalMatrix";
  }

  //  the admin section uses these values to determine which menu items to show
  public static class AdminAccessValues
  {
    public const string ADMINACCESS = "Idmgmt AIM Admin";
    public const string BATCHADMIN = "Idmgmt Support Batch Admin";
    public const string SUPPORTADMIN = "Idmgmt Support Admin";
    public const string PASSWORDRESET = "Delegated AD Role - Technical Support";
    public const string ONBOARDING = "Onboarding Support";
    public const string POLICYADMIN = "Idmgmt Additional Access Admin Update";
    public const string POLICYREADONLY = "Idmgmt Additional Access Admin Read Only";
    public const string LEGALADMIN = "Idmgmt Support Legal Admin";
    public const string APPROVALMATRIX = "Approval Matrix Admin";
    public const string VPNTUTORIALADMIN = "VPN Tutorial Admin";
  }

  public static class MiscValues
  {
    public const string JobTypeEmployeeOnly = "Emp. Only";
  }
}
