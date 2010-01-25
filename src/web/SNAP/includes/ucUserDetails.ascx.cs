using System;
using System.Web.UI.WebControls;
//using ComputerAccessProcess;
using Apollo.Ultimus.CAP.Model;
using Apollo.Ultimus.CAP;
//using ComputerAccessProcess.Controls.Search;

namespace OOSPA.includes
{
    public partial class ucUserDetails : System.Web.UI.UserControl
    {

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CapSession cs = new CapSession(Session);
            if (cs.SearchParameters != null)
            {
                SearchParams param = cs.SearchParameters;

                if (Request.Params["id"] == null)
                {
                    cs.ClearSearchParameters();
                }
                else
                {
                    UpdateField(param, Request.Params["id"].ToString());
                }
                
            }

        }

        /// <summary>
        /// Gets the person from session.
        /// </summary>
        /// <returns></returns>
        private Person GetPersonFromSession()
        {
            CapSession cs = new CapSession(Session);
            return cs.SavedPerson;
        }

        /// <summary>
        /// Gets the new person context.
        /// </summary>
        /// <param name="PersonOID">The person OID.</param>
        /// <returns></returns>
        private static Person GetNewPersonContext(string PersonOID)
        {
            return PersonTools.GetPersonFromOID(PersonOID);
        }

        /// <summary>
        /// Saves the person changes to session.
        /// </summary>
        private void SavePersonChangesToSession()
        {
            CapSession cs = new CapSession(Session);
            Person person = cs.SavedPerson;

            person.FirstName = txtFName.Text;
            person.MiddleInitial = txtMName.Text;
            person.LastName = txtLName.Text;
            person.PreferredName = txtPreFName.Text;
            person.DisplayName = txtDName.Text;
            person.employeeId = txtEmpID.Text;
            person.EmployeeType = ddlEmpType.SelectedValue;
            person.EmployeeStatus = ddlEmpStatus.SelectedValue;
            if (txtStartDate.Text != "") { person.StartDate = Convert.ToDateTime(txtStartDate.Text); } else { person.StartDateNullable = null; }
            if (txtEndDate.Text != "") { person.EndDate = Convert.ToDateTime(txtEndDate.Text); } else { person.EndDateNullable = null; }
            person.PSUOPEmplId = txtUOPEmpID.Text;
            person.PSUOPUserId = txtUOPUserID.Text;
            person.PSWIUEmplId = txtWIUEmpID.Text;
            person.PSWIUUserId = txtWIUUserID.Text;
            if (chkUrgentRemoval.Checked) { person.UrgentTerm = "Y"; } else { person.UrgentTermNullable = null; }
    }

