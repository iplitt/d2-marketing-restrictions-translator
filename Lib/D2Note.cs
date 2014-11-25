using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class D2Note : IComparable<D2Note>
    {
        #region Properties
        public string Value
        {
            get;
            private set;
        }
        public DateTime UpdateDate
        {
            get;
            private set;
        }
        #endregion

        public D2Note(string value, DateTime updateDate)
        {
            Value = value;
            UpdateDate = updateDate;
        }

        #region IComparable<D2Note> Members

        public int CompareTo(D2Note other)
        {
            return this.UpdateDate.CompareTo(other.UpdateDate);
        }

        #endregion
    }
}
