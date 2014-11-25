using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class ProductRestrictions
    {
        #region Properties
        public bool ProductExists { get; set; }
        public List<DistributionGroup> DistributionGroups { get; set; }
        public List<string> DeliveryCommentsList { get; set; }
        public List<string> WorkflowStatusList { get; set; }
        public DateTime? Timestamp { get; set; }
        #endregion

        public ProductRestrictions()
        {
            ProductExists = false;
            DistributionGroups = new List<DistributionGroup>();
            DeliveryCommentsList = new List<string>();
            WorkflowStatusList = new List<string>();
            Timestamp = null;
        }

        public DistributionGroup GetDistributionGroupByName(string distGroupName)
        {
            DistributionGroup result = null;
            foreach (DistributionGroup distGroup in DistributionGroups)
            {
                if (distGroup.Name == distGroupName)
                {
                    result = distGroup;
                    break;
                }
            }
            return result;
        }

    }
}
