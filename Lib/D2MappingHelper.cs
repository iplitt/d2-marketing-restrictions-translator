using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class D2MappingHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="upc"></param>
        /// <param name="territoryID">A menu id.</param>
        /// <param name="productRestrictions">ProductRestrictions object.</param>
        /// <param name="partnerRestrictions">PartnerRestrictions object.</param>
        /// <returns>DigitalDelivery object.</returns>
        //public static DigitalDelivery MapFromD2ToGdrs(
        //  string upc, long territoryID, ProductRestrictions productRestrictions, PartnerRestrictions partnerRestrictions)
        //{
        //    DigitalDelivery dd = new DigitalDelivery();

        //    dd.UPC = upc;
        //    dd.TerritoryID = territoryID;

        //    if (productRestrictions != null && productRestrictions.ProductExists)
        //    {
        //        ProductRestrictionItems productRestrictionItems = GetMappedProductRestrictions(productRestrictions);
        //        dd.DistributionRestriction = productRestrictionItems.DistributionRestriction;
        //        dd.AlbumOnlyRestriction = productRestrictionItems.AlbumOnlyRestriction;
        //        dd.PreOrderOnlyRestriction = productRestrictionItems.PreOrderOnlyRestriction;
        //        dd.DeliveryComments = productRestrictions.DeliveryCommentsList;
        //        dd.ProductExists = 1;
        //    }

        //    if (partnerRestrictions != null && partnerRestrictions.PartnersExist)
        //    {
        //        PartnerRestrictionItems partnerRestrictionItems = GetMappedPartnerRestrictions(partnerRestrictions);
        //        dd.ExclusivityRestriction = partnerRestrictionItems.ExclusivityRestriction;
        //        dd.DoNotDeliverRestriction = partnerRestrictionItems.DoNotDeliverRestriction;
        //        dd.PreOrderRestriction = partnerRestrictionItems.PreOrderRestriction;
        //        dd.ExpirationRestriction = partnerRestrictionItems.ExpirationRestriction;
        //        dd.PreviewRestriction = partnerRestrictionItems.PreviewRestriction;
        //        dd.D2PartnerList = partnerRestrictionItems.D2PartnerList;
        //        dd.PartnersExist = 1;
        //    }

        //    // Note: one of the restrictions objects could be null.
        //    // Get Timestamp and Workflow Status from the first valid object.
        //    // If neither one is valid, leave these properties set to their default values.
        //    if (productRestrictions != null && productRestrictions.ProductExists)
        //    {
        //        dd.WorkflowStatus = GetWorkflowStatus(productRestrictions.WorkflowStatusList);
        //        dd.Timestamp = productRestrictions.Timestamp;
        //    }
        //    else if (partnerRestrictions != null)
        //    {
        //        dd.WorkflowStatus = partnerRestrictions.WorkflowStatus;
        //        dd.Timestamp = partnerRestrictions.Timestamp;
        //    }

        //    return dd;
        //}

        #region Private Classes
        private class AlbumOnlyRestriction
        {
            public bool HasUnknown;
            public bool HasRestricted;
            public bool HasNotRestricted;
            public int RuleNumber;

            public void SetProperties()
            {
                // Album Only Restriction Rule is a function of the 3 boolean flags:
                /*
                    Existing Album Only Restrictions	      Rule to
                    Unknown	Restricted	Not Restricted      Use
                    ------- ----------  --------------      -----------
                    No      No          No                  (Not possible)
                    No	    No	        Yes	                Rule 4
                    No	    Yes	        No	                Rule 2
                    No	    Yes	        Yes	                Rule 1
                    Yes	    No	        No	                Rule 3
                    Yes	    No	        Yes	                Rule 4
                    Yes	    Yes	        No	                Rule 1
                    Yes	    Yes	        Yes	                Rule 1
                 */

                // Order the restriction groups in the following order:
                // Partially Restricted, Restricted, Restricted/Unknown, Not Restricted,
                // Not Restricted/Unknown and Unknown.

                if (!HasUnknown && !HasRestricted && HasNotRestricted)
                {
                    RuleNumber = 4;
                }
                else if (!HasUnknown && HasRestricted && !HasNotRestricted)
                {
                    RuleNumber = 2;
                }
                else if (!HasUnknown && HasRestricted && HasNotRestricted)
                {
                    RuleNumber = 1;
                }
                else if (HasUnknown && !HasRestricted && !HasNotRestricted)
                {
                    RuleNumber = 3;
                }
                else if (HasUnknown && !HasRestricted && HasNotRestricted)
                {
                    RuleNumber = 4;
                }
                else if (HasUnknown && HasRestricted && !HasNotRestricted)
                {
                    RuleNumber = 1;
                }
                else if (HasUnknown && HasRestricted && HasNotRestricted)
                {
                    RuleNumber = 1;
                }
                else
                {
                    RuleNumber = 0;
                }
            }
        }

        private class GroupRestriction
        {
            public string GroupName;
            public bool HasUnknown;
            public bool HasRestricted;
            public bool HasNotRestricted;
            public string RestrictionValue;
            public int SortOrder;
            public bool IsReportable;

            public void SetProperties()
            {
                // Group Restriction is a function of the 3 boolean flags:
                /*
                    Existing Asset or Product Restrictions	Group
                    Unknown	Restricted	Not Restricted      Restriction
                    ------- ----------  --------------      -----------
                    No      No          No                  (Not possible)
                    No	    No	        Yes	                Not Restricted (Not reportable)
                    No	    Yes	        No	                Restricted
                    No	    Yes	        Yes	                Partially Restricted
                    Yes	    No	        No	                Unknown
                    Yes	    No	        Yes	                Not Restricted/Unknown
                    Yes	    Yes	        No	                Restricted/Unknown
                    Yes	    Yes	        Yes	                Partially Restricted
                 */

                // Order the restriction groups in the following order:
                // Partially Restricted, Restricted, Restricted/Unknown, Not Restricted,
                // Not Restricted/Unknown and Unknown.

                IsReportable = true; // Default

                if (!HasUnknown && !HasRestricted && HasNotRestricted)
                {
                    RestrictionValue = "Not Restricted";
                    SortOrder = 4;
                    IsReportable = false;
                }
                else if (!HasUnknown && HasRestricted && !HasNotRestricted)
                {
                    RestrictionValue = "Restricted";
                    SortOrder = 2;
                }
                else if (!HasUnknown && HasRestricted && HasNotRestricted)
                {
                    RestrictionValue = "Partially Restricted";
                    SortOrder = 1;
                }
                else if (HasUnknown && !HasRestricted && !HasNotRestricted)
                {
                    RestrictionValue = "Unknown";
                    SortOrder = 6;
                }
                else if (HasUnknown && !HasRestricted && HasNotRestricted)
                {
                    RestrictionValue = "Not Restricted/Unknown";
                    SortOrder = 5;
                }
                else if (HasUnknown && HasRestricted && !HasNotRestricted)
                {
                    RestrictionValue = "Restricted/Unknown";
                    SortOrder = 3;
                }
                else if (HasUnknown && HasRestricted && HasNotRestricted)
                {
                    RestrictionValue = "Partially Restricted";
                    SortOrder = 1;
                }
                else
                {
                    RestrictionValue = "(Not Possible)";
                    SortOrder = 7;
                }
            }
        }

        private class RestrictionGroup
        {
            public int SortOrder;
            public List<string> groupNameList;
            public string RestrictionValue;
        }

        private class Exclusivity : IComparable<Exclusivity>
        {
            public DateTime? SalesStartDate;
            public DateTime? ExclusivityEndDate;

            // This key is used to store these objects in a dictionary or hash table.
            // Unique objects are identified by a unique SalesStartDate and ExclusivityEndDate.
            public string key
            {
                get
                {
                    return SalesStartDate.ToString() + ", " + ExclusivityEndDate.ToString();
                }
            }
            public List<string> ParterNameList;

            public Exclusivity(DateTime? salesStartDate, DateTime? exclusivityEndDate)
            {
                SalesStartDate = salesStartDate;
                ExclusivityEndDate = exclusivityEndDate;
                ParterNameList = new List<string>();
            }

            // IComparable interface is used to order these objects by SalesStartDate and ExclusivityEndDate

            #region IComparable<Exclusivity> Members

            public int CompareTo(Exclusivity other)
            {
                int i = UtilParse.Compare(this.SalesStartDate, other.SalesStartDate);
                if (i != 0)
                {
                    return i;
                }
                else
                {
                    return UtilParse.Compare(this.ExclusivityEndDate, other.ExclusivityEndDate.Value);
                }
            }

            #endregion
        }

        private class SingleDate : IComparable<SingleDate>
        {
            public DateTime? Date;
            public List<string> ParterNameList;

            public SingleDate(DateTime? dt)
            {
                Date = dt;
                ParterNameList = new List<string>();
            }

            // IComparable interface is used to order these objects by Date

            #region IComparable<SingleDate> Members

            public int CompareTo(SingleDate other)
            {
                return UtilParse.Compare(this.Date, other.Date);
            }

            #endregion
        }

        #endregion

        #region Product Mapping
        public static ProductRestrictionItems GetMappedProductRestrictions(ProductRestrictions productRestrictions)
        {
            /*
              If Assets exist,
              Determine which Asset Restriction values exist for all Distribution Types within the Distribution Group.
              Else,
              Determine which Product Restriction values exist for all Distribution Types within the Distribution Group.
             */

            ProductRestrictionItems returnedValue = new ProductRestrictionItems();

            if (IsValidObject(productRestrictions))
            {
                // Object is valid.
                // Determine whether assets exist.
                if (AssetsExist(productRestrictions))
                {
                    // Assets Exist.
                    returnedValue.DistributionRestriction = ParseDistributionRestrictionAssets(productRestrictions);
                    returnedValue.AlbumOnlyRestriction = ParseAlbumOnlyAssets(productRestrictions.DistributionGroups[0].DistributionChannels[0].Assets);
                    returnedValue.PreOrderOnlyRestriction = ParsePreOrderOnlyAssets(productRestrictions.DistributionGroups[0].DistributionChannels[0].Assets);
                }
                else
                {
                    // Product Defaults Exist.
                    returnedValue.DistributionRestriction = ParseDistributionRestrictionProductDefaults(productRestrictions);
                    returnedValue.AlbumOnlyRestriction = ParseAlbumOnlyProductDefaults(productRestrictions.DistributionGroups[0].DistributionChannels[0].Product);
                    returnedValue.PreOrderOnlyRestriction = ParsePreOrderOnlyProductDefaults(productRestrictions.DistributionGroups[0].DistributionChannels[0].Product);
                }
            }
            else
            {
                returnedValue.DistributionRestriction = "";
                returnedValue.AlbumOnlyRestriction = "";
                returnedValue.PreOrderOnlyRestriction = "";
            }

            return returnedValue;
        }

        private static bool IsValidObject(ProductRestrictions productRestrictions)
        {
            // Make sure there is at least one Distribution Group and at least one Distribution Channel
            // within each group.
            if (productRestrictions.DistributionGroups == null)
            {
                return false;
            }
            if (productRestrictions.DistributionGroups.Count == 0)
            {
                return false;
            }
            foreach (DistributionGroup distributionGroup in productRestrictions.DistributionGroups)
            {
                if (distributionGroup.DistributionChannels == null)
                {
                    return false;
                }
                if (distributionGroup.DistributionChannels.Count == 0)
                {
                    return false;
                }
            }

            // Distribution Groups are valid.

            // Each Distribution Channel must either have at least one Asset or a Product or both.
            foreach (DistributionGroup distributionGroup in productRestrictions.DistributionGroups)
            {
                foreach (DistributionChannel distributionChannel in distributionGroup.DistributionChannels)
                {
                    bool hasAssets = true;
                    bool hasProduct = true;
                    if (distributionChannel.Assets == null)
                    {
                        hasAssets = false;
                    }
                    else if (distributionChannel.Assets.Count == 0)
                    {
                        hasAssets = false;
                    }
                    if (distributionChannel.Product == null)
                    {
                        hasProduct = false;
                    }
                    if (!hasAssets && !hasProduct)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #region Distribution Restrictions
        private static string ParseDistributionRestrictionAssets(ProductRestrictions productRestrictions)
        {
            List<GroupRestriction> groupRestrictionList = new List<GroupRestriction>();

            foreach (DistributionGroup distributionGroup in productRestrictions.DistributionGroups)
            {
                // Create a new item
                GroupRestriction groupRestriction = new GroupRestriction();

                // Add it to the list.
                groupRestrictionList.Add(groupRestriction);

                // Set its properties.
                groupRestriction.GroupName = distributionGroup.Name;
                foreach (DistributionChannel distributionChannel in distributionGroup.DistributionChannels)
                {
                    foreach (Asset asset in distributionChannel.Assets)
                    {
                        if (asset.Restriction == "Unknown")
                        {
                            groupRestriction.HasUnknown = true;
                        }
                        if (asset.Restriction == "Restricted")
                        {
                            groupRestriction.HasRestricted = true;
                        }
                        if (asset.Restriction == "Not Restricted")
                        {
                            groupRestriction.HasNotRestricted = true;
                        }
                    }
                }
                groupRestriction.SetProperties();
            }

            // groupRestrictionsList is now populated.
            return DistributionRestrictions(groupRestrictionList);
        }

        private static string ParseDistributionRestrictionProductDefaults(ProductRestrictions productRestrictions)
        {
            List<GroupRestriction> groupRestrictionList = new List<GroupRestriction>();

            foreach (DistributionGroup distributionGroup in productRestrictions.DistributionGroups)
            {
                // Create a new item
                GroupRestriction groupRestriction = new GroupRestriction();

                // Add it to the list.
                groupRestrictionList.Add(groupRestriction);

                // Set its properties.
                groupRestriction.GroupName = distributionGroup.Name;
                foreach (DistributionChannel distributionChannel in distributionGroup.DistributionChannels)
                {
                    if (distributionChannel.Product != null)
                    {
                        if (distributionChannel.Product.Restriction == "Unknown")
                        {
                            groupRestriction.HasUnknown = true;
                        }
                        if (distributionChannel.Product.Restriction == "Restricted")
                        {
                            groupRestriction.HasRestricted = true;
                        }
                        if (distributionChannel.Product.Restriction == "Not Restricted")
                        {
                            groupRestriction.HasNotRestricted = true;
                        }
                    }
                }
                groupRestriction.SetProperties();
            }

            // groupRestrictionsList is now populated.
            return DistributionRestrictions(groupRestrictionList);
        }

        private static string DistributionRestrictions(List<GroupRestriction> groupRestrictionList)
        {
            // Create a sorted list of restriction groups.
            SortedList<int, RestrictionGroup> sortedList = new SortedList<int, RestrictionGroup>();
            foreach (GroupRestriction groupRestriction in groupRestrictionList)
            {
                if (groupRestriction.IsReportable)
                {
                    RestrictionGroup rg = null;
                    if (sortedList.ContainsKey(groupRestriction.SortOrder))
                    {
                        rg = sortedList[groupRestriction.SortOrder];
                    }
                    else
                    {
                        rg = new RestrictionGroup();
                        rg.SortOrder = groupRestriction.SortOrder;
                        rg.RestrictionValue = groupRestriction.RestrictionValue;
                        rg.groupNameList = new List<string>();
                        sortedList.Add(rg.SortOrder, rg);
                    }
                    rg.groupNameList.Add(groupRestriction.GroupName);
                }
            }

            // For each restriction group, join the Distribution Group names via the “standard join rule”.

            List<string> restrictionClauseList = new List<string>();

            // Traverse the list in sorted order, using the standard join rule to combine the group names.
            foreach (KeyValuePair<int, RestrictionGroup> kvp in sortedList)
            {
                RestrictionGroup restrictionGroup = kvp.Value;
                restrictionClauseList.Add(
                  restrictionGroup.RestrictionValue + " for " + StandardJoinRuleSorted(restrictionGroup.groupNameList));
            }

            string joinedString = "None.";
            if (restrictionClauseList.Count > 0)
            {
                // Combine the restriction clauses with ", " delimiters.
                joinedString = string.Join(", ", restrictionClauseList.ToArray());

                // And add a final period.
                joinedString += ".";
            }

            return joinedString;
        }
        #endregion

        #region Album Only Restrictions
        private static string ParseAlbumOnlyProductDefaults(Product product)
        {
            int ruleNumber = 0;
            if (product.AlbumOnly == "Restricted")
            {
                ruleNumber = 2;
            }
            else if (product.AlbumOnly == "Unknown")
            {
                ruleNumber = 3;
            }
            else if (product.AlbumOnly == "Not Restricted")
            {
                ruleNumber = 4;
            }

            return AlbumOnlyClause(ruleNumber);
        }

        private static string ParseAlbumOnlyAssets(List<Asset> assets)
        {
            // Create an AlbumOnlyRestriction object to keep track of which restriction types are in these assets.
            AlbumOnlyRestriction albumOnlyRestriction = new AlbumOnlyRestriction();
            foreach (Asset asset in assets)
            {
                if (asset.AlbumOnly == "Unknown")
                {
                    albumOnlyRestriction.HasUnknown = true;
                }
                else if (asset.AlbumOnly == "Restricted")
                {
                    albumOnlyRestriction.HasRestricted = true;
                }
                else if (asset.AlbumOnly == "Not Restricted")
                {
                    albumOnlyRestriction.HasNotRestricted = true;
                }
            }
            albumOnlyRestriction.SetProperties();

            if (albumOnlyRestriction.RuleNumber == 1)
            {
                return AlbumOnlyRule1(assets);
            }
            else
            {
                return AlbumOnlyClause(albumOnlyRestriction.RuleNumber);
            }
        }

        private static string AlbumOnlyRule1(List<Asset> assets)
        {
            /*
              Rule 1: Any Assets but not all have Album Only values of Restricted -
              Join the Asset (track) names using the standard join rule.
              Create a sentence in one of the following forms:
              “Album Only restriction for: Sweet Child of Mine.”
              “Album Only restriction for: Sweet Child of Mine and Welcome to the Jungle.”
              “Album Only restriction for: Sweet Child of Mine, Welcome to the Jungle, and Mr. Brownstone.”
             */

            // Set unambiguous track titles.
            SetUnambiguousTrackTitles(assets);

            List<string> trackTitleList = new List<string>();
            foreach (Asset asset in assets)
            {
                if (asset.AlbumOnly == "Restricted")
                {
                    trackTitleList.Add(asset.UnambiguousTrackTitle);
                }
            }

            return "Album Only restriction for: "
              + StandardJoinRule(trackTitleList) + ".";
        }

        /// <summary>
        /// Sets unambiguous track titles by appending track numbers if necessary
        /// to disambiguate identical titles.
        /// </summary>
        /// <param name="assets"></param>
        /// <returns></returns>
        private static void SetUnambiguousTrackTitles(List<Asset> assets)
        {
            // 1) Create a list of track titles that have duplicates.
            List<string> originalTrackTitles = new List<string>();
            List<string> duplicateTrackTitles = new List<string>();
            foreach (Asset asset in assets)
            {
                if (!originalTrackTitles.Contains(asset.TrackTitle))
                {
                    originalTrackTitles.Add(asset.TrackTitle);
                }
                else
                {
                    // Found a duplicate.
                    duplicateTrackTitles.Add(asset.TrackTitle);
                }
            }

            // 2) Alter each duplicate title by appending " (Volumn v Track t)" to it.
            foreach (Asset asset in assets)
            {
                if (duplicateTrackTitles.Contains(asset.TrackTitle))
                {
                    asset.UnambiguousTrackTitle = asset.TrackTitle + " (Volume " + asset.Volume + " Track " + asset.TrackNumber + ")";
                }
                else
                {
                    asset.UnambiguousTrackTitle = asset.TrackTitle;
                }
            }
        }

        private static string AlbumOnlyClause(int ruleNumber)
        {
            string returnedValue = "";

            switch (ruleNumber)
            {
                case 2:
                    returnedValue = "No tracks can be sold individually.";
                    break;
                case 3:
                    returnedValue = "Album Only Restriction is unknown.";
                    break;
                case 4:
                    returnedValue = "No Album Only restriction.";
                    break;
                default:
                    break;
            }

            return returnedValue;
        }
        #endregion

        #region PreOrder Only Restricitions
        /*
			Case 1: No PreOrder and no IG for all tracks => no sentence.
			Case 2: All tracks are PreOrder => PreOrder Only for all tracks.
			Case 3: All tracks are IG with the same SSD or no SSD =>
				Instant Gratification [starting on xx/yy/zzzz] for all tracks.
			Case 4: Other => Compose sentence(s).

			Note that it is not possible for a track to have both PreOrder Only and IG.
				Track SSD is used only for IG.
		 */

        private static string ParsePreOrderOnlyAssets(List<Asset> assets)
        {
            // Set unambiguous track titles.
            SetUnambiguousTrackTitles(assets);

            // Early exit if no assets.
            if (assets.Count == 0)
                return "";

            // Get case number
            int caseNumber = GetPreOrderOnlyCaseNumber(assets);

            // If case 1, 2, or 3:
            switch (caseNumber)
            {
                case 1:
                case 2:
                case 3:
                    return PreOrderOnlyClause(caseNumber, assets[0].SalesStartDate);
            }

            // else, case number is 4 (complicated)
            return PreOrderOnlyClause(assets);

        }

        private static int GetPreOrderOnlyCaseNumber(List<Asset> assets)
        {
            int caseNumber = 4; // default is that the list contains different restrictions.

            var customList = assets.Select(x => new { PreOrderOnly = x.PreOrderOnly, SalesStartDate = x.SalesStartDate }).Distinct().ToList();

            if (customList.Count == 1)
            {
                // If custom list contains only one item, PreOrderOnly restrictions for all assets are the same.
                switch (customList[0].PreOrderOnly)
                {
                    case "PreOrder Only":
                        caseNumber = 2;
                        break;
                    case "Instant Gratification":
                        caseNumber = 3;
                        break;
                    case "":
                    default:
                        caseNumber = 1;
                        break;
                }
            }

            return caseNumber;
        }

        private static string PreOrderOnlyClause(List<Asset> assets)
        {
            // The clause is composed of up to 3 items: PreOrderOnly, Instant Gratification with no SSD,
            // and Instant Gritifaction with SSDs.

            List<string> clauses = new List<string>();

            List<Asset> preOrderOnlyList = assets.Where(x => x.PreOrderOnly == "PreOrder Only").ToList();
            if (preOrderOnlyList.Count > 0)
            {
                clauses.Add(GetPreOrderOnlyClause(preOrderOnlyList));
            }

            List<Asset> igListWithoutSalesStartDates = assets.Where(x => x.PreOrderOnly == "Instant Gratification" && x.SalesStartDate == "").ToList();
            if (igListWithoutSalesStartDates.Count > 0)
            {
                clauses.AddRange(GetInstantGratificationClauses(igListWithoutSalesStartDates, false));
            }

            List<Asset> igListWithSalesStartDates = assets.Where(x => x.PreOrderOnly == "Instant Gratification" && x.SalesStartDate != "").ToList();
            if (igListWithSalesStartDates.Count > 0)
            {
                clauses.AddRange(GetInstantGratificationClauses(igListWithSalesStartDates, true));
            }

            return string.Join("  ", clauses.ToArray());
        }

        private static string GetPreOrderOnlyClause(List<Asset> assets)
        {
            List<string> trackTitleList = assets.Select(x => x.UnambiguousTrackTitle).ToList();
            return "PreOrder Only for: " + StandardJoinRule(trackTitleList) + ".";
        }

        private static List<string> GetInstantGratificationClauses(List<Asset> assets, bool includeDates)
        {
            List<string> clauses = new List<string>();

            if (includeDates)
            {
                // Create an ordered list of sales start dates.
                var dateList = assets.Select(x => new { SalesStartDate = x.SalesStartDate, SalesStartDt = x.SalesStartDt }).Distinct().OrderBy(x => x.SalesStartDt.Value).ToList();
                foreach (var item in dateList)
                {
                    // create a clause per sales start date, ordered by sales strart date.
                    List<string> trackTitleList = assets.Where(x => x.SalesStartDt.Value == item.SalesStartDt.Value).Select(x => x.UnambiguousTrackTitle).ToList();
                    clauses.Add("Instant Gratification starting " + item.SalesStartDate + " for: " + StandardJoinRule(trackTitleList) + ".");
                }
            }
            else
            {
                List<string> trackTitleList = assets.Select(x => x.UnambiguousTrackTitle).ToList();
                clauses.Add("Instant Gratification for: " + StandardJoinRule(trackTitleList) + ".");
            }

            return clauses;
        }

        private static string ParsePreOrderOnlyProductDefaults(Product product)
        {
            int caseNumber = 1; // default
            switch (product.PreOrderOnly)
            {
                case "PreOrder Only":
                    caseNumber = 2;
                    break;
                case "Instant Gratification":
                    caseNumber = 3;
                    break;
            }
            return PreOrderOnlyClause(caseNumber, product.SalesStartDate);
        }

        private static string PreOrderOnlyClause(int caseNumber, string salesStartDate)
        {
            switch (caseNumber)
            {
                case 2:
                    return "PreOrder Only for all tracks.";
                case 3:
                    string dateClause = string.IsNullOrEmpty(salesStartDate) ? "" : (" starting on " + salesStartDate);
                    return "Instant Gratification" + dateClause + " for all tracks.";
                default:
                    return "";
            }
        }

        #endregion

        #region Workflow Status
        public static string GetWorkflowStatus(List<string> workflowStatusList)
        {
            // workflowStatusList is a list of values, each of which is either "Not Started", "In Progress", "Complete", or "Not Applicable".
            // The final value is a function of which of these values exists on the list.
            // The basic idea is that if only "Not Started" and "Not Applicable" are on the list, use "Not Started";
            // If only "Complete" and "Not Applicable" are on the list, use "Complete".
            // If more than one value is on the list, use "In Progress".
            // If no values are on the list or the list is empty, use "".
            // The truth table below fully specifies all possible results.

            // Not Started / In Progress / Complete  / Not Applicable      Result
            //     0             0             0            0              ""
            //     0             0             0            1              Not Applicable
            //     0             0             1            0              Complete
            //     0             0             1            1              Complete
            //     0             1             0            0              In Progress
            //     0             1             0            1              In Progress
            //     0             1             1            0              In Progress
            //     0             1             1            1              In Progress
            //     1             0             0            0              Not Started
            //     1             0             0            1              Not Started
            //     1             0             1            0              In Progress
            //     1             0             1            1              In Progress
            //     1             1             0            0              In Progress
            //     1             1             0            1              In Progress
            //     1             1             1            0              In Progress
            //     1             1             1            1              In Progress

            bool hasNotStarted = false;
            bool hasInProgress = false;
            bool hasComplete = false;
            bool hasNotApplicable = false;

            foreach (string workflowStatus in workflowStatusList)
            {
                if (workflowStatus == "Not Started")
                {
                    hasNotStarted = true;
                    continue;
                }
                if (workflowStatus == "In Progress")
                {
                    hasInProgress = true;
                    continue;
                }
                if (workflowStatus == "Complete")
                {
                    hasComplete = true;
                    continue;
                }
                if (workflowStatus == "Not Applicable")
                {
                    hasNotApplicable = true;
                    continue;
                }
            }

            if (!hasNotStarted && !hasInProgress && !hasComplete && !hasNotApplicable)
            {
                return "";
            }
            if (!hasNotStarted && !hasInProgress && !hasComplete && hasNotApplicable)
            {
                return "Not Applicable";
            }
            if (!hasNotStarted && !hasInProgress && hasComplete)
            {
                return "Complete";
            }
            if (hasNotStarted && !hasInProgress && !hasComplete)
            {
                return "Not Started";
            }
            return "In Progress";

        }
        #endregion

        #endregion

        #region Utilities
        // Standard Join Rule
        // If there is only one item, use it.
        // If there are two items, join them with the word “and”.
        // If there are three or more items, join all but the last two with a comma and space
        // and join the last two with a comma, space, and the word “and”.
        private static string StandardJoinRule(List<string> list)
        {
            if (list.Count == 1)
            {
                return list[0];
            }

            if (list.Count == 2)
            {
                return list[0] + " and " + list[1];
            }

            // Three or more items:
            // Create a new list with all but the last item.
            List<string> shorterList = new List<string>();
            for (int i = 0; i < list.Count - 1; i++)
            {
                shorterList.Add(list[i]);
            }

            // Join the first n-1 items with ", "
            string joinedItems = string.Join(", ", shorterList.ToArray());

            // Add the last item with ", and ".
            joinedItems += ", and " + list[list.Count - 1];

            return joinedItems;
        }

        private static string StandardJoinRuleSorted(List<string> list)
        {
            List<string> newList = new List<string>(list);
            newList.Sort();
            return StandardJoinRule(newList);
        }

        // Determine if assests exist.  This is determined by the number of Assets
        // in the first DistributionGroup's first Distribution Channel.
        private static bool AssetsExist(ProductRestrictions productRestrictions)
        {
            return (productRestrictions.DistributionGroups[0].DistributionChannels[0].Assets.Count > 0);
        }
        #endregion

        #region Partner Mapping

        public static PartnerRestrictionItems GetMappedPartnerRestrictions(PartnerRestrictions partnerRestrictions)
        {
            PartnerRestrictionItems returnedValue = new PartnerRestrictionItems();

            if (IsValidObject(partnerRestrictions))
            {
                // Bug 21454 - Dates need to be removed if "DoNotDeliver" is true.
                PartnerRestrictions partnerRestrictionsCopy = SerializationHelper.Copy<PartnerRestrictions>(partnerRestrictions);
                RemoveDatesForDoNotDeliverPartners(partnerRestrictionsCopy);

                // Requirement 21932:
                /*
                 * If at least one business partner has an Exclusive End Date value of "12/31/9999"
                 * applied in D2 (meaning, an Exclusive In Perpetuity restriction has been applied)
                 * then GDRS should not show any Do Not Deliver information for any business partner
                 * in the Partner Restrictions text area.
                 * Rationale: When an EIP restriction is applied in D2, an Exclusivity End Date value
                 * of 12/31/9999 is saved in the database. This event then results in a "Do Not Deliver"
                 * restriction applied at the Partner Default level to all other partners.
                 * When this occurs under current implementation, all other partners with this restriction
                 * are listed out in GDRS. Sandi and Mark indicated (on 3/3/10)  that when there is an EIP
                 * restriction applied, GDRS users should not see each partner which has in turn inherited
                 * the "Do Not Deliver" restriction in the GDRS screen.
                */

                string exclusivityRestrictions;
                GetExclusivityClause(partnerRestrictionsCopy, out exclusivityRestrictions);
                returnedValue.ExclusivityRestriction = exclusivityRestrictions;

                returnedValue.DoNotDeliverRestriction = GetDoNotDeliverClause(partnerRestrictionsCopy);
                returnedValue.PreOrderRestriction = GetPreOrderClause(partnerRestrictionsCopy);
                returnedValue.ExpirationRestriction = GetExpirationDateClause(partnerRestrictionsCopy);
                returnedValue.PreviewRestriction = GetPreviewClause(partnerRestrictionsCopy);
                returnedValue.D2PartnerList = GetD2PartnerList(partnerRestrictionsCopy);
            }
            else
            {
                returnedValue.ExclusivityRestriction = "Invalid Partner Restrictions Object - D2 to GDRS Mapping Failed.";
            }

            return returnedValue;
        }

        private static void RemoveDatesForDoNotDeliverPartners(PartnerRestrictions partnerRestrictions)
        {
            foreach (Partner partner in partnerRestrictions.Partners)
            {
                if (partner.DoNotDeliver)
                {
                    partner.ExclusivityEndDate = null;
                    partner.ExpirationDate = null;
                    partner.PreOrderDate = null;
                    partner.SalesStartDate = null;
                }
            }
        }

        private static void GetExclusivityClause(PartnerRestrictions partnerRestrictions, out string exclusivityRestrictions)
        {
            exclusivityRestrictions = "";
            /*
              Identify the partners which have ExclusivityEndDate not equal to null.
              If there are any:
                Create three lists:
                  1) Partners with no SalesStartDate and an ExclusivityEndDate of 12/31/9999.
                  2) Partners with a SalesStartDate and an ExclusivityEndDate of 12/31/9999.
                  3) Partners with a SalesStartDate and an ExclusivityEndDate not equal to 12/31/9999.
                Within each list, group partners by SalesStartDate and ExclusivityEndDate.
                Order the groups by SalesStartDate and ExclusivityEndDate.
                For each group, join the partner names with the standard join rule.
                For list 1 there will be only one group, if any. But to be general, create exclusivity clauses of the form:
                  "P1, P2, and P3"
                Join the exclusivity clauses with the standard join rule, prepend "Exclusive in Perpetuity to ", and terminate with a period.
                For list 2, create exclusivity clauses of the form:
                  "P1, P2, and P3 from SalesStartDate"
                Join the exclusivity clauses with the standard join rule, prepend "Exclusive in Perpetuity to ", and terminate with a period.
                For list 3, create exclusivity clauses of the form:
                  "P1, P2, and P3 from SalesStartDate to ExclusivityEndDate"
                Join the exclusivity clauses with the standard join rule, prepend "Exclusive to ", and terminate with a period.
                This should result in three types of final exclusivity clauses of the following forms:
                  “Exclusive in Perpetuity to P1, P2, and P3”
                  “Exclusive in Perpetuity to P1, and P2 from SalesStartDate1, P3 from SalesStartDate2”
                  “Exclusive to P1 and P2 from SalesStartDate1 to ExclusivityEndDate1, P3 from SalesStartDate2 to ExclusivityEndDate2”
                The first form is used when the ExclusivityEndDate is 12/31/9999 and the SalesStartDate is null.
                The second form is used when the ExclusivityEndDate is 12/31/9999 and the SalesStartDate has a value.
                Join the final exclusivity clauses with comma and space separators and terminate the last one with a period.

              Sample final sentence form:
                  "Exclusive in Perpetuity to iTunes,
                   Exclusive in Perpetuity to Walmart from 9/5/2009,
                   Exclusive to Apple and Target from 9/1/2009 to 10/1/2009 and Napster from 9/5/2009 to 9/27/2009."
            */

            // Determine if any Exclusivity End dates are specified.
            if (HasExclusivityEndDates(partnerRestrictions))
            {
                // Group them by Sales Start Date and Exclusivity End Date.

                // Create a dictionary object to store Exclusivity objects by their keys,
                // which are a combination of Sales Start Date and Exclusivity End Date.
                Dictionary<string, Exclusivity> dict = new Dictionary<string, Exclusivity>();

                // Add items to the dictionary.
                foreach (Partner partner in partnerRestrictions.Partners)
                {
                    if (partner.ExclusivityEndDate.HasValue)
                    {
                        Exclusivity exclusivity = new Exclusivity(partner.SalesStartDate, partner.ExclusivityEndDate);
                        string key = exclusivity.key;
                        if (dict.ContainsKey(key))
                        {
                            // Replace the new exclusivity object with the one that's already in the dictionary.
                            exclusivity = dict[key];
                        }
                        else
                        {
                            // The key was not found, so add this Exclusivity object to the dictionary.
                            dict.Add(key, exclusivity);
                        }
                        // And add the partner name to the list of partner names.
                        exclusivity.ParterNameList.Add(partner.Name);
                    }
                }

                // Create a list for each exclusivity type.
                List<Exclusivity> listOne = new List<Exclusivity>();
                List<Exclusivity> listTwo = new List<Exclusivity>();
                List<Exclusivity> listThree = new List<Exclusivity>();

                // Populate the lists.
                foreach (KeyValuePair<string, Exclusivity> kvp in dict)
                {
                    if (kvp.Value.ExclusivityEndDate.Value == new DateTime(9999, 12, 31))
                    {
                        // No end date.
                        if (kvp.Value.SalesStartDate.HasValue)
                        {
                            // Has a sales start date but no end date.  -- Type Two.
                            listTwo.Add(kvp.Value);
                        }
                        else
                        {
                            // Has no sales start date either.  -- Type One.
                            listOne.Add(kvp.Value);
                        }
                    }
                    else
                    {
                        // End date is specified.  Assume that start date is specified as well.  -- Type Three.
                        listThree.Add(kvp.Value);
                    }
                }

                // Create exclusivity clauses.
                string exclusivityClauseOne = GetExclusivityClause("Exclusive in Perpetuity to ", listOne);
                string exclusivityClauseTwo = GetExclusivityClause("Exclusive in Perpetuity to ", listTwo);
                string exclusivityClauseThree = GetExclusivityClause("Exclusive to ", listThree);

                // Add clauses to a list.
                List<string> exclusivityClauseList = new List<string>();
                if (exclusivityClauseOne != "")
                {
                    exclusivityClauseList.Add(exclusivityClauseOne);
                }
                if (exclusivityClauseTwo != "")
                {
                    exclusivityClauseList.Add(exclusivityClauseTwo);
                }
                if (exclusivityClauseThree != "")
                {
                    exclusivityClauseList.Add(exclusivityClauseThree);
                }

                // Join the clauses with ", " and terminate with "."
                exclusivityRestrictions = string.Join(", ", exclusivityClauseList.ToArray()) + ".";
            }
        }

        private static string GetExclusivityClause(string exclusivityPrefix, List<Exclusivity> list)
        {
            string result = "";

            if (list.Count > 0)
            {
                list.Sort();
                List<string> exclusivityClauseList = new List<string>();

                // Create an exclusivity clause from each item on the list.
                foreach (Exclusivity item in list)
                {
                    if (item.ExclusivityEndDate.Value != new DateTime(9999, 12, 31))
                    {
                        string s =
                          StandardJoinRuleSorted(item.ParterNameList)
                          + " from "
                          + item.SalesStartDate.Value.ToShortDateString()
                          + " to "
                          + item.ExclusivityEndDate.Value.ToShortDateString();
                        exclusivityClauseList.Add(s);
                    }
                    else if (item.SalesStartDate.HasValue)
                    {
                        string s =
                          StandardJoinRuleSorted(item.ParterNameList)
                          + " from "
                          + item.SalesStartDate.Value.ToShortDateString();
                        exclusivityClauseList.Add(s);
                    }
                    else
                    {
                        string s =
                          StandardJoinRuleSorted(item.ParterNameList);
                        exclusivityClauseList.Add(s);
                    }
                }

                result = exclusivityPrefix + StandardJoinRule(exclusivityClauseList);
            }

            return result;
        }

        private static string GetDoNotDeliverClause(PartnerRestrictions partnerRestrictions)
        {
            /*
              Identify the partners which have DoNotDeliver equal to true.
              If any are found,
                Join the partner names with the standard join rule.
                Create a sentence of the form:
                  “Do not deliver to P1, P2, and P3.”

              Sample final sentence form:
                  “Do not deliver to Apple and Target.”
        
             NEW REQUIREMENT: If any partner has a value in the Exclusivity End Date field
             and the product default “Do Not Deliver” value is true (i.e. DND exists),
             then GDRS text must display “Do not deliver to all other partners.” after
             the display of the exclusivity information. If any partner has a value in
             the Exclusivity End Date field and the product default “Do Not Deliver” value is false
             (i.e., no DND exists), then GDRS text must display any individual partners
             carrying Do Not Deliver restrictions as per REQ 11540 (next requirement).
       
             [Notes for internal use] This is a change request to current implementation,
             where GDRS lists out all partner names if DND=true at the product default
             level and there is an exclusivity applied to a partner.
       
             Note: Per CR 30346, “Do not deliver to all other partners.” is eliminated.

             If partners are selected in D2 for “Do Not Deliver” and the product default
             “Do Not Deliver” is False (i.e., no DND exists), then GDRS text to combine
             all partners in text to read: "Do not deliver to Partner A, Partner B, and Partner C etc.”
             If these partners previously had Exclusivity Dates, Preorder Date, Expiration Date,
             or Exclusive in Perpetuity fields assigned in D2 prior to selecting “Do not Deliver”,
             all previously assigned data should be erased in GDRS, thereby matching D2 data
             representation. If “Do Not Deliver” partners were grouped with other partners
             in GDRS (for instance, "Exclusive in Perpetuity to Partner A, Partner B, Partner C”),
             such partners should be removed from a group. (REQ 11540)

             * 
             *  HasExclusivity  HasDoNotDeliver  ProductDefault  Result
             *       0                0               0            -
             *       0                0               1            -
             *       0                1               0            Do not deliver to A, B, and C.
             *       0                1               1            -
             *       1                0               0            -
             *       1                0               1            -
             *       1                1               0            Do not deliver to A, B, and C.
             *       1                1               1            -
             * 
             * 
            */

            string returnedValue = "";
            if (HasDoNotDeliver(partnerRestrictions) && (false == partnerRestrictions.ProductDefaultDoNotDeliver))
            {
                List<string> list = new List<string>();
                foreach (Partner partner in partnerRestrictions.Partners)
                {
                    if (partner.DoNotDeliver)
                    {
                        list.Add(partner.Name);
                    }
                }
                returnedValue = "Do not deliver to " + StandardJoinRuleSorted(list) + ".";
            }
            return returnedValue;
        }

        private static bool HasDoNotDeliver(PartnerRestrictions partnerRestrictions)
        {
            foreach (Partner partner in partnerRestrictions.Partners)
            {
                if (partner.DoNotDeliver)
                {
                    return true;
                }
            }
            return false;
        }

        private static string GetPreviewClause(PartnerRestrictions partnerRestrictions)
        {
            /*
              Identify the partners which have PreviewRestriction equal to true.
              If there are no partners with a preview restriction, return "".
              If all partners have a preview restriction, return "No preview for any partners.".
              If there are restrictions but not all partners have restrictions,
                Form two sentences:
                  1) Join the names of restricted partners with the standard join rule.
                     Create a sentence of the form: “No preview for P1, P2, and P3.”
                  2) Join the names of unrestricted partners with the standard join rule.
                     Create a sentence of the form: “Preview allowed only for P1, P2, and P3.”
                Choose the shorter of sentence 1 and sentence 2.  Use sentence 2 if they are the same length.
            */

            string returnedValue = "";
            if (partnerRestrictions.Partners.Where(s => s.PreviewRestriction).Any())
            {
                // At least one restriction exists.
                if (partnerRestrictions.Partners.Where(s => !s.PreviewRestriction).Any())
                {
                    // At least one non-restriction exists.
                    List<string> list1 = partnerRestrictions.Partners.Where(s => s.PreviewRestriction).Select(s => s.Name).ToList();
                    string sentence1 = "No preview for " + StandardJoinRuleSorted(list1) + ".";

                    List<string> list2 = partnerRestrictions.Partners.Where(s => !s.PreviewRestriction).Select(s => s.Name).ToList();
                    string sentence2 = "Preview allowed only for " + StandardJoinRuleSorted(list2) + ".";

                    if (sentence2.Length <= sentence1.Length)
                        returnedValue = sentence2;
                    else
                        returnedValue = sentence1;
                }
                else
                {
                    // They are ALL restricted.
                    returnedValue = "No preview for any partners.";
                }
            }
            return returnedValue;
        }

        private static string GetPreOrderClause(PartnerRestrictions partnerRestrictions)
        {
            /*
              Identify the partners which have PreOrderDate not equal to null.
              If there are any,
                Group them by PreOrderDate.
                Order the groups by PreOrderDate.
                For each group, join the partner names with the standard join rule.
                Create a pre order clause of the form:
                  “P1, P2, and P3 on PreOrderDate”
                Join the pre order clauses with the standard join rule and terminate the last one with a period.
                Prepend the words "Automatic Preorder for ".

              Sample final sentence form:
                  “Automatic Preorder for Apple and Target on 8/1/2009, Napster on 8/5/2009, and Walmart on 8/6/2009.”
            */

            string returnedValue = "";

            if (HasPreOrder(partnerRestrictions))
            {
                // Create a dictionary of SingleDate objects
                Dictionary<DateTime, SingleDate> dict = new Dictionary<DateTime, SingleDate>();
                foreach (Partner partner in partnerRestrictions.Partners)
                {
                    if (partner.PreOrderDate.HasValue)
                    {
                        SingleDate preOrder = new SingleDate(partner.PreOrderDate);
                        if (dict.ContainsKey(partner.PreOrderDate.Value))
                        {
                            // Replace with reference to existing item in dictionary.
                            preOrder = dict[partner.PreOrderDate.Value];
                        }
                        else
                        {
                            // Add this item to the dictionary.
                            dict.Add(partner.PreOrderDate.Value, preOrder);
                        }
                        // And add the partner name to the list.
                        preOrder.ParterNameList.Add(partner.Name);
                    }
                }

                // Extract the SingleDate objects into a list and sort them (by Date).
                List<SingleDate> list = new List<SingleDate>();
                foreach (KeyValuePair<DateTime, SingleDate> kvp in dict)
                {
                    list.Add(kvp.Value);
                }
                list.Sort();

                // Create a list of preOrder Clauses.
                List<string> preOrderClauseList = new List<string>();
                foreach (SingleDate item in list)
                {
                    string s =
                      StandardJoinRuleSorted(item.ParterNameList)
                      + " on "
                      + item.Date.Value.ToShortDateString();
                    preOrderClauseList.Add(s);
                }

                // Join the pre order clauses with the standard join rule and terminate the last one with a period.
                // Prepend the words "Automatic Preorder ".
                returnedValue = "Automatic Preorder for " + StandardJoinRule(preOrderClauseList) + ".";
            }

            return returnedValue;
        }

        private static bool HasPreOrder(PartnerRestrictions partnerRestrictions)
        {
            foreach (Partner partner in partnerRestrictions.Partners)
            {
                if (partner.PreOrderDate.HasValue)
                {
                    return true;
                }
            }
            return false;
        }

        private static string GetExpirationDateClause(PartnerRestrictions partnerRestrictions)
        {
            /*
              Identify the partners which have ExpirationDate not equal to null.
              If there are any,
                Group them by ExpirationDate.
                Order the groups by ExpirationDate.
                If there is only one group and all partners have an ExpirationDate set,
                    Create the sentence:
                        “Expires for all partners on ExpirationDate.”
                If there is more than one group, or not all partners have an ExpirationDate set,
                  For each group, join the partner names with the standard join rule.
                  Create an expiration clause of the form:
                    “Expires for P1, P2, and P3 on ExpirationDate”
                  Join the expiration clauses with comma and space separators and terminate the last one with a period.

                Sample final sentence form:
                    “Expires for Apple and Target on 9/1/2010, Napster on 9/5/2010, Walmart on 9/6/2010.”
 
             NEW REQUIREMENT: Regarding display text for the Expiration Date field,
             GDRS must display text according to the following rules:
             • If an Expiration Date value is provided at the product default level,
               then GDRS text to read “Expires for all partners on Date 1.”
             • If there is an Expiration Date value provided at the product default level
               and also an Expiration Date for a partner which is different from
               the product default value (i.e., a partner override), then GDRS will list
               the ‘all partners’ text in the last bullet and also the differing expiration
               date for the specific partner, i.e. “Expires for all partners on Date 1.
               Expires for Partner A on Date 2.”
             • If there is no Expiration Date at the product default level (null)
               and an Expiration Date value exists for a specific partner, then GDRS
               will explicitly list out the Expiration Date value for that partner. I.e.,
               “Expires for Partner A on Date 1.”  If multiple partners are selected in D2
               for expiration on the same date and the product default Expiration Date value
               is null, then GDRS to aggregate partners in the same sentence,
               i.e. "Expires for Partner A, Partner B, Partner C on Date 1."
             • If an Expiration date value of 12/31/9999 is provided for any partner,
               then GDRS text to display “Does not expire for Partner A.”  If more than one
               partner have an Expiration Date value of 12/31/9999, then GDRS to aggregate
               partners in the same sentence, i.e. “Does not expire for Partner A, Partner B, Partner C.”
             • [Note for internal use] To support these rules, the code in work item 23850 will
               need to be undone. That previous code was put in place to avoid the Non Commerical
               partners (i.e. ignore expiration if 12/31/9999 was provided), but later we added
               code to completely ignore the Non Commercial partner type.
             */

            string returnedValue = "";

            #region Create a dictionary of SingleDate objects
            Dictionary<DateTime, SingleDate> dict = new Dictionary<DateTime, SingleDate>();

            foreach (Partner partner in partnerRestrictions.Partners)
            {
                // Include only partners that have overridden expiration dates.
                if (HasOverriddenExpirationDate(
                  partnerRestrictions.ProductDefaultExpirationDate, partner.ExpirationDate))
                {
                    // Assume that this ExpirationDate is not yet in the dictionary.
                    SingleDate expiration = new SingleDate(partner.ExpirationDate);
                    if (dict.ContainsKey(partner.ExpirationDate.Value))
                    {
                        // Assumption is not correct.  Replace with reference to existing item in dictionary.
                        expiration = dict[partner.ExpirationDate.Value];
                    }
                    else
                    {
                        // Assumption is correct.  Add this item to the dictionary.
                        dict.Add(partner.ExpirationDate.Value, expiration);
                    }
                    // Add the partner name to the list.
                    expiration.ParterNameList.Add(partner.Name);
                }
            }
            #endregion

            #region Extract the SingleDate objects into a list and sort them (by Date).
            List<SingleDate> list = new List<SingleDate>();
            foreach (KeyValuePair<DateTime, SingleDate> kvp in dict)
            {
                list.Add(kvp.Value);
            }
            list.Sort();
            #endregion

            // Create a list of expiration Clauses.
            List<string> expirationClauseList = new List<string>();

            #region All Partners Clause
            // If the product default expiration date is set,
            // Create the sentence:  “Expires for all partners on ExpirationDate.”
            if (partnerRestrictions.ProductDefaultExpirationDate.HasValue)
            {
                expirationClauseList.Add(
                  "Expires for all partners on "
                  + partnerRestrictions.ProductDefaultExpirationDate.Value.ToShortDateString()
                  );
            }
            #endregion

            #region Overridden Expiration Dates
            List<string> partialExpirationClauseList = new List<string>();
            foreach (SingleDate item in list)
            {
                #region Determine if item should be included.
                bool includeItem = false;
                if (item.Date.Value != new DateTime(9999, 12, 31))
                {
                    if (partnerRestrictions.ProductDefaultExpirationDate.HasValue)
                    {
                        if (partnerRestrictions.ProductDefaultExpirationDate.Value != item.Date.Value)
                        {
                            includeItem = true;  // Overridden Expiration Date.
                        }
                    }
                    else
                    {
                        includeItem = true;  // Normal Expiration Date (no default expiration date).
                    }
                }
                #endregion

                #region Include item
                if (includeItem)
                {
                    string s =
                      StandardJoinRuleSorted(item.ParterNameList)
                      + " on "
                      + item.Date.Value.ToShortDateString();
                    partialExpirationClauseList.Add(s);
                }
                #endregion
            }
            if (partialExpirationClauseList.Count > 0)
            {
                expirationClauseList.Add("Expires for " + StandardJoinRule(partialExpirationClauseList));
            }
            #endregion

            #region Does Not Expire Clause
            // If there are any partners with 12/31/9999 expiration dates,
            // Create the sentence: “Does not expire for A, B, and C.”
            bool includeDoesNotExpireClause = false;
            SingleDate doesNotExpireSingleDate = null;
            foreach (SingleDate item in list)
            {
                if (item.Date.Value == new DateTime(9999, 12, 31))
                {
                    includeDoesNotExpireClause = true;
                    doesNotExpireSingleDate = item;
                    break;
                }
            }
            if (includeDoesNotExpireClause)
            {
                expirationClauseList.Add(
                  "Does not expire for "
                  + StandardJoinRuleSorted(doesNotExpireSingleDate.ParterNameList)
                  );
            }
            #endregion

            if (expirationClauseList.Count > 0)
            {
                // Join them with ", " and terminate with "."
                returnedValue = string.Join(", ", expirationClauseList.ToArray()) + ".";
            }

            return returnedValue;
        }

        private static List<D2Partner> GetD2PartnerList(PartnerRestrictions partnerRestrictions)
        {
            List<D2Partner> d2PartnerList = new List<D2Partner>();

            foreach (Partner partner in partnerRestrictions.Partners)
            {
                if (!string.IsNullOrEmpty(partner.Name) && partner.SalesStartDate.HasValue)
                {
                    // Exclude partners with the default sales start date.
                    if (partnerRestrictions.ProductDefaultSalesStartDate.HasValue &&
                      partnerRestrictions.ProductDefaultSalesStartDate.Value == partner.SalesStartDate.Value)
                    {
                        // Exclude
                    }
                    else
                    {
                        // Include
                        D2Partner d2Partner = new D2Partner(partner.Name, partner.SalesStartDate);
                        d2PartnerList.Add(d2Partner);
                    }
                }
            }

            return d2PartnerList;
        }

        private static bool HasOverriddenExpirationDate(DateTime? defaultDt, DateTime? finalDt)
        {
            // Override of a null Product Default
            if (finalDt.HasValue && !defaultDt.HasValue)
            {
                return true;
            }
            // Override of a not null Product Default
            if (finalDt.HasValue && defaultDt.HasValue
              && finalDt.Value != defaultDt.Value)
            {
                return true;
            }

            return false;
        }

        private static bool HasExclusivityEndDates(PartnerRestrictions partnerRestrictions)
        {
            foreach (Partner partner in partnerRestrictions.Partners)
            {
                if (partner.ExclusivityEndDate.HasValue)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsValidObject(PartnerRestrictions partnerRestrictions)
        {
            // Make sure that for each partner having a "real" Exclusivity End Date there is a Sales Start Date.
            // A "real" exclusivity end date is one not used to signal "Exclusive in Perpetuity", 12/31/9999.
            foreach (Partner partner in partnerRestrictions.Partners)
            {
                if (partner.ExclusivityEndDate.HasValue
                  && partner.ExclusivityEndDate.Value != new DateTime(9999, 12, 31)
                  && !partner.SalesStartDate.HasValue)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        public static string CombineProductRestrictions(ProductRestrictionItems dd)
        {
            string productRestrictions = ""; // default

            List<string> list = new List<string>();

            if (dd.DistributionRestriction != "" && dd.DistributionRestriction != "None.")
                list.Add(dd.DistributionRestriction);

            if (dd.AlbumOnlyRestriction != "")
                list.Add(dd.AlbumOnlyRestriction);

            if (dd.PreOrderOnlyRestriction != "")
                list.Add(dd.PreOrderOnlyRestriction);

            if (list.Count > 0)
                productRestrictions = string.Join(" ", list.ToArray());

            return productRestrictions;
        }

        public static string CombinePartnerRestrictions(PartnerRestrictionItems dd)
        {
            var partnerRestrictions = ""; // default

            List<string> list = new List<string>();

            if (dd.ExclusivityRestriction != "")
                list.Add(dd.ExclusivityRestriction);

            if (dd.DoNotDeliverRestriction != "")
                list.Add(dd.DoNotDeliverRestriction);

            if (dd.PreOrderRestriction != "")
                list.Add(dd.PreOrderRestriction);

            if (dd.ExpirationRestriction != "")
                list.Add(dd.ExpirationRestriction);

            if (dd.PreviewRestriction != "")
                list.Add(dd.PreviewRestriction);

            foreach (var partner in dd.D2PartnerList)
            {
                list.Add("SSD " + partner.SalesStartDate.Value.ToString("M/d/yy") + " for: " + partner.Name + ".");
            }

            if (list.Count > 0)
                partnerRestrictions = string.Join(" ", list.ToArray());

            return partnerRestrictions;
        }

        public static string CombineDeliveryComments(List<string> list)
        {
            if (list.Count == 0)
            {
                return "";  // Default if no delivery comments.
            }
            else
            {
                //return string.Join("<br />", list.ToArray());
                // #26064 - show only the last note.
                return list[list.Count - 1];
            }
        }

    }
}
