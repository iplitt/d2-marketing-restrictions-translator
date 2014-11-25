using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public struct PartnerRestrictionItems
    {
        public string ExclusivityRestriction;
        public string DoNotDeliverRestriction;
        public string PreOrderRestriction;
        public string ExpirationRestriction;
        public string PreviewRestriction;
        public List<D2Partner> D2PartnerList;
    }
}
