using HWYZ.Context;
using HWYZ.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class LocationsController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Location.Include("Store").AsQueryable();

                string storeId = UserContext.user.StoreId;

                if (!string.IsNullOrEmpty(storeId)) { query = query.Where(q => q.StoreId.Equals(storeId)); }

                if (!string.IsNullOrEmpty(key)) { query = query.Where(q => q.Customer.Contains(key) || q.CustomerTel.Contains(key)); }

                PagedList<Location> cards = query.OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Location>(new List<Location>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
            }
        }
    }
}
