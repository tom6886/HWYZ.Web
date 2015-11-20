using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Utils;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class ProductSetController : BaseController
    {
        public ActionResult Index(string key, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                Expression<Func<Product, bool>> where = PredicateExtensions.True<Product>();

                if (!string.IsNullOrEmpty(key)) { where = where.And(q => q.ProductName.Contains(key) || q.ProductCode.Contains(key)); }

                PagedList<Product> cards = db.Product.Where(where.Compile()).OrderByDescending(q => q.ModifyTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Product>(new List<Product>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
            }
        }

        [HttpPost]
        public object queryDialog(string productId)
        {
            using (DBContext db = new DBContext())
            {
                if (!string.IsNullOrEmpty(productId))
                {
                    Product product = db.Product.Where(q => q.ID.Equals(productId)).FirstOrDefault();

                    if (product == null) { return Json(new { code = -1, msg = "找不到指定商品" }); }

                    ViewBag.product = product;

                    return PartialView("Edit");
                }

                return PartialView("Add");
            }
        }

        public JsonResult savePicture()
        {
            try
            {
                string storeId = UserContext.user.StoreId;

                string path = string.Format(@"{0}\Upload\{1}\", Server.MapPath("/"), storeId);

                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;

                var suffix = files[0].ContentType.Split('/');
                //获取文件格式
                var _suffix = suffix[1];

                //随机生成文件名
                string fileName = string.Format("{0}.{1}", StringUtil.UniqueID(), _suffix);

                files[0].SaveAs(string.Format("{0}{1}", path, fileName));

                return Json(new { code = 1, src = string.Format("/Upload{0}/{1}", string.IsNullOrEmpty(storeId) ? "" : "/" + storeId, fileName) });
            }
            catch (Exception e)
            {
                return Json(new { code = -1, msg = string.Format("文件上传失败，请联系管理员，失败原因：{0}", e.Message) });
            }
        }
    }
}
