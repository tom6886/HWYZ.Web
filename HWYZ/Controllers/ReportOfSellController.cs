using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HWYZ.Controllers
{
    public class ReportOfSellController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        private IQueryable<Order> GetOrderQuery(IQueryable<Order> orderQuery, string StartDate, string EndDate)
        {
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

            orderQuery = orderQuery.Where(q => q.Status == OrderStatus.Sended && q.ModifyTime.CompareTo(start) > 0 && q.ModifyTime.CompareTo(end) < 0);

            return orderQuery;
        }

        public PartialViewResult ListOfNumber(string ProductName, string StartDate, string EndDate, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var itemQuery = db.OrderItem.AsQueryable();

                var orderQuery = db.Order.AsQueryable();

                orderQuery = GetOrderQuery(orderQuery, StartDate, EndDate);

                if (!string.IsNullOrEmpty(ProductName)) { itemQuery = itemQuery.Where(q => q.ProductName.Equals(ProductName)); }

                ViewBag.list = (from q in itemQuery
                                join o in orderQuery on q.OrderId equals o.ID
                                group q by new { q.ProductName, q.ProductCode } into s
                                select new CXSL() { ProductName = s.Key.ProductName, ProductCode = s.Key.ProductCode, ProductNumber = s.Sum(p => p.RealNumber) }).OrderByDescending(q => q.ProductNumber).Skip((pi - 1) * 10).Take(10).ToList();

                double totalCount = (from q in itemQuery
                                     join o in orderQuery on q.OrderId equals o.ID
                                     group q by new { q.ProductName } into s
                                     select new { s.Key.ProductName }).Count();

                ViewBag.pager = new Pager()
                {
                    _TotalCount = totalCount,
                    _PageIndex = pi
                };

                return PartialView();
            }
        }

        public FileResult ExportNumber(string ProductName, string StartDate, string EndDate)
        {
            using (DBContext db = new DBContext())
            {
                var itemQuery = db.OrderItem.AsQueryable();

                var orderQuery = db.Order.AsQueryable();

                orderQuery = GetOrderQuery(orderQuery, StartDate, EndDate);

                if (!string.IsNullOrEmpty(ProductName)) { itemQuery = itemQuery.Where(q => q.ProductName.Equals(ProductName)); }

                var list = (from q in itemQuery
                            join o in orderQuery on q.OrderId equals o.ID
                            group q by new { q.ProductName, q.ProductCode } into s
                            select new CXSL() { ProductName = s.Key.ProductName, ProductCode = s.Key.ProductCode, ProductNumber = s.Sum(p => p.RealNumber) }).OrderByDescending(q => q.ProductNumber).ToList();

                //创建Excel文件的对象
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

                //给sheet1添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("商品名称");
                row1.CreateCell(1).SetCellValue("商品编号");
                row1.CreateCell(2).SetCellValue("数量");

                string status = string.Empty;
                //将数据逐步写入sheet1各个行
                for (int i = 0; i < list.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].ProductName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].ProductCode);
                    rowtemp.CreateCell(2).SetCellValue(list[i].ProductNumber);
                }

                DateTime now = DateTime.Now;
                // 写入到客户端 
                return ExportExcel(book, now.ToString("yyMMddHHmmssfff"));
            }
        }

        public PartialViewResult ListOfPay(string ProductName, string StartDate, string EndDate, int pi = 1)
        {
            using (DBContext db = new DBContext())
            {
                var itemQuery = db.OrderItem.AsQueryable();

                var orderQuery = db.Order.AsQueryable();

                orderQuery = GetOrderQuery(orderQuery, StartDate, EndDate);

                if (!string.IsNullOrEmpty(ProductName)) { itemQuery = itemQuery.Where(q => q.ProductName.Equals(ProductName)); }

                ViewBag.list = (from q in itemQuery
                                join o in orderQuery on q.OrderId equals o.ID
                                group q by new { q.ProductName, q.ProductCode } into s
                                select new CXJE() { ProductName = s.Key.ProductName, ProductCode = s.Key.ProductCode, ProductPay = s.Sum(p => p.RealNumber * p.Price * p.Discount) }).OrderByDescending(q => q.ProductPay).Skip((pi - 1) * 10).Take(10).ToList();

                double totalCount = (from q in itemQuery
                                     join o in orderQuery on q.OrderId equals o.ID
                                     group q by new { q.ProductName } into s
                                     select new { s.Key.ProductName }).Count();

                ViewBag.pager = new Pager()
                {
                    _TotalCount = totalCount,
                    _PageIndex = pi
                };

                return PartialView();
            }
        }

        public FileResult ExportPay(string ProductName, string StartDate, string EndDate)
        {
            using (DBContext db = new DBContext())
            {
                var itemQuery = db.OrderItem.AsQueryable();

                var orderQuery = db.Order.AsQueryable();

                orderQuery = GetOrderQuery(orderQuery, StartDate, EndDate);

                if (!string.IsNullOrEmpty(ProductName)) { itemQuery = itemQuery.Where(q => q.ProductName.Equals(ProductName)); }

                var list = (from q in itemQuery
                            join o in orderQuery on q.OrderId equals o.ID
                            group q by new { q.ProductName, q.ProductCode } into s
                            select new CXJE() { ProductName = s.Key.ProductName, ProductCode = s.Key.ProductCode, ProductPay = s.Sum(p => p.RealNumber * p.Price * p.Discount) }).OrderByDescending(q => q.ProductPay).ToList();

                //创建Excel文件的对象
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //添加一个sheet
                NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

                //给sheet1添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
                row1.CreateCell(0).SetCellValue("商品名称");
                row1.CreateCell(1).SetCellValue("金额");

                string status = string.Empty;
                //将数据逐步写入sheet1各个行
                for (int i = 0; i < list.Count; i++)
                {
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                    rowtemp.CreateCell(0).SetCellValue(list[i].ProductName);
                    rowtemp.CreateCell(1).SetCellValue(list[i].ProductPay.ToString());
                }

                DateTime now = DateTime.Now;
                // 写入到客户端 
                return ExportExcel(book, now.ToString("yyMMddHHmmssfff"));
            }
        }
    }
}
