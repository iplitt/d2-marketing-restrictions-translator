using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;

namespace Lib
{
    public class GrsHelper
    {
        public static GRS.ReleaseRights[] GetReleaseRights(string id, string searchOption)
        {
            var client = new GRS.ReleaseDataProviderClient();
            var response = client.GetReleaseRights(id, searchOption);
            client.Close();
            return response;
        }

        public static GRS.RepertoireRightsSearchResult[] GetReleasesSearch()
        {
            var client = new GRS.ReleaseClient();
            var criteria = new GRS.SearchRepertoireCriteria();
            var response = client.GetReleasesSearch(false, criteria);
            client.Close();
            return response;
        }
    }
}
