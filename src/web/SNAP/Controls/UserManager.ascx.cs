using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SNAP.Controls
{
    public partial class UserManager : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string UserName
        {
            get { return this.userName.Text; }
        }

        public string UserLoginId
        {
            get { return this.userLoginId.Text; }
        }

        public string ManagerName
        {
            get { return this.mgrName.Text; }
        }

        public string ManagerLoginId
        {
            get { return this.mgrLoginId.Text; }
        }

    }
}