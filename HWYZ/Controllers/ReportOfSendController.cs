using HWYZ.Context;
using HWYZ.Filters;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HWYZ.Controllers
{
    public class ReportOfSendController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult ListOfDetail(int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                ViewBag.list = (from q in db.OrderItem
                                join o in db.Order on q.OrderId equals o.ID
                                group q by new { o.StoreId, o.StoreName, q.ProductName, q.ProductCode } into s
                                orderby new { s.Key.StoreName, s.Key.ProductName }
                                select new PSMX() { StoreName = s.Key.StoreName, ProductName = s.Key.ProductName, ProductCode = s.Key.ProductCode, ProductNumber = s.Sum(p => p.RealNumber) }).Skip((pi - 1) * 2).Take(2).ToList();

                return PartialView();
            }
        }
    }
}
