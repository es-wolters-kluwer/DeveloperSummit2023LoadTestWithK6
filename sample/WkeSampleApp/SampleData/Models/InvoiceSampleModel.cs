namespace WkeSampleLoadApp.a3innuva
{
    public class InvoiceSampleModel
    {
        public string CorrelationId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string AccountCode { get; set; }
        public string Concept { get; set; }
        public Double Amount { get; set; }
        public Double TaxAmount {get; set;}
        public string State { get; set; }
        public DateTime Version { get; set; }
        public long SequenceVersion { get; set; }
    }
}
