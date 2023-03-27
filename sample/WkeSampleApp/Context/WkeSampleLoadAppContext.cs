using Microsoft.EntityFrameworkCore;
using WkeSampleLoadApp.a3innuva;

namespace WkeSampleLoadApp.Context
{
    public class WkeSampleLoadAppContext : DbContext
    {
        public WkeSampleLoadAppContext(DbContextOptions<WkeSampleLoadAppContext> options)
            : base(options)
        {
        }

        public DbSet<CompanyModel> Companies { get; set; }

        public DbSet<AccountModel> Accounts { get; set; }

        public DbSet<InvoiceModel> Invoices { get; set; }

    }
}
