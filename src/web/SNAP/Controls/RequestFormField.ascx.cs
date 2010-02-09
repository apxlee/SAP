using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AIM.SNAP.Web.Controls
{
    public partial class RequestFormField : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string FieldLabel
        {
            set
            {
                InnerLabel.Text = value;
            }
        }

        public string FieldDescription
        {
            set
            {
                InnerDescription.Text = value;
            }
        }

        public string FieldId
        {
            set
            {
                Access_Details_FormId.ID = value;
            }
        }
    }
}