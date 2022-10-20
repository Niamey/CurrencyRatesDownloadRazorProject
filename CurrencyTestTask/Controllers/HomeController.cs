using Core;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CurrencyTestTask.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index(string id)
        {
            CoreDataProcessing coreData = new CoreDataProcessing();
            if (id== null)
            {
                return View();
            }
            if (id.Contains("GetCurrencyInformation"))
            {
              await coreData.DownloadCurrencyRates();
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}