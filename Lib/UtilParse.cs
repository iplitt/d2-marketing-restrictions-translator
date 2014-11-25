using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    class UtilParse
    {
        /// <summary>
        /// NOTE: 2010-06-10: This is copied from the BaseComparer class in the UMG.GDRS.Layers.IL.Tier5.DataItemSupport project.  
        /// Plan is to create a bullet-proofed, fully tested mechanism later that could potendially 
        /// move all of those comparisons to a UtilComparison Class that is in "lock-down" after the validation testing.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int Compare(DateTime? x, DateTime? y)
        {
            int ret;

            if (!x.HasValue)
            {
                if (!y.HasValue)
                {
                    // If x is null and y is null, they're equal. 
                    ret = 0;
                }
                else
                {
                    // If x is null and y is not null, y is greater. 
                    ret = -1;
                }
            }
            else
            {
                if (!y.HasValue)
                {
                    // If x is not null and y is null, x is greater.
                    ret = 1;
                }
                else
                {
                    // If x is not null and y is not null, compare the values.
                    ret = x.Value.CompareTo(y.Value);
                }
            }

            return ret;
        }

    }
}
