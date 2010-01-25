using System;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using Apollo.CA;
using Apollo.Ultimus;
using Apollo.Ultimus.Audit;
using Apollo.Ultimus.CAP.Model;
using Apollo.Ultimus.UltimusIncidents;
using Apollo.AIM.Ultimus.Workflows;
//using ComputerAccessProcess.Controls.Search;

namespace SNAP
{
  internal class CapSession
  {
    private HttpSessionState _session;

    /// <summary>
    /// Initializes a new instance of the <see cref="CapSession"/> class.
    /// </summary>
    /// <param name="capSession">The cap session.</param>
    public CapSession(HttpSessionState capSession)
    {
      _session = capSession;
    }

    /// <summary>
    /// The session variable that decides if a list is to be sorted 
    /// ascending or descending
    /// </summary>
    public ListOrder SortDirection
    {
      get
      {
        if (_session[SessionVariables.SortDir] == null)
        {
          _session[SessionVariables.SortDir] = ListOrder.Ascending;
        }

        return (ListOrder)_session[SessionVariables.SortDir];
      }

      set { _session[SessionVariables.SortDir] = value; }
    }

    /// <summary>
    /// Gets or sets the audit sort direction.
    /// </summary>
    /// <value>The audit sort direction.</value>
    public ListOrder AuditSortDirection
    {
      get
      {
        if (_session[SessionVariables.AuditSortDirection] == null)
        {
          _session[SessionVariables.AuditSortDirection] = ListOrder.Ascending;
        }

        return (ListOrder)_session[SessionVariables.AuditSortDirection];
      }

      set { _session[SessionVariables.AuditSortDirection] = value; }
    }

    /// <summary>
    /// Gets or sets the sort expression.
    /// </summary>
    /// <value>The sort expression.</value>
    public string SortExpression
    {
      get
      {
        if (_session[SessionVariables.SortOrder] == null)
        {
          _session[SessionVariables.SortOrder] = "TaskId";
        }
        return _session[SessionVariables.SortOrder].ToString();
      }

      set { _session[SessionVariables.SortOrder] = value; }
    }

