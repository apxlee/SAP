using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Apollo.AIM.SNAP.Model;
using Apollo.AIM.SNAP.Web.Common;
using Apollo.CA.Logging;

namespace Apollo.AIM.SNAP.Web.Controls
{
	public partial class SearchView : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

        protected void Search_Click(object sender, EventArgs e)
        {
            try
            {
                PopulateSections();
            }
            catch (Exception ex)
            {
                Logger.Error("SNAP - Index: loadSearchResults failed", ex);
            }
        }

        private void PopulateSections()
        {
            var searchLoader = new Common.SearchRequestLoader();
            searchLoader.searchText = _searchInput.Text;
            searchLoader.Load();

            ViewBaseUtilities.BuildRequestBlades(RequestState.Search, _searchResultsContainer
                , _nullDataMessage_SearchRequests, null);
        }
	}
}