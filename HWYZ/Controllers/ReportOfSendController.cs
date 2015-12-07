using HWYZ.Context;
using HWYZ.Filters;
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
            using (DBContext db = new DBContext())
            {
                var query = db.Order.AsQueryable();

                

                return null;
            }
        }
    }
}