    /// <summary>
    /// Gets or sets the audit sort expression.
    /// </summary>
    /// <value>The audit sort expression.</value>
    public string AuditSortExpression
    {
      get
      {
        if (_session[SessionVariables.AuditSortExpression] == null)
        {
          _session[SessionVariables.AuditSortExpression] = "Auditee.DisplayName";
        }
        return _session[SessionVariables.AuditSortExpression].ToString();
      }

      set { _session[SessionVariables.AuditSortExpression] = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [new person created].
    /// </summary>
    /// <value><c>true</c> if [new person created]; otherwise, <c>false</c>.</value>
    public bool NewPersonCreated
    {
      get { return Convert.ToBoolean(_session[SessionVariables.NewPersonCreated], CultureInfo.CurrentCulture); }

      set { _session[SessionVariables.NewPersonCreated] = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [refresh incident list].
    /// </summary>
    /// <value><c>true</c> if [refresh incident list]; otherwise, <c>false</c>.</value>
    public bool RefreshIncidentList
    {
      get { return Convert.ToBoolean(_session[SessionVariables.RefreshIncidentList], CultureInfo.CurrentCulture); }

      set { _session[SessionVariables.RefreshIncidentList] = value; }
    }

    /// <summary>
    /// Gets or sets the audits.
    /// </summary>
    /// <value>The audits.</value>
    public ArrayList Audits
    {
      get { return (ArrayList)_session[SessionVariables.Audits]; }

      set { _session[SessionVariables.Audits] = value; }
    }

    /// <summary>
    /// Gets or sets the audit list.
    /// </summary>
    /// <value>The audit list.</value>
    public AuditTaskList AuditList
    {
      get { return (AuditTaskList)_session[SessionVariables.AuditList]; }

      set { _session[SessionVariables.AuditList] = value; }
    }

    /// <summary>
    /// Sets the combo key variable.
    /// </summary>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="request">The request.</param>
    /// <param name="item">The item.</param>
    private void SetComboKeyVariable(string variableName, HttpRequest request, object item)
    {
      string sessionVariable = PersonTools.CombineVarGuid4Request(variableName, request);
      _session[sessionVariable] = item;
    }

    /// <summary>
    /// Sets the combo key variable.
    /// </summary>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="guid">The GUID.</param>
    /// <param name="item">The item.</param>
    private void SetComboKeyVariable(string variableName, string guid, object item)
    {
      string sessionVariable = PersonTools.CombineVarGuid4Request(variableName, guid);
      _session[sessionVariable] = item;
    }

    /// <summary>
    /// Gets the combo key variable.
    /// </summary>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    private object GetComboKeyVariable(string variableName, HttpRequest request)
    {
      string sessionVariable = PersonTools.CombineVarGuid4Request(variableName, request);
      return _session[sessionVariable];
    }

    /// <summary>
    /// Gets the combo key variable.
    /// </summary>
    /// <param name="variableName">Name of the variable.</param>
    /// <param name="guid">The GUID.</param>
    /// <returns></returns>
    private object GetComboKeyVariable(string variableName, string guid)
    {
      string sessionVariable = PersonTools.CombineVarGuid4Request(variableName, guid);
      return _session[sessionVariable];
    }

    /// <summary>
    /// Sets the task variables.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="taskVariables">The task variables.</param>
    public void SetTaskVariables(HttpRequest request, Hashtable taskVariables)
    {
      SetComboKeyVariable(SessionVariables.TaskVariables, request, taskVariables);
    }

    /// <summary>
    /// Gets the task variables.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public Hashtable GetTaskVariables(HttpRequest request)
    {
      return (Hashtable)GetComboKeyVariable(SessionVariables.TaskVariables, request);
    }

    /// <summary>
    /// Sets the submitted.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="isSubmitted">if set to <c>true</c> [is submitted].</param>
    public void SetSubmitted(HttpRequest request, bool isSubmitted)
    {
      SetComboKeyVariable(SessionVariables.Submitted, request, isSubmitted);
    }

    /// <summary>
    /// Determines whether the specified request is submitted.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>
    /// 	<c>true</c> if the specified request is submitted; otherwise, <c>false</c>.
    /// </returns>
    public bool IsSubmitted(HttpRequest request)
    {
      object isSubmitted = GetComboKeyVariable(SessionVariables.Submitted, request);
      return Convert.ToBoolean(isSubmitted, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Clears the is submitted.
    /// </summary>
    /// <param name="request">The request.</param>
    public void ClearIsSubmitted(HttpRequest request)
    {
      string isSubmittedVariable = PersonTools.CombineVarGuid4Request(SessionVariables.Submitted, request);
      _session.Remove(isSubmittedVariable);
    }

    /// <summary>
    /// Sets the task list.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="taskList">The task list.</param>
    public void SetTaskList(HttpRequest request, UltTaskList taskList)
    {
      SetComboKeyVariable(SessionVariables.TaskList, request, taskList);
    }

    /// <summary>
    /// Gets the task list.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public UltTaskList GetTaskList(HttpRequest request)
    {
      return (UltTaskList)GetComboKeyVariable(SessionVariables.TaskList, request);
    }

    /// <summary>
    /// Sets the VPN rationale.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="rationale">The rationale.</param>
    public void SetVpnRationale(HttpRequest request, string rationale)
    {
      SetComboKeyVariable(SessionVariables.VPNRationale, request, rationale);
    }

    /// <summary>
    /// Gets the VPN rationale.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public string GetVpnRationale(HttpRequest request)
    {
      return (string)GetComboKeyVariable(SessionVariables.VPNRationale, request);
    }

    /// <summary>
    /// Clears the VPN rationale.
    /// </summary>
    /// <param name="request">The request.</param>
    public void ClearVpnRationale(HttpRequest request)
    {
      string vpnRationaleVariable = PersonTools.CombineVarGuid4Request(SessionVariables.VPNRationale, request);
      _session.Remove(vpnRationaleVariable);
    }

    /// <summary>
    /// Sets the current person.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="person">The person.</param>
    public void SetCurrentPerson(HttpRequest request, Person person)
    {
      SetComboKeyVariable(SessionVariables.CurrentPerson, request, person);
    }

    /// <summary>
    /// Sets the current person.
    /// </summary>
    /// <param name="guid">The GUID.</param>
    /// <param name="person">The person.</param>
    public void SetCurrentPerson(string guid, Person person)
    {
      SetComboKeyVariable(SessionVariables.CurrentPerson, guid, person);
    }

    /// <summary>
    /// Gets the current person.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public Person GetCurrentPerson(HttpRequest request)
    {
      return (Person)GetComboKeyVariable(SessionVariables.CurrentPerson, request);
    }

    /// <summary>
    /// Gets the current person.
    /// </summary>
    /// <param name="guid">The GUID.</param>
    /// <returns></returns>
    public Person GetCurrentPerson(string guid)
    {
      return (Person)GetComboKeyVariable(SessionVariables.CurrentPerson, guid);
    }

    /// <summary>
    /// Clears the current person.
    /// </summary>
    /// <param name="request">The request.</param>
    public void ClearCurrentPerson(HttpRequest request)
    {
      string personSessionVariable = PersonTools.CombineVarGuid4Request(SessionVariables.CurrentPerson, request);
      _session.Remove(personSessionVariable);
    }

    /// <summary>
    /// Removes the error.
    /// </summary>
    public void RemoveError()
    {
      _session.Remove(SessionVariables.Err1Message);
    }

    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    /// <value>The error.</value>
    public string Error
    {
      get { return (string)_session[SessionVariables.Err1Message]; }
      set { _session.Add(SessionVariables.Err1Message, value); }
    }

    /// <summary>
    /// Gets or sets the XML batch provision info.
    /// </summary>
    /// <value>The XML batch provision info.</value>
    public string XMLBatchProvisionInfo
    {
      get { return (string)_session[SessionVariables.XMLBatchProvInfo]; }
      set { _session.Add(SessionVariables.XMLBatchProvInfo, value); }
    }

    /// <summary>
    /// Gets or sets the login person.
    /// </summary>
    /// <value>The login person.</value>
    public Person LoginPerson
    {
      get { return (Person)_session[SessionVariables.LoginPerson]; }
      set { _session[SessionVariables.LoginPerson] = value; }
    }

    /// <summary>
    /// Clears the login person.
    /// </summary>
    public void ClearLoginPerson()
    {
      _session.Remove(SessionVariables.LoginPerson);
    }

    /// <summary>
    /// Gets or sets the campus list items.
    /// </summary>
    /// <value>The campus list items.</value>
    public ListItemCollection CampusListItems
    {
      get { return (ListItemCollection)_session[SessionVariables.CampusListItems]; }
      set { _session[SessionVariables.CampusListItems] = value; }
    }

    /// <summary>
    /// Gets or sets the cost code list items.
    /// </summary>
    /// <value>The cost code list items.</value>
    public ListItemCollection CostCodeListItems
    {
      get { return (ListItemCollection)_session[SessionVariables.CostCodeListItems]; }
      set { _session[SessionVariables.CostCodeListItems] = value; }
    }

    /// <summary>
    /// Gets or sets the job code list items.
    /// </summary>
    /// <value>The job code list items.</value>
    public ListItemCollection JobCodeListItems
    {
      get { return (ListItemCollection)_session[SessionVariables.JobCodeListItems]; }
      set { _session[SessionVariables.JobCodeListItems] = value; }
    }

    /// <summary>
    /// Gets or sets the org list items.
    /// </summary>
    /// <value>The org list items.</value>
    public ListItemCollection OrgListItems
    {
      get { return (ListItemCollection)_session[SessionVariables.OrgListItems]; }
      set { _session[SessionVariables.OrgListItems] = value; }
    }

    /// <summary>
    /// Gets or sets the site list items.
    /// </summary>
    /// <value>The site list items.</value>
    public ListItemCollection SiteListItems
    {
      get { return (ListItemCollection)_session[SessionVariables.SiteListItems]; }
      set { _session[SessionVariables.SiteListItems] = value; }
    }

    /// <summary>
    /// Gets or sets the user incidents list.
    /// </summary>
    /// <value>The user incidents list.</value>
    public UltIncidentList UserIncidentsList
    {
      get { return (UltIncidentList)_session[SessionVariables.UserIncidentsList]; }
      set { _session[SessionVariables.UserIncidentsList] = value; }
    }

    /// <summary>
    /// Gets or sets the incident numbers.
    /// </summary>
    /// <value>The incident numbers.</value>
    public int[] IncidentNumbers
    {
      get { return (int[])_session[SessionVariables.IncidentNumbers]; }
      set { _session[SessionVariables.IncidentNumbers] = value; }
    }

    /// <summary>
    /// Gets or sets the single vars.
    /// </summary>
    /// <value>The single vars.</value>
    public Hashtable SingleVars
    {
      get { return (Hashtable)_session[SessionVariables.SingleVars]; }
      set { _session[SessionVariables.SingleVars] = value; }
    }

    /// <summary>
    /// Gets or sets the multi vars.
    /// </summary>
    /// <value>The multi vars.</value>
    public Hashtable MultiVars
    {
      get { return (Hashtable)_session[SessionVariables.MultiVars]; }
      set { _session[SessionVariables.MultiVars] = value; }
    }

    /// <summary>
    /// Gets or sets selected approval.
    /// </summary>
    /// <value>The selected approval.</value>
    public Object Approval
    {
      get { return (WorkflowBase)_session[SessionVariables.Approval]; }
      set { _session[SessionVariables.Approval] = value; }
    }

    /// <summary>
    /// Gets or sets the approvals contains WorkflowCAP and WorkflowProvisioning objects
    /// </summary>
    /// <value>The approvals.</value>
    public ArrayList Approvals
    {
      get { return (ArrayList)_session[SessionVariables.Approvals]; }
      set { _session[SessionVariables.Approvals] = value; }
    }

    /// <summary>
    /// Gets or sets the approval matrixlist.
    /// </summary>
    /// <value>The approval matrixlist.</value>
    public ApprovalMatrixList ApprovalMatrixlist
    {
      get { return (ApprovalMatrixList)_session[SessionVariables.AppovalMatrix]; }
      set { _session[SessionVariables.AppovalMatrix] = value; }
    }

    /// <summary>
    /// Gets or sets the approval matrixlist.
    /// </summary>
    /// <value>The approval matrixlist.</value>
    public ApprovalMatrixList DeletedApprovalMatrixlist
    {
      get { return (ApprovalMatrixList)_session[SessionVariables.DeletedApprovalMatrix]; }
      set { _session[SessionVariables.DeletedApprovalMatrix] = value; }
    }

    /// <summary>
    /// Clears the approvals.
    /// </summary>
    public void ClearApprovals()
    {
      _session.Remove(SessionVariables.Approvals);
    }

    /// <summary>
    /// Gets or sets the new roles.
    /// </summary>
    /// <value>The new roles.</value>
    public RoleList NewRoles
    {
      get { return (RoleList)_session[SessionVariables.NewRoles]; }
      set { _session[SessionVariables.NewRoles] = value; }
    }

    /// <summary>
    /// Gets or sets the original roles.
    /// </summary>
    /// <value>The original roles.</value>
    public RoleList OriginalRoles
    {
      get { return (RoleList)_session[SessionVariables.OriginalRoles]; }
      set { _session[SessionVariables.OriginalRoles] = value; }
    }

    /// <summary>
    /// Gets or sets the original location.
    /// </summary>
    /// <value>The original location.</value>
    public string OriginalLocation
    {
      get { return (string)_session[SessionVariables.OriginalLocation]; }
      set { _session[SessionVariables.OriginalLocation] = value; }
    }

    /// <summary>
    /// Gets or sets the name of the policy.
    /// </summary>
    /// <value>The name of the policy.</value>
    public string PolicyName
    {
      get { return (string)_session[SessionVariables.PolicyName]; }
      set { _session[SessionVariables.PolicyName] = value; }
    }

    /// <summary>
    /// Clears the name of the policy.
    /// </summary>
    public void ClearPolicyName()
    {
      _session.Remove(SessionVariables.PolicyName);
    }

    /// <summary>
    /// Gets or sets the policy jobs.
    /// </summary>
    /// <value>The policy jobs.</value>
    public ArrayList PolicyJobs
    {
      get { return (ArrayList)_session[SessionVariables.PolicyJobs]; }
      set { _session[SessionVariables.PolicyJobs] = value; }
    }

    /// <summary>
    /// Clears the policy jobs.
    /// </summary>
    public void ClearPolicyJobs()
    {
      _session.Remove(SessionVariables.PolicyJobs);
    }

    /// <summary>
    /// Gets or sets the policy ranges.
    /// </summary>
    /// <value>The policy ranges.</value>
    public ArrayList PolicyRanges
    {
      get { return (ArrayList)_session[SessionVariables.PolicyRanges]; }
      set {
        if (value == null || value.Count == 0)
        {
          ClearPolicyRanges();
        }
        else
        {
          _session[SessionVariables.PolicyRanges] = value;
        }
      }
    }

    /// <summary>
    /// Clears the policy ranges.
    /// </summary>
    public void ClearPolicyRanges()
    {
      _session.Remove(SessionVariables.PolicyRanges);
    }

    /// <summary>
    /// Gets or sets the ranges.
    /// </summary>
    /// <value>The ranges.</value>
    public ArrayList Ranges
    {
      get { return (ArrayList)_session[SessionVariables.Ranges]; }
      set { _session[SessionVariables.Ranges] = value; }
    }

    /// <summary>
    /// Clears the ranges.
    /// </summary>
    public void ClearRanges()
    {
      _session.Remove(SessionVariables.Ranges);
    }

    /// <summary>
    /// Gets or sets the policy defaults.
    /// </summary>
    /// <value>The policy defaults.</value>
    public ArrayList PolicyDefaults
    {
      get { return (ArrayList)_session[SessionVariables.PolicyDefaults]; }
      set { _session[SessionVariables.PolicyDefaults] = value; }
    }

    /// <summary>
    /// Clears the policy defaults.
    /// </summary>
    public void ClearPolicyDefaults()
    {
      _session.Remove(SessionVariables.PolicyDefaults);
    }

    /// <summary>
    /// Gets or sets the policy exclusions.
    /// </summary>
    /// <value>The policy exclusions.</value>
    public ArrayList PolicyExclusions
    {
      get { return (ArrayList)_session[SessionVariables.PolicyExclusions]; }
      set { _session[SessionVariables.PolicyExclusions] = value; }
    }

    /// <summary>
    /// Gets or sets the search parameters.
    /// </summary>
    /// <value>The search parameters.</value>
    public SearchParams SearchParameters
    {
      get { return (SearchParams)_session[SessionVariables.SearchParams]; }
      set { _session[SessionVariables.SearchParams] = value; }
    }

    /// <summary>
    /// Clears the search parameters.
    /// </summary>
    public void ClearSearchParameters()
    {
      _session.Remove(SessionVariables.SearchParams);
    }

    /// <summary>
    /// Gets or sets the saved person.
    /// </summary>
    /// <value>The saved person.</value>
    public Person SavedPerson
    {
      get { return (Person)_session[SessionVariables.SavedPerson]; }
      set { _session[SessionVariables.SavedPerson] = value; }
    }

    /// <summary>
    /// Clears the policy exclusions.
    /// </summary>
    public void ClearPolicyExclusions()
    {
      _session.Remove(SessionVariables.PolicyExclusions);
    }

    /// <summary>
    /// Clears the session variables for policy name,
    /// policy jobs, policy ranges, and policy exclusions
    /// </summary>
    public void ClearPolicyInformation()
    {
      ClearPolicyName();
      ClearPolicyJobs();
      ClearPolicyRanges();
      ClearPolicyExclusions();
    }

    /// <summary>
    /// Clears the original location.
    /// </summary>
    public void ClearOriginalLocation()
    {
      _session.Remove(SessionVariables.OriginalLocation);
    }

    /// <summary>
    /// Gets or sets the eaf manager.
    /// </summary>
    /// <value>The eaf manager.</value>
    public Person EafManager
    {
      get { return (Person)_session[SessionVariables.EafManager]; }
      set { _session[SessionVariables.EafManager] = value; }
    }

    /// <summary>
    /// Gets or sets the eaf user.
    /// </summary>
    /// <value>The eaf user.</value>
    public EafUser EafUser
    {
      get { return (EafUser)_session[SessionVariables.EafUser]; }
      set { _session[SessionVariables.EafUser] = value; }
    }

    /// <summary>
    /// Sets the session variable for the given list to the items contained
    /// list item collection. 
    /// </summary>
    /// <param name="listName">The session variable for the list</param>
    /// <param name="items">the list to store</param>
    /// <exception cref="ArgumentException">If invalid list name is given. 
    /// Please use the SessionVariable class constants when calling this method.</exception>
    public void SetList(string listName, ListItemCollection items)
    {
      switch (listName)
      {
        case SessionVariables.CampusListItems:
          CampusListItems = items;
          break;
        case SessionVariables.CostCodeListItems:
          CostCodeListItems = items;
          break;
        case SessionVariables.JobCodeListItems:
          JobCodeListItems = items;
          break;
        case SessionVariables.OrgListItems:
          OrgListItems = items;
          break;
        case SessionVariables.SiteListItems:
          SiteListItems = items;
          break;
        default:
          throw new ArgumentException("The session variable " + listName +
                                      " is not currently being used in the session!");
      }
    }

    /// <summary>
    /// Returns the ListItemCollection stored in the session variable.
    /// </summary>
    /// <param name="listName">The session varaiable name for the list</param>
    /// <returns>the list stored in the session varaible</returns>
    /// <exception cref="ArgumentException">If invalid list name is given. 
    /// Please use the SessionVariable class constants when calling this method.</exception>
    public ListItemCollection GetList(string listName)
    {
      switch (listName)
      {
        case SessionVariables.CampusListItems:
          return CampusListItems;
        case SessionVariables.CostCodeListItems:
          return CostCodeListItems;
        case SessionVariables.JobCodeListItems:
          return JobCodeListItems;
        case SessionVariables.OrgListItems:
          return OrgListItems;
        case SessionVariables.SiteListItems:
          return SiteListItems;
        default:
          throw new ArgumentException("The session variable " + listName +
                                      " is not currently being used in the session!");
      }
    }

    /// <summary>
    /// Sets the roles reviewed.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="rolesReviewed">if set to <c>true</c> [roles reviewed].</param>
    public void SetRolesReviewed(HttpRequest request, bool rolesReviewed)
    {
      SetComboKeyVariable(SessionVariables.RolesReviewed, request, rolesReviewed);
    }

    /// <summary>
    /// Determines whether [is roles reviewed] [the specified request].
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>
    /// 	<c>true</c> if [is roles reviewed] [the specified request]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsRolesReviewed(HttpRequest request)
    {
      object rolesReviewed = GetComboKeyVariable(SessionVariables.RolesReviewed, request);
      return Convert.ToBoolean(rolesReviewed, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Sets the reset roles.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="resetRoles">if set to <c>true</c> [reset roles].</param>
    public void SetResetRoles(HttpRequest request, bool resetRoles)
    {
      SetComboKeyVariable(SessionVariables.ResetRoles, request, resetRoles);
    }

    /// <summary>
    /// Sets the reset roles.
    /// </summary>
    /// <param name="guid">The GUID.</param>
    /// <param name="resetRoles">if set to <c>true</c> [reset roles].</param>
    public void SetResetRoles(string guid, bool resetRoles)
    {
      SetComboKeyVariable(SessionVariables.ResetRoles, guid, resetRoles);
    }

    /// <summary>
    /// Sets the current incident.
    /// </summary>
    /// <param name="incidentId">The incident id.</param>
    /// <param name="incident">The incident.</param>
    public void SetCurrentIncident(int incidentId, UltIncident incident)
    {
      string sessionKey = SessionVariables.CurrentIncidentPrefix + incidentId.ToString();
      _session[sessionKey] = incident;
    }

    /// <summary>
    /// Gets the current incident.
    /// </summary>
    /// <param name="incidentId">The incident id.</param>
    /// <returns></returns>
    public UltIncident GetCurrentIncident(int incidentId)
    {
      string sessionKey = SessionVariables.CurrentIncidentPrefix + incidentId.ToString();
      return (UltIncident)_session[sessionKey];
    }

    /// <summary>
    /// Shoulds the reset roles.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public bool ShouldResetRoles(HttpRequest request)
    {
      object resetRoles = GetComboKeyVariable(SessionVariables.ResetRoles, request);
      return getDefaultResetRolesOnNull(resetRoles);
    }

    /// <summary>
    /// Shoulds the reset roles.
    /// </summary>
    /// <param name="guid">The GUID.</param>
    /// <returns></returns>
    public bool ShouldResetRoles(string guid)
    {
      object resetRoles = GetComboKeyVariable(SessionVariables.ResetRoles, guid);
      return getDefaultResetRolesOnNull(resetRoles);
    }

    /// <summary>
    /// This code ensures that the reset roles variable defaults to true
    /// </summary>
    /// <param name="resetRoles">true/false or null</param>
    /// <returns>True if null true/false otherwise</returns>
    private static bool getDefaultResetRolesOnNull(object resetRoles)
    {
      if (resetRoles == null)
      {
        return true;
      }
      else
      {
        return Convert.ToBoolean(resetRoles, CultureInfo.CurrentCulture);
      }
    }
  }

  public class SearchParams
  {
      public string SelectURL = "";
      public SearchType Type;
      public string ID = "";
      public enum SearchType
      {
          CostCenter,
          Job,
          Location,
          Manager,
          Person,
          PersonByID,
          RangeSets,
          Policies,
          PersonDelete,
          HRUserView,
          PersonVPN
      }
  }

}