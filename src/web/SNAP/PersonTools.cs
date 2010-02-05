using System;
using System.Data;
using System.Web;
using System.Web.SessionState;
using Apollo.CA.Data;
using Apollo.Ultimus.CAP;
using Apollo.Ultimus.CAP.Model;
using Neo.Core;

namespace Apollo.AIM.SNAP.Web
{
  /// <summary>
  /// Summary description for PersonManager.
  /// </summary>
  public static class PersonTools
  {

    public static string CombineVarGuid4Request(string VarName, HttpRequest Request)
    {
      return CombineVarGuid4Request(VarName, Request.QueryString[QueryStringConstants.GUID]);
    }

    public static string CombineVarGuid4Request(string VarName, string guid)
    {
      return VarName + guid;
    }

    /// <summary>
    /// Gets the person.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="session">The session.</param>
    /// <returns></returns>
    public static Person GetPerson(HttpRequest request, HttpSessionState session)
    {
      CapSession cs = new CapSession(session);
      Person currentPerson = cs.GetCurrentPerson(request);
      if (currentPerson != null)
      {
        return currentPerson;
      }
      else
      {
        return SetPersonFromQS(request, session);
      }
    }

    /// <summary>
    /// Gets the person.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="session">The session.</param>
    /// <param name="guid">The GUID.</param>
    /// <returns></returns>
    public static Person GetPerson(HttpRequest request, HttpSessionState session, string guid)
    {
      CapSession cs = new CapSession(session);
      Person p = cs.GetCurrentPerson(guid);
      if (p != null)
      {
        return p;
      }
      else
      {
        throw new ArgumentException("There is no person currently stored under the guid: " + guid);
      }
    }

    /// <summary>
    /// Sets the person.
    /// </summary>
    /// <param name="session">The session.</param>
    /// <param name="personValue">The person value.</param>
    /// <param name="request">The request.</param>
    public static void SetPerson(HttpSessionState session, Person personValue, HttpRequest request)
    {
      CapSession cs = new CapSession(session);
      cs.SetCurrentPerson(request, personValue);
    }

    public static void SetPerson(HttpSessionState session, Person personValue, string guid)
    {
      CapSession cs = new CapSession(session);
      cs.SetCurrentPerson(guid, personValue);
    }

    /// <summary>
    /// Gets the new person.
    /// </summary>
    /// <param name="idmContext">The idm context.</param>
    /// <returns></returns>
    public static Person GetNewPerson(ObjectContext idmContext)
    {
      //idmContext.Clear();
      PersonFactory pf = new PersonFactory(idmContext);
      Person newP = pf.CreateObject(DbIdHelper.GetNewPersonOid());
      newP.StartDate = DateTime.Now;
      newP.JobOid = 0;
      newP.CostCodeOid = 0;
      return newP;
    }

    /// <summary>
    /// Sets the person from QS.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="session">The session.</param>
    /// <returns></returns>
    public static Person SetPersonFromQS(HttpRequest request, HttpSessionState session)
    {
      CapSession cs = new CapSession(session);
      cs.NewPersonCreated = false;
      if (String.IsNullOrEmpty(request[QueryStringConstants.PersonOidKey]))
      {
        PersonAdapter pa = new PersonAdapter(NeoDbManager.IdmFreshContext());

        if (request[QueryStringConstants.TypeKey] != "Approve")
        {
          string EafId = request[QueryStringConstants.EAFIdKey];
          string EmployeeId = request[QueryStringConstants.EmployeeIdKey];
          //string ContractorCheck = request[QueryStringConstants.ContractorCheckKey];
          string strFoundUserOid = request[QueryStringConstants.FoundUserOidKey] ?? "-1";
          EafUser u;

          if (EafId != null || EmployeeId != null)  // probably a new staff
          {

            EafUserFactory factory = new EafUserFactory(NeoDbManager.EafContext);
            u = EafUser.FindRelatedEAFUser(EafId, EmployeeId, factory);

            if (u == null)
            {
              throw new ApplicationException("Could not find eaf user for eafId = '" + EafId + "', employeeId = '" +
                                             EmployeeId + "'");
            }

            Person manager = GetPersonFromEmpId(u.managerId);

            if (manager == null)
            {
              throw new ApplicationException("Could not find manager with Emplid " + u.managerId);
            }

            cs.EafUser = u;
            cs.EafManager = manager;
          }
          else  // its a contractor.
          {
            Person newP;


            if (strFoundUserOid != "-1")
            {
              PersonFactory pf = new PersonFactory(NeoDbManager.IdmFreshContext());
              newP = pf.FindFirst("Oid = " + strFoundUserOid);
              newP.StartDate = DateTime.Now;
              newP.JobOid = 0;
              newP.CostCodeOid = 0;
            }
            else
            {
              newP = GetNewPerson(NeoDbManager.IdmFreshContext());
            }
            newP.EmployeeType = "C";
            SetPerson(session, newP, request);
            return newP;
          }

          Person p;
          if (strFoundUserOid != "-1")
          {
            p = pa.OverwritePersonWithEaf(u, strFoundUserOid);
            cs.NewPersonCreated = false;
          }
          else
          {
            p = pa.GetPerson(u);
            cs.NewPersonCreated = true;
          }
          cs.SetCurrentPerson(request, p);

          return p;
        }
        else
        {
          PersonFactory pf = new PersonFactory(NeoDbManager.IdmFreshContext());

          Person newP;

          try
          {
            newP = pf.FindFirst("Oid={0}", UltimusTools.GetVariableValue("G_Subject_Person_OID", session, request).ToString());//

            if (newP == null)
              newP = GetNewPerson(NeoDbManager.IdmFreshContext());
          }
          catch
          {
            newP = GetNewPerson(NeoDbManager.IdmFreshContext());
          }

          SetPerson(session, newP, request);
          return newP;
        }
      }
      else
      {
        //NeoDbManager.IdmContext.Clear();
        CaObjectContext oc = NeoDbManager.IdmFreshContext();
        PersonFactory pf = new PersonFactory(oc);
        PersonList pl = pf.Find("Oid={0}", request[QueryStringConstants.PersonOidKey]);

        if (pl.Count == 0)
        {
          throw new DataException("Fatal Error in SetPersonFromQS: Could not find person with OID " + request[QueryStringConstants.PersonOidKey].ToString() + ".");
        }
        Person p = pl[0];

        cs.SetCurrentPerson(request, p);
        return p;
      }
    }

    /// <summary>
    /// Gets the person oid from empl id.
    /// </summary>
    /// <param name="emplId">The empl id.</param>
    /// <returns></returns>
    public static int GetPersonOidFromEmplId(params object[] emplId)
    {
      Person p = GetPersonFromEmpId(NeoDbManager.IdmFreshContext(), emplId);
      return (p == null ? -1 : p.Oid);
    }

    /// <summary>
    /// Returns the person with the given employeeId
    /// </summary>
    /// <param name="context"></param>
    /// <param name="empId"></param>
    /// <returns></returns>
    public static Person GetPersonFromEmpId(ObjectContext context, params object[] empId)
    {
      PersonFactory pf = new PersonFactory(context);
      Person p = pf.FindFirst("employeeId = {0}", empId);
      return p;
    }

    /// <summary>
    /// Gets the person from emp id.
    /// </summary>
    /// <param name="empId">The emp id.</param>
    /// <returns></returns>
    public static Person GetPersonFromEmpId(params object[] empId)
    {
      return GetPersonFromEmpId(NeoDbManager.IdmContext, empId);
    }

    /// <summary>
    /// Returns the person based on OID using the given context.
    /// </summary>
    /// <param name="context">CaObjectContext</param>
    /// <param name="oid">OID of person</param>
    /// <returns></returns>
    public static Person GetPersonFromOID(ObjectContext context, params object[] oid)
    {
      string personOid = oid[0].ToString();
      if (personOid == String.Empty)
      {
        personOid = "0";
      }
      PersonFactory pf = new PersonFactory(context);
      Person selectedPerson = pf.FindFirst("Oid = {0}", personOid);
      return selectedPerson;
    }

    /// <summary>
    /// Returns the person based on OID using NeoDbManager.IdmContext
    /// </summary>
    /// <param name="oid">OID of person.</param>
    /// <returns></returns>
    public static Person GetPersonFromOID(params object[] oid)
    {
      return GetPersonFromOID(NeoDbManager.IdmFreshContext(), oid);
    }

    /// <summary>
    /// Finds the person based on User ID
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userId"></param>
    /// <returns>Person based on UserID</returns>
    public static Person GetPersonFromUserId(ObjectContext context, params object[] userId)
    {
      PersonFactory pf = new PersonFactory(context);
      Person p = pf.FindFirst("userId = {0}", userId);
      return p;
    }

    /// <summary>
    /// Finds the person based on User ID using the NeoDbManager.IdmContext
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Person based on UserID</returns>
    public static Person GetPersonFromUserId(params object[] userId)
    {
      return GetPersonFromUserId(NeoDbManager.IdmContext, userId);
    }

  }
}
