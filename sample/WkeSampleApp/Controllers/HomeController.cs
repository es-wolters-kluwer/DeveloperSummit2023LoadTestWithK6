using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WkeSampleLoadApp.Models;
using WkeSampleLoadApp.Context;
using Microsoft.EntityFrameworkCore;

namespace WkeSampleLoadApp.Controllers
{

    //[Authorize]
    public class HomeController : BaseController
    {

        private readonly ILogger<HomeController> _logger;
        private readonly WkeSampleLoadAppContext wkeSampleLoadAppContext;

        public HomeController(
            IConfiguration configuration,
            ILogger<HomeController> logger,
            WkeSampleLoadAppContext wkeSampleLoadAppContext
            )
            : base(configuration)
        {
            _logger = logger;
            this.wkeSampleLoadAppContext = wkeSampleLoadAppContext ?? throw new ArgumentNullException(nameof(wkeSampleLoadAppContext));
        }

        public async Task<IActionResult> Index()
        {

            var companies = await this.wkeSampleLoadAppContext.Companies.ToListAsync();
            return View(companies);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}