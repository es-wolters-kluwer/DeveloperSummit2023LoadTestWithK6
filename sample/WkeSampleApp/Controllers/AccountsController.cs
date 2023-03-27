namespace WkeSampleLoadApp.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using WkeSampleLoadApp.a3innuva;
    using WkeSampleLoadApp.Context;

    //[Authorize]
    public class AccountsController : BaseController
    {

        private readonly WkeSampleLoadAppContext wkeSampleLoadAppContext;

        public AccountsController(
            IConfiguration configuration,
            WkeSampleLoadAppContext wkeSampleLoadAppContext) 
            : base(configuration)
        {
            this.wkeSampleLoadAppContext = wkeSampleLoadAppContext ?? throw new ArgumentNullException(nameof(wkeSampleLoadAppContext));
        }

        // GET: AccountsController
        public async Task<IActionResult> Index(string companyCorrelationId)
        {

            var accounts = await this.wkeSampleLoadAppContext.Accounts.Where(a => a.CompanyId == new Guid(companyCorrelationId)).OrderBy(a => a.Code).ToListAsync();
            ViewBag.CompanyCorrelationId = companyCorrelationId;

            return View(accounts);

        }

        // GET: AccountsController/Details/5
        public async Task<IActionResult> Details(string companyCorrelationId, string accountCorrelationId)
        {
            var account = await this.wkeSampleLoadAppContext.Accounts.FirstAsync(r => r.CorrelationId == accountCorrelationId);

            ViewBag.CompanyCorrelationId = companyCorrelationId;

            return View(account);
        }

        // GET: AccountsController/Edit/5
        public async Task<IActionResult> Edit(string companyCorrelationId, string accountCorrelationId)
        {

            var account = await this.wkeSampleLoadAppContext.Accounts.FirstAsync(r => r.CorrelationId == accountCorrelationId);

            ViewBag.CompanyCorrelationId = companyCorrelationId;

            return View(account);

        }

        // POST: AccountsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CorrelationId,Code,Description")] ModifyAccountCommand modifyAccountCommand, string companyCorrelationId)
        {


            var account = await this.wkeSampleLoadAppContext.Accounts.FirstAsync(r => r.CorrelationId == modifyAccountCommand.CorrelationId);

            account.Description = modifyAccountCommand.Description;

            await this.wkeSampleLoadAppContext.SaveChangesAsync();

            ViewBag.CompanyCorrelationId = companyCorrelationId;

            return RedirectToAction("Index", new { companyCorrelationId = companyCorrelationId });

        }

    }
}
