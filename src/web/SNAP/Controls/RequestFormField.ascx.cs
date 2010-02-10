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
                _innerLabel.Text = value;
            }
        }

        public string FieldDescription
        {
            set
            {
                _innerDescription.Text = value;
            }
        }

        public string FieldId
        {
            set
            {
                _accessDetailsFormId.ID = value;
                _labelContainer.AssociatedControlID = _accessDetailsFormId.ID;
            }
        }
    }
}