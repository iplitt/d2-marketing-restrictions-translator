using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client = Lib.D2Terms.D2TermsClient;

namespace Lib
{
    public class Helper
    {
        public static async Task<ProductRestrictions> GetProductRestrictionsAsync(string upc, string countryCode)
        {
            var client = new Client();
            var response = client.GetMustExistInDaveProductTermsAllByUpc(upc, countryCode, D2Terms.ConstantseTermType.Marketing);
            return CreateProductRestrictions(response);
        }

        public static ProductRestrictions GetProductRestrictions(string upc, string countryCode)
        {
            var client = new Client();
            var response = client.GetMustExistInDaveProductTermsAllByUpc(upc, countryCode, D2Terms.ConstantseTermType.Marketing);
            return CreateProductRestrictions(response);
        }

        private static ProductRestrictions CreateProductRestrictions(D2Terms.Product product)
        {
            if (product == null)
                throw new Exception("Product does not exist.");

            ProductRestrictions productRestrictions = new ProductRestrictions();

            if (product.ProductLocalId != null)
            {
                productRestrictions.ProductExists = true;

                bool assetsExist = false;

                #region Assets
                if (product.Assets != null && product.Assets.Length > 0)
                {
                    foreach (var asset in product.Assets)
                    {
                        // Map all asset types. Bug # 22972
                        if (asset.Terms != null)
                        {
                            foreach (var term in asset.Terms)
                            {
                                // Use only the first term item in the Terms array.
                                foreach (var distributionChannel in term.DistributionChannels)
                                {
                                    // Exclude the distribution channel if its distribution group id is "1753".
                                    // Bug 21699 - must ignore distribution group "Other".
                                    if (distributionChannel.GroupId == "1753")
                                    {
                                        continue;
                                    }

                                    //foreach(TermsTermDistributionChannelsDistributionChannelStatus status in distributionChannel.Status)
                                    foreach (var status in distributionChannel.Status)
                                    {
                                        string groupName = distributionChannel.GroupName;
                                        DistributionGroup dg = productRestrictions.GetDistributionGroupByName(groupName);
                                        if (dg == null)
                                        {
                                            dg = new DistributionGroup();
                                            dg.Name = groupName;
                                            productRestrictions.DistributionGroups.Add(dg);
                                        }

                                        string channelName = distributionChannel.Name;
                                        DistributionChannel dc = dg.GetDistributionChannelByName(channelName);
                                        if (dc == null)
                                        {
                                            dc = new DistributionChannel();
                                            dc.Name = channelName;
                                            dg.DistributionChannels.Add(dc);
                                        }

                                        Asset thisAsset = dc.GetAssetByTrackTitleVolumeAndTrackNumber(asset.Title, asset.Volume, asset.TrackNumber);
                                        if (thisAsset == null)
                                        {
                                            thisAsset = new Asset();
                                            thisAsset.TrackTitle = asset.Title;
                                            thisAsset.TrackNumber = asset.TrackNumber;
                                            thisAsset.Volume = asset.Volume;
                                            dc.Assets.Add(thisAsset);
                                        }

                                        switch (status.Value)
                                        {
                                            case D2Terms.TermsOfUseRestrictionTypes.Unknown:
                                                thisAsset.Restriction = "Unknown";
                                                break;
                                            case D2Terms.TermsOfUseRestrictionTypes.Restricted:
                                                thisAsset.Restriction = "Restricted";
                                                break;
                                            case D2Terms.TermsOfUseRestrictionTypes.NotRestricted:
                                                thisAsset.Restriction = "Not Restricted";
                                                break;
                                            default:
                                                thisAsset.Restriction = "Unknown";
                                                break;
                                        }

                                        thisAsset.AlbumOnly = "Unknown";  // default value.
                                        // Use the first AlbumOnly value.
                                        foreach (var albumOnly in term.AlbumOnly)
                                        {
                                            switch (albumOnly.Value)
                                            {
                                                case D2Terms.AlbumOnlyTypes.Unknown:
                                                    thisAsset.AlbumOnly = "Unknown";
                                                    break;
                                                case D2Terms.AlbumOnlyTypes.Yes:
                                                    thisAsset.AlbumOnly = "Restricted";
                                                    break;
                                                case D2Terms.AlbumOnlyTypes.No:
                                                    thisAsset.AlbumOnly = "Not Restricted";
                                                    break;
                                                default:
                                                    thisAsset.AlbumOnly = "Unknown";
                                                    break;
                                            }
                                            break; // use only the first one.
                                        }

                                        switch (term.PreOrderOnly)
                                        {
                                            case "1":
                                                thisAsset.PreOrderOnly = "PreOrder Only";
                                                break;
                                            case "2":
                                                thisAsset.PreOrderOnly = "Instant Gratification";
                                                break;
                                            case "0":
                                            default:
                                                thisAsset.PreOrderOnly = "";
                                                break;
                                        }

                                        // Track-level sales start date - used in conjunction with Instant Gratification.
                                        // i.e. when Instant Gratification is in effect for this track, purchaser can
                                        // download the track starting on this date.  It is typically between
                                        // the PreOrder date and the normal Sales Start Date.
                                        thisAsset.SalesStartDate = string.IsNullOrEmpty(term.SalesStartDate) ? "" : term.SalesStartDate;
                                        thisAsset.SalesStartDt = string.IsNullOrEmpty(term.SalesStartDate) ? new DateTime?() : DateTime.Parse(term.SalesStartDate);

                                    }
                                }
                                assetsExist = true;
                                break;  // Use only the first term item for each asset.
                            }
                        }
                    }
                }
                #endregion

                #region Product

                if (product.Terms != null && product.Terms.Length > 0)
                {
                    // Use only the first term item.  There should only be one anyway.
                    var term = product.Terms[0];

                    #region Notes
                    // Extract notes.
                    if (term.Notes != null)
                    {
                        // Order notes by UpdateDate.

                        // Put them in a D2Note list first, then order them, then add them to DeliveryCommentsList.
                        List<D2Note> d2NoteList = new List<D2Note>();
                        foreach (var note in term.Notes)
                        {
                            d2NoteList.Add(new D2Note(note.Value, note.UpdateDate));
                        }

                        d2NoteList.Sort(); // IComparable interface orders them by UpdateDate.

                        foreach (D2Note d2Note in d2NoteList)
                        {
                            productRestrictions.DeliveryCommentsList.Add(d2Note.Value);
                        }
                    }
                    #endregion

                    #region Workflow Status

                    // If Confirm-Complete is set, workflow status is "Complete".
                    if (term.ConfirmComplete == "1")
                    {
                        productRestrictions.WorkflowStatusList.Add("Complete");
                    }
                    else // Otherwise, use the reported workflow status.
                    {
                        if (term.WorkflowStatus != null)
                        {
                            foreach (var workflowStatus in term.WorkflowStatus)
                            {
                                switch (workflowStatus.Value)
                                {
                                    case D2Terms.WorkFlowStatusTypes.NotStarted:
                                        productRestrictions.WorkflowStatusList.Add("Not Started");
                                        break;
                                    case D2Terms.WorkFlowStatusTypes.InProgress:
                                        productRestrictions.WorkflowStatusList.Add("In Progress");
                                        break;
                                    case D2Terms.WorkFlowStatusTypes.Complete:
                                        productRestrictions.WorkflowStatusList.Add("Complete");
                                        break;
                                    case D2Terms.WorkFlowStatusTypes.NotApplicable:
                                        productRestrictions.WorkflowStatusList.Add("Not Applicable");
                                        break;
                                    default:
                                        productRestrictions.WorkflowStatusList.Add("");
                                        break;
                                }
                            }
                        }
                    }
                    #endregion

                    #region Timestamp (UpdateDate)
                    productRestrictions.Timestamp = term.UpdateDate;
                    #endregion

                    if (!assetsExist)
                    {
                        #region Product instead of Assets
                        foreach (var distributionChannel in term.DistributionChannels)
                        {
                            // Exclude the distribution channel if its distribution group id is "1753".
                            // Bug 21699 - must ignore distribution group "Other".
                            if (distributionChannel.GroupId == "1753")
                            {
                                continue;
                            }
                            string groupName = distributionChannel.GroupName;
                            DistributionGroup dg = productRestrictions.GetDistributionGroupByName(groupName);
                            if (dg == null)
                            {
                                dg = new DistributionGroup();
                                dg.Name = groupName;
                                productRestrictions.DistributionGroups.Add(dg);
                            }

                            string channelName = distributionChannel.Name;
                            DistributionChannel dc = dg.GetDistributionChannelByName(channelName);
                            if (dc == null)
                            {
                                dc = new DistributionChannel();
                                dc.Name = channelName;
                                dg.DistributionChannels.Add(dc);
                            }

                            if (dc.Product == null)
                            {
                                dc.Product = new Product();
                            }

                            dc.Product.AlbumOnly = "Unknown"; // default.
                            // Use only the first item
                            foreach (var albumOnly in term.AlbumOnly)
                            {
                                switch (albumOnly.Value)
                                {
                                    case D2Terms.AlbumOnlyTypes.Unknown:
                                        dc.Product.AlbumOnly = "Unknown";
                                        break;
                                    case D2Terms.AlbumOnlyTypes.Yes:
                                        dc.Product.AlbumOnly = "Restricted";
                                        break;
                                    case D2Terms.AlbumOnlyTypes.No:
                                        dc.Product.AlbumOnly = "Not Restricted";
                                        break;
                                    default:
                                        dc.Product.AlbumOnly = "Unknown";
                                        break;
                                }
                                break; // First item only.
                            }

                            switch (term.PreOrderOnly)
                            {
                                case "1":
                                    dc.Product.PreOrderOnly = "PreOrder Only";
                                    break;
                                case "2":
                                    dc.Product.PreOrderOnly = "Instant Gratification";
                                    break;
                                case "0":
                                default:
                                    dc.Product.PreOrderOnly = "";
                                    break;
                            }

                            // Track-level sales start date - used in conjunction with Instant Gratification.
                            // i.e. when Instant Gratification is in effect for this track, purchaser can
                            // download the track starting on this date.  It is typically between
                            // the PreOrder date and the normal Sales Start Date.
                            dc.Product.SalesStartDate = string.IsNullOrEmpty(term.SalesStartDate) ? "" : term.SalesStartDate;
                            dc.Product.SalesStartDt = string.IsNullOrEmpty(term.SalesStartDate) ? new DateTime?() : DateTime.Parse(term.SalesStartDate);

                            dc.Product.Restriction = "Unknown"; // default.
                            //foreach(TermsTermDistributionChannelsDistributionChannelStatus status in distributionChannel.Status)
                            foreach (var status in distributionChannel.Status)
                            {
                                // Use first value only.
                                switch (status.Value)
                                {
                                    case D2Terms.TermsOfUseRestrictionTypes.Unknown:
                                        dc.Product.Restriction = "Unknown";
                                        break;
                                    case D2Terms.TermsOfUseRestrictionTypes.Restricted:
                                        dc.Product.Restriction = "Restricted";
                                        break;
                                    case D2Terms.TermsOfUseRestrictionTypes.NotRestricted:
                                        dc.Product.Restriction = "Not Restricted";
                                        break;
                                    default:
                                        dc.Product.Restriction = "Unknown";
                                        break;
                                }
                                break; // first value only.
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    throw new Exception("No product level terms.");
                }
                #endregion

            }
            return productRestrictions;
        }

        public static async Task<PartnerRestrictions> GetPartnerRestrictionsAsync(string upc, string countryCode)
        {
            var client = new Client();
            var response = client.GetPartnerProductTermsByUpc(upc, countryCode, null, "", "", false, false);
            return CreatePartnerRestrictions(response);
        }
        
        public static PartnerRestrictions GetPartnerRestrictions(string upc, string countryCode)
        {
            var client = new Client();
            var response = client.GetPartnerProductTermsByUpc(upc, countryCode, null, "", "", false, false);
            return CreatePartnerRestrictions(response);
        }

        private static PartnerRestrictions CreatePartnerRestrictions(D2Terms.Product1 productPartners)
        {
            if (productPartners == null)
                throw new Exception("Product does not exist.");

            var partnerRestrictions = new PartnerRestrictions();

            if (productPartners.Terms != null && productPartners.Terms.Length > 0)
            {
                partnerRestrictions.PartnersExist = true;

                // Use only the first term (there should only be one anyway).
                var term = productPartners.Terms[0];

                partnerRestrictions.Timestamp = term.UpdateDate;

                // If Confirm-Complete is set, workflow status is "Complete"
                if (term.ConfirmCompleteSpecified && term.ConfirmComplete)
                {
                    partnerRestrictions.WorkflowStatus = "Complete";
                }
                else // Use the actual workflow status
                {
                    partnerRestrictions.WorkflowStatus = term.WorkflowStatus;
                }

                DateTime? defaultSalesStartDate = null;
                DateTime? defaultExpirationDate = null;
                bool defaultDoNotDeliver = false;

                // If there are local defaults, use them.
                if (term.LocalDefault != null)
                {
                    if (term.LocalDefault.Length > 0)
                    {
                        DateTime dt = DateTime.UtcNow;
                        if (DateTime.TryParse(term.LocalDefault[0].SalesStartDate, out dt))
                        {
                            defaultSalesStartDate = dt;
                        }
                        if (DateTime.TryParse(term.LocalDefault[0].ExpirationDate, out dt))
                        {
                            defaultExpirationDate = dt;
                        }

                        bool.TryParse(term.LocalDefault[0].DoNotDeliver, out defaultDoNotDeliver);
                    }
                }

                // Some dates have inconsistent time parts, which causes problems.
                // Extract just the date part.
                if (defaultExpirationDate.HasValue)
                {
                    defaultExpirationDate = defaultExpirationDate.Value.Date;
                }
                if (defaultSalesStartDate.HasValue)
                {
                    defaultSalesStartDate = defaultSalesStartDate.Value.Date;
                }

                // Set Product Default properties from local default values.
                partnerRestrictions.ProductDefaultDoNotDeliver = defaultDoNotDeliver;
                partnerRestrictions.ProductDefaultExpirationDate = defaultExpirationDate;
                partnerRestrictions.ProductDefaultSalesStartDate = defaultSalesStartDate;


                foreach (var partner in term.Partners)
                {
                    /*
                        Partner types (from Kaz Baygan)
                        type_id       type
                        1669          Internal
                        1670          Commercial
                        1680          Non-Commercial
                    */

                    // Bug 21701 - Include only Commercial and Non-Commercial partners.
                    // Bug 24081 - Include only Commercial partners.
                    string partnerTypeId = partner.PartnerType.Id;
                    if (partnerTypeId == "1670")
                    {
                        // Bug 21639 - Use StorefrontName instead of PartnerName field.
                        string partnerName = partner.StorefrontName;
                        DateTime? salesStartDate = defaultSalesStartDate;
                        DateTime dt = DateTime.Now;
                        if (DateTime.TryParse(partner.SalesStartDate, out dt))
                        {
                            salesStartDate = dt;
                        }
                        DateTime? preOrderDate = null;
                        if (DateTime.TryParse(partner.PreOrderDate, out dt))
                        {
                            preOrderDate = dt;
                        }
                        DateTime? expirationDate = defaultExpirationDate;
                        if (DateTime.TryParse(partner.ExpirationDate, out dt))
                        {
                            expirationDate = dt;
                        }
                        DateTime? exclusivityEndDate = null;
                        if (DateTime.TryParse(partner.ExclusivityEndDate, out dt))
                        {
                            exclusivityEndDate = dt;
                        }
                        bool doNotDeliver = defaultDoNotDeliver;
                        bool.TryParse(partner.DoNotDeliver, out doNotDeliver);

                        // Note: The field "AllowPreview" is misnamed.  It should have been named "NoAllowPreview",
                        // but to minimize impact on other systems it was kept this way.

                        bool previewRestriction = false; // default value.
                        string allowPreviewLowerCase = "";
                        if (partner.AllowPreview != null)
                        {
                            allowPreviewLowerCase = partner.AllowPreview.ToLower();
                        }
                        // AllowPreview default value is false, so look for indications that it is being set to true.
                        if (allowPreviewLowerCase == "1" || allowPreviewLowerCase == "true" || allowPreviewLowerCase == "yes")
                        {
                            previewRestriction = true;
                        }

                        Partner p = new Partner();
                        p.DoNotDeliver = doNotDeliver;
                        p.PreviewRestriction = previewRestriction;
                        p.ExclusivityEndDate = exclusivityEndDate;
                        p.ExpirationDate = expirationDate;
                        p.Name = partnerName;
                        p.PreOrderDate = preOrderDate;
                        p.SalesStartDate = salesStartDate;

                        // Bug 21700 - Some dates have inconsistent time parts, which causes problems.
                        // Extract just the date part.
                        if (p.ExclusivityEndDate.HasValue)
                        {
                            p.ExclusivityEndDate = p.ExclusivityEndDate.Value.Date;
                        }
                        if (p.ExpirationDate.HasValue)
                        {
                            p.ExpirationDate = p.ExpirationDate.Value.Date;
                        }
                        if (p.PreOrderDate.HasValue)
                        {
                            p.PreOrderDate = p.PreOrderDate.Value.Date;
                        }
                        if (p.SalesStartDate.HasValue)
                        {
                            p.SalesStartDate = p.SalesStartDate.Value.Date;
                        }

                        partnerRestrictions.Partners.Add(p);
                    }

                }
            }
            else
            {
                throw new Exception("No product partner terms.");
            }

            return partnerRestrictions;
        }



    }
}
