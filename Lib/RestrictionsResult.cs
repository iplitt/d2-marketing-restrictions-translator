namespace Lib
{
    public class RestrictionsResult
    {
        public int Index { get; set; }
        public string Upc { get; set; }
        public ProductRestrictionItems ProductItems { get; set; }
        public PartnerRestrictionItems PartnerItems { get; set; }
        public bool Exists { get; set; }
        public string Restrictions { get; set; }
        public string Note { get; set; }
        public double TotalSeconds { get; set; }
    }
}
