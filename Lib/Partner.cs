using System;

namespace Lib
{
	[Serializable]
	public class Partner
  {
    public string Name { get; set; }
    public bool DoNotDeliver { get; set; }
    public bool PreviewRestriction { get; set; }
    public DateTime? SalesStartDate { get; set; }
    public DateTime? ExclusivityEndDate { get; set; }
    public DateTime? PreOrderDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
  }
}
