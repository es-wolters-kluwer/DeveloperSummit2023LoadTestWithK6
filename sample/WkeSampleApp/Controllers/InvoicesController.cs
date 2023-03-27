namespace WkeSampleLoadApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
    using Microsoft.EntityFrameworkCore;
    using WkeSampleLoadApp.a3innuva;
    using WkeSampleLoadApp.Context;

    public class InvoicesController : BaseController
    {
        private readonly WkeSampleLoadAppContext wkeSampleLoadAppContext;

        public InvoicesController(
            IConfiguration configuration,
            WkeSampleLoadAppContext wkeSampleLoadAppContext)
            : base(configuration)
        {
            this.wkeSampleLoadAppContext = wkeSampleLoadAppContext ?? throw new ArgumentNullException(nameof(wkeSampleLoadAppContext));
        }

        // GET: InvoicesController
        public async Task<IActionResult> Index(string companyCorrelationId)
        {

            var invoices = await this.wkeSampleLoadAppContext.Invoices.Where(a => a.CompanyId == new Guid(companyCorrelationId)).OrderBy(a => a.InvoiceDate).ToListAsync();
            ViewBag.CompanyCorrelationId = companyCorrelationId;

            return View(invoices);

        }

        // GET: InvoicesController/Details/5
        public async Task<IActionResult> Details(string companyCorrelationId, string invoiceCorrelationId)
        {
            var invoice = await this.wkeSampleLoadAppContext.Invoices.FirstAsync(r => r.CorrelationId == invoiceCorrelationId);

            ViewBag.CompanyCorrelationId = companyCorrelationId;

            return View(invoice);
        }

        // GET: InvoicesController/Edit/5
        public async Task<IActionResult> Edit(string companyCorrelationId, string invoiceCorrelationId)
        {

            var invoice = await this.wkeSampleLoadAppContext.Invoices.FirstAsync(r => r.CorrelationId == invoiceCorrelationId);

            ViewBag.CompanyCorrelationId = companyCorrelationId;

            return View(invoice);

        }

        // POST: InvoicesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CorrelationId, InvoiceDate, AccountCode,Concept, Amount, TaxAmounbt")] ModifyInvoiceCommand modifyInvoiceCommand, string companyCorrelationId)
        {


            var invoice = await this.wkeSampleLoadAppContext.Invoices.FirstAsync(r => r.CorrelationId == modifyInvoiceCommand.CorrelationId);

            invoice.Concept = modifyInvoiceCommand.Concept;
            invoice.AccountCode = modifyInvoiceCommand.AccountCode;
            invoice.Amount = modifyInvoiceCommand.Amount;
            invoice.TaxAmount = modifyInvoiceCommand.TaxAmount;

            await this.wkeSampleLoadAppContext.SaveChangesAsync();

            ViewBag.CompanyCorrelationId = companyCorrelationId;

            return RedirectToAction("Index", new { companyCorrelationId = companyCorrelationId });

        }
    }
}
