using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class GrsHelper
    {
        public static GRS.ReleaseRights[] GetReleaseRights(string upc, string searchOption)
        {
            var client = new Lib.GRS.ReleaseDataProviderClient();
            var response = client.GetReleaseRights(upc, searchOption);
            client.Close();
            return response;
        }

        public static GRS.RepertoireRightsSearchResult[] GetReleasesSearch()
        {
            var client = new Lib.GRS.ReleaseClient();
            var criteria = new GRS.SearchRepertoireCriteria();
            var response = client.GetReleasesSearch(false, criteria);
            client.Close();
            return response;
        }
    }
}
