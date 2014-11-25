using System;
using System.Collections.Generic;

namespace Lib
{
	[Serializable]
  public class PartnerRestrictions
  {
    public bool PartnersExist { get; set; }
    public List<Partner> Partners { get; set; }
    public string WorkflowStatus { get; set; }
    public DateTime? Timestamp { get; set; }
    public bool ProductDefaultDoNotDeliver { get; set; }
    public DateTime? ProductDefaultExpirationDate { get; set; }
    public DateTime? ProductDefaultSalesStartDate { get; set; }
    
		public PartnerRestrictions()
    {
      PartnersExist = false;
      Partners = new List<Partner>();
      WorkflowStatus = "";
      Timestamp = null;
      ProductDefaultDoNotDeliver = false;
      ProductDefaultExpirationDate = null;
      ProductDefaultSalesStartDate = null;
    }
  }
}
