using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Lib
{
    public class RestrictionsWorker
    {
        int Index { get; set; }
        string Upc { get; set; }
        string CountryCode { get; set; }
        public RestrictionsResult Result { get; private set; }

        public RestrictionsWorker(int index, string upc, string countryCode)
        {
            Index = index;
            Upc = upc;
            CountryCode = countryCode;
        }


        public void Go()
        {
            Result = GetRestrictionsResultAsync(Index, Upc, CountryCode);
        }
        
        private RestrictionsResult GetRestrictionsResultAsync(int index, string upc, string countryCode)
        {
            var sw = new Stopwatch();
            sw.Start();
            Logger.LogInfo("Starting Stop Watch " + index.ToString());

            var result = new RestrictionsResult { Index = index, Upc = upc, Exists = false, Restrictions = "", Note = "" };

            try
            {
                var productResponse = Helper.GetProductRestrictionsAsync(upc, countryCode);
                result.ProductItems = D2MappingHelper.GetMappedProductRestrictions(productResponse.Result);
                result.Exists = true;
                // Use only the last note.
                if (productResponse.Result.DeliveryCommentsList.Count > 0)
                    result.Note = productResponse.Result.DeliveryCommentsList.Last();
            }
            catch
            {
                // Do nothing.
            }
            try
            {
                var partnerResponse = Helper.GetPartnerRestrictionsAsync(upc, countryCode);
                result.PartnerItems = D2MappingHelper.GetMappedPartnerRestrictions(partnerResponse.Result);
            }
            catch
            {
                // Do nothing.
            }

            // Build final sentence.
            var finalSentenceList = new List<string>();
            finalSentenceList.Add(D2MappingHelper.CombineProductRestrictions(result.ProductItems));
            finalSentenceList.Add(D2MappingHelper.CombinePartnerRestrictions(result.PartnerItems));
            finalSentenceList.Add(result.Note);

            finalSentenceList = finalSentenceList.Where(x => x != "").ToList();

            result.Restrictions = result.Exists ? string.Join(" ", finalSentenceList.ToArray()) : "Not found.";

            sw.Stop();
            result.TotalSeconds = sw.Elapsed.TotalSeconds;
            Logger.LogInfo(Index.ToString() + " " + sw.Elapsed.TotalSeconds.ToString());

            return result;
        }
    }
}
