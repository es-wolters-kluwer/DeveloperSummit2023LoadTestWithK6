namespace WkeSampleLoadApp.a3innuva
{
    public class CompanyModel
    {
        public Guid Id { get; set; }
        public string VatName { get; set; }
        public string VatNumber { get; set; }
        public string State { get; set; }
        public int AccountLength { get; set; }
        public string CorrelationId { get; set; }
        public DateTime Version { get; set; } 
        public long SequenceVersion { get; set; }
    }
}
