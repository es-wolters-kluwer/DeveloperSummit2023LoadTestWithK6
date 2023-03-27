namespace WkeSampleLoadApp.a3innuva
{
    public class AccountModel
    {
        public Guid Id { get; set; }
        public string CorrelationId { get; set; }
        public Guid CompanyId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public DateTime Version { get; set; }
        public long SequenceVersion { get; set; }
        public bool IsTemplate => this.State == "Template";
    }
}
