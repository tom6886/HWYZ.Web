using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace HWYZ.Controllers
{
    public class OrderController : BaseController
    {
        public ActionResult Index(string OrderCode, string StoreId, string Tel, string StartDate, string EndDate, string Status, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Order.AsQueryable();

                Guser user = UserContext.user;

                StoreId = user.Store == null ? StoreId : user.StoreId;

                query = SetQuery(query, OrderCode, StoreId, Tel, StartDate, EndDate, Status);

                PagedList<Order> cards = query.OrderByDescending(q => q.SubmitTime).ToPagedList(pi, 10);

                if (null == cards)
                    cards = new PagedList<Order>(new List<Order>(), 10, 0);

                if (Request.IsAjaxRequest())
                    return PartialView("List", cards);

                return View(cards);
            }
        }

        public FileResult Export(string OrderCode, string StoreId, string Tel, string StartDate, string EndDate, string Status)
        {
            using (DBContext db = new DBContext())
            {
                var query = db.Order.AsQueryable();

                Guser user = UserContext.user;

                StoreId = user.Store == null ? StoreId : user.StoreId;

                query = SetQuery(query, OrderCode, StoreId, Tel, StartDate, EndDate, Status);

                //获取list数据
                var list = query.OrderByDescending(q => q.SubmitTime).ToList();

                //创建Excel文件的对象
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

                //给sheet1添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("订单号");
                row1.CreateCell(1).SetCellValue("采购单位");
                row1.CreateCell(2).SetCellValue("负责人");
                row1.CreateCell(3).SetCellValue("联系电话");
                row1.CreateCell(4).SetCellValue("下单时间");
                row1.CreateCell(5).SetCellValue("订单金额");
                row1.CreateCell(6).SetCellValue("订单状态");

                string status = string.Empty;
                //将数据逐步写入sheet1各个行
                for (int i = 0; i < list.Count; i++)
                {
                    switch (list[i].Status)
                    {
                        case OrderStatus.BeforeSend: status = "待发货"; break;
                        case OrderStatus.BeforeSubmit: status = "待提交"; break;
                        case OrderStatus.Reject: status = "驳回"; break;
                        case OrderStatus.Sended: status = "已发货"; break;
                    }

                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].OrderCode);
                    rowtemp.CreateCell(1).SetCellValue(list[i].StoreName);
                    rowtemp.CreateCell(2).SetCellValue(list[i].Creator);
                    rowtemp.CreateCell(3).SetCellValue(list[i].Tel);
                    rowtemp.CreateCell(4).SetCellValue(list[i].SubmitTime.ToString());
                    rowtemp.CreateCell(5).SetCellValue(list[i].Paid.ToString());
                    rowtemp.CreateCell(6).SetCellValue(status);
                }

                DateTime now = DateTime.Now;
                // 写入到客户端 
                return ExportExcel(book, now.ToString("yyMMddHHmmssfff"));
            }
        }

        private IQueryable<Order> SetQuery(IQueryable<Order> query, string OrderCode, string StoreId, string Tel, string StartDate, string EndDate, string Status)
        {
            if (!string.IsNullOrEmpty(OrderCode)) { query = query.Where(q => q.OrderCode.Contains(OrderCode)); }

            if (!string.IsNullOrEmpty(Tel)) { query = query.Where(q => q.Tel.Contains(Tel)); }

            if (!string.IsNullOrEmpty(Status)) { query = query.Where(q => q.Status.ToString().Equals(Status)); }

            DateTime now = DateTime.Now;
            //不选择开始日期默认为本月1号
            DateTime start = string.IsNullOrEmpty(StartDate) ? DateTime.Parse(string.Format("{0}/{1}/{2}", now.Year.ToString(), now.Month.ToString(), "01")) : DateTime.Parse(StartDate);
            DateTime end = string.IsNullOrEmpty(EndDate) ? now : DateTime.Parse(EndDate).AddDays(1);

            if (start > end)
            {
                DateTime temp = DateTime.MinValue;
                temp = end;
                end = start;
                start = temp;
            }

            query = query.Where(q => q.SubmitTime.CompareTo(start) > 0 && q.SubmitTime.CompareTo(end) < 0);

            if (!string.IsNullOrEmpty(StoreId)) { query = query.Where(q => q.StoreId.Equals(StoreId)); }

            return query;
        }

        public ActionResult addOrder()
        {
            Guser user = UserContext.user;

            if (user.Store == null) { return null; }

            using (DBContext db = new DBContext())
            {
                DateTime now = DateTime.Now;

                string code = db.Order.Where(q => q.StoreId.Equals(user.StoreId)).Max(q => q.OrderCode);

                string start = string.Format("{0}{1}", user.Store.StoreCode, now.ToString("yyMMdd"));

                if (code == null || !code.StartsWith(start))
                {
                    code = string.Format("{0}{1}00", user.Store.StoreCode, now.ToString("yyMMdd"));
                }
                else
                {
                    int num = Convert.ToInt16(code.Substring(start.Length)) + 1;

                    if (num > 99) { return null; }

                    string numStr = num > 9 ? num.ToString() : "0" + num;

                    code = start + numStr;
                }

                Order order = new Order()
                {
                    Creator = user.DisplayName,
                    CreatorID = user.ID,
                    OrderCode = code,
                    StoreId = user.StoreId,
                    StoreName = user.Store.StoreName,
                    Tel = user.Tel,
                    Status = OrderStatus.BeforeSubmit,
                    SubmitTime = now
                };

                db.Order.Add(order);
                db.SaveChanges();

                return RedirectToAction("Index", "OrderEdit", new { orderId = order.ID });
            }
        }

        [HttpPost]
        public JsonResult delOrder(string orderId)
        {
            using (DBContext db = new DBContext())
            {
                Order order = db.Order.Where(q => q.ID.Equals(orderId)).FirstOrDefault();

                if (order == null) { return Json(new { code = -1, msg = "找不到对应订单" }); }

                db.OrderItem.RemoveRange(db.OrderItem.Where(q => q.OrderId.Equals(orderId)));

                db.Order.Remove(order);

                db.SaveChanges();

                return Json(new { code = 1, msg = "删除成功" });
            }
        }
    }
}
