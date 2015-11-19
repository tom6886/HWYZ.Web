using System.Web.Mvc;

namespace HWYZ.Controllers
{
    public class StoreInfoController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            return View();
        }
    }
}