        /// <summary>
        /// Inits the specified person OID.
        /// </summary>
        /// <param name="PersonOID">The person OID.</param>
        public void init(string PersonOID)
        {

            Person person = GetNewPersonContext(PersonOID);
            lblOID.Text = person.Oid.ToString();
            txtFName.Text = person.FirstName;
            txtMName.Text = person.MiddleInitial;
            txtLName.Text = person.LastName;
            txtPreFName.Text = person.PreferredName;
            txtDName.Text = person.DisplayName;
            lblSSN.Text = person.Last4SocSec.ToString();
            txtEmpID.Text = person.employeeId;
            ddlEmpType.SelectedValue = person.EmployeeType;
            ddlEmpStatus.SelectedValue = person.EmployeeStatus;

            txtManager.Text = person.ManagerLink.DisplayName;
            lblMangerID.Text = person.ManagerPersonOid.ToString();

            txtJob.Text = person.Job.Name;
            lblJobID.Text = person.JobOid.ToString();

            txtCostCenter.Text = person.CostCode.Description;
            lblCostCenterID.Text = person.CostCodeOid.ToString();

            txtCampus.Text = person.Location.campus;
            lblCampusID.Text = person.locationOid.ToString();
            
            txtContract.Text = person.ContractorAgency;
            txtUOPEmpID.Text = person.PSUOPEmplId;
            txtUOPUserID.Text = person.PSUOPUserId;
            txtWIUEmpID.Text = person.PSWIUEmplId;
            txtWIUUserID.Text = person.PSWIUUserId;

            if (person.UrgentTerm == "Y")
            {
                chkUrgentRemoval.Checked = true;
            }
            else
            {
                chkUrgentRemoval.Checked = false;
            }

            if (person.StartDate.Year == 1)
            {
                txtStartDate.Text = "";
            }
            else { txtStartDate.Text = person.StartDate.ToShortDateString(); }

            if (person.EndDate.Year == 1)
            {
                txtEndDate.Text = "";
            }else{ txtEndDate.Text = person.EndDate.ToShortDateString();}

            CapSession cs = new CapSession(Session);
            cs.SavedPerson = person;

            //string searchURL = ResolveUrl("../../Controls/Search/SearchService.asmx");

            lblCheckID.Attributes.Clear();
            //lblCheckID.Attributes.Add("onclick", "javascript:DoIDSearch(" + txtEmpID.ClientID + ", " + person.Oid.ToString() + ", '" + searchURL + "')");
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            
            txtFName.Text = "";
            txtMName.Text = "";
            txtLName.Text = "";
            txtPreFName.Text = "";
            txtDName.Text = "";
            lblSSN.Text = "";
            txtEmpID.Text = "";
            txtManager.Text = "";
            txtJob.Text = "";
            txtCostCenter.Text = "";
            txtCampus.Text = "";
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtContract.Text = "";
            txtUOPEmpID.Text = "";
            txtUOPUserID.Text = "";
            txtWIUEmpID.Text = "";
            txtWIUUserID.Text = "";
            chkUrgentRemoval.Checked = false;
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblOutput.Text = "Not supported!!!";
            return;

            CapSession cs = new CapSession(Session);
            if (cs.SavedPerson == null)
            {
                return;
            }

            Person person = cs.SavedPerson;
            person.FirstName = txtFName.Text;
            person.MiddleInitial = txtMName.Text;
            person.LastName = txtLName.Text;
            person.PreferredName = txtPreFName.Text;
            person.DisplayName = txtDName.Text;
            person.employeeId = txtEmpID.Text;
            person.EmployeeType = ddlEmpType.SelectedValue;
            person.EmployeeStatus = ddlEmpStatus.SelectedValue;
            person.ManagerPersonOid = Convert.ToInt32(lblMangerID.Text);
            person.JobOid = Convert.ToInt32(lblJobID.Text);
            person.CostCodeOid = Convert.ToInt32(lblCostCenterID.Text);
            person.locationOid = Convert.ToInt32(lblCampusID.Text);
            person.ContractorAgency = txtContract.Text;
            person.PSUOPEmplId = txtUOPEmpID.Text;
            person.PSUOPUserId = txtUOPUserID.Text;
            person.PSWIUEmplId = txtWIUEmpID.Text;
            person.PSWIUUserId = txtWIUUserID.Text;

            if (txtEndDate.Text == "" && ddlEmpType.SelectedValue == "S")
            {
                person.EndDateNullable = null;
            }
            else
            {
                DateTime dtProperDate;
                if (DateTime.TryParse(txtEndDate.Text, out dtProperDate))
                {
                    person.EndDate = dtProperDate;
                }
                else
                {
                    lblOutput.Text = "Could not save due to an invalid end date.";
                    return;
                }
            }

            if (txtStartDate.Text == "" && ddlEmpType.SelectedValue == "S")
            {
                person.StartDateNullable = null;
            }
            else
            {
                DateTime dtProperDate;
                if ((DateTime.TryParse(txtStartDate.Text, out dtProperDate) &&
                    DateTime.Compare(dtProperDate, person.EndDate) < 0) ||
                    (txtEndDate.Text == "" && ddlEmpType.SelectedValue == "S"))
                {
                    person.StartDate = dtProperDate;
                }
                else
                {
                    lblOutput.Text = "Could not save due to an invalid start date.";
                    return;
                }
            }

            if (chkUrgentRemoval.Checked)
            {
                person.UrgentTermNullable = "Y";
            }
            else
            {
                person.UrgentTermNullable = null;
            }

            // validate the employee ID
            Person p = PersonTools.GetPersonFromEmpId(person.employeeId);
            if (p != null && p.Oid != person.Oid)
            {
                // can't save - invalid employee ID
                lblOutput.Text = "Could not save due to an invalid Employee ID.  That Employee ID already belongs to someone else.";
                return;
            }

            // save
            (new ContextManager()).Persist(person.Context);

            NeoDbManager.IdmContext.Clear();

            lblOutput.Text = "Save was successful";
            cs.SavedPerson = PersonTools.GetPersonFromOID(person.Oid);
        }

        /// <summary>
        /// Updates the field.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="updateID">The update ID.</param>
        public void UpdateField(SearchParams param, string updateID)
        {
            Person person = GetPersonFromSession();

            switch (param.Type)
            {
                case SearchParams.SearchType.CostCenter:
                    person.CostCodeOid = Convert.ToInt32(updateID);
                    CostCodeFactory ccf = new CostCodeFactory(NeoDbManager.IdmReadonlyContext);
                    CostCode cc = ccf.FindObject(person.CostCodeOid);
                    txtCostCenter.Text = cc.Description;
                    lblCostCenterID.Text = updateID;
                    break;

                case SearchParams.SearchType.Job:
                    person.JobOid = Convert.ToInt32(updateID);
                    JobFactory jf = new JobFactory(NeoDbManager.IdmReadonlyContext);
                    Job j = jf.FindObject(person.JobOid);
                    txtJob.Text = j.Name;
                    lblJobID.Text = updateID;
                    break;
                case SearchParams.SearchType.Location:
                    person.locationOid = Convert.ToInt32(updateID);
                    txtCampus.Text = person.Location.campus;
                    lblCampusID.Text = updateID;
                    break;
                    //Location l = lf.
                    //txtCampus.Text = l.campus;
                case SearchParams.SearchType.PersonByID:
                    // everything is done in the javascript
                    break;
                case SearchParams.SearchType.Manager:
                    person.ManagerPersonOid = Convert.ToInt32(updateID);
                    Person manager = PersonTools.GetPersonFromOID(NeoDbManager.IdmReadonlyContext, updateID);
                    txtManager.Text = manager.DisplayName;
                    lblMangerID.Text = updateID;
                    break;
            }

            CapSession cs = new CapSession(Session);
            cs.ClearSearchParameters();
            cs.SavedPerson = person;
        }

        /// <summary>
        /// Searches the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
        public void Search(object sender, CommandEventArgs e)
        {
            // save any values that changed before we leave the page
            SavePersonChangesToSession();

            SearchParams param = new SearchParams();
            param.SelectURL = "UserMgmt.aspx?do=" + lblOID.Text + "&id=";
            param.ID = lblOID.Text;

            // this seems stupid
            if (e.CommandArgument.ToString() == SearchParams.SearchType.CostCenter.ToString())
            {
                param.Type = SearchParams.SearchType.CostCenter;
            }
            else if (e.CommandArgument.ToString() == SearchParams.SearchType.Job.ToString())
            {
                param.Type = SearchParams.SearchType.Job;
            }
            else if (e.CommandArgument.ToString() == SearchParams.SearchType.Location.ToString())
            {
                param.Type = SearchParams.SearchType.Location;
            }
            else if (e.CommandArgument.ToString() == SearchParams.SearchType.PersonByID.ToString())
            {
                param.Type = SearchParams.SearchType.PersonByID;
            }
            else if (e.CommandArgument.ToString() == SearchParams.SearchType.Manager.ToString())
            {
                param.Type = SearchParams.SearchType.Manager;
            }


            CapSession cs = new CapSession(Session);
            cs.SearchParameters = param;
            Server.Transfer("search.aspx");
            //string thing = e.CommandArgument.ToString();
        }
    }
}