namespace WkeSampleLoadApp.a3innuva
{
    public class InvoiceModel
    {
        public Guid Id { get; set; }
        public string CorrelationId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string AccountCode { get; set; }
        public string Concept { get; set; }
        public Double Amount { get; set; }
        public Double TaxAmount {get; set;}        
        public DateTime Version { get; set; }
        public long SequenceVersion { get; set; }
        public Double Total => Amount + TaxAmount;
    }
}
