using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WkeSampleLoadApp.a3innuva;

namespace WkeSampleLoadApp.Context
{
    public static class ContextInitialize
    {

        public static async Task InitializeContextAsync(this WebApplication app)
        {

            using (var scope = app.Services.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<WkeSampleLoadAppContext>())
                {
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

                    if (pendingMigrations.Any())
                    {
                        await context.Database.MigrateAsync();
                    }

                    IEnumerable<CompanyModel> companies = GetCompanies();

                    if (!context.Companies.Any())
                    {
                        await context.Companies.AddRangeAsync(companies);
                        await context.SaveChangesAsync();
                    }

                    if (!context.Accounts.Any())
                    {

                        using (var streamReader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleData", "accounts.json")))
                        {

                            var json = streamReader.ReadToEnd();
                            var sampleAccounts = JsonConvert.DeserializeObject<List<AccountSampleModel>>(json);
                            foreach (var company in companies)
                            {
                                var accounts = sampleAccounts
                                    .Select(sample => new AccountModel()
                                    {
                                        Id = Guid.NewGuid(),
                                        Code = sample.Code,
                                        CompanyId = new Guid(company.CorrelationId),
                                        CorrelationId = Guid.NewGuid().ToString(),
                                        Description = sample.Description,
                                        SequenceVersion = sample.SequenceVersion,
                                        State = sample.State,
                                        Version = sample.Version
                                    });

                                await context.Accounts.AddRangeAsync(accounts);
                                await context.SaveChangesAsync();
                            }

                        }

                    }
                    if (!context.Invoices.Any())
                    {

                        //using (var streamReader = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}\\SampleData\\accounts.json"))
                        using (var streamReader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleData", "invoices.json")))
                        {

                            var json = streamReader.ReadToEnd();
                            var sampleInvoices = JsonConvert.DeserializeObject<List<InvoiceSampleModel>>(json);
                            foreach (var company in companies)
                            {
                                var invoices = sampleInvoices
                                    .Select(sample => new InvoiceModel()
                                    {
                                        Id = Guid.NewGuid(),
                                        InvoiceDate = sample.InvoiceDate,
                                        AccountCode = sample.AccountCode,
                                        CompanyId = new Guid(company.CorrelationId),
                                        CorrelationId = Guid.NewGuid().ToString(),
                                        Concept = sample.Concept,
                                        Amount = sample.Amount,
                                        TaxAmount = sample.TaxAmount,
                                        SequenceVersion = sample.SequenceVersion,
                                        Version = sample.Version
                                    });

                                await context.Invoices.AddRangeAsync(invoices);
                                await context.SaveChangesAsync();
                            }

                        }

                    }

                }
            }
        }

        private static IEnumerable<CompanyModel> GetCompanies()
        {
            IEnumerable<CompanyModel> companies = new List<CompanyModel>();
            using (var streamReader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleData", "companies.json")))
            {
                var json = streamReader.ReadToEnd();
                companies = JsonConvert.DeserializeObject<List<CompanySampleModel>>(json)                      
                                  .Select(sample => new CompanyModel()
                                  {
                                      Id = Guid.NewGuid(),
                                      AccountLength = sample.AccountLength,
                                      CorrelationId = sample.CorrelationId,
                                      SequenceVersion = sample.SequenceVersion,
                                      State = sample.State,
                                      VatName = sample.VatName,
                                      VatNumber = sample.VatNumber,
                                      Version = sample.Version
                                  }); 
            }
            return companies;
        }
        
    }
}
