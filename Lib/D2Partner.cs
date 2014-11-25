using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class D2Partner
    {
        #region Properties
        public string Name
        {
            get;
            set;
        }
        public DateTime? SalesStartDate
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public D2Partner()
        {
            // For serialization
            Name = "";
            SalesStartDate = new DateTime?();//i.e. null
        }
        public D2Partner(string name, DateTime? salesStartDate)
        {
            Name = name;
            SalesStartDate = salesStartDate;
        }
        #endregion

    }
}
