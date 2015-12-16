using HWYZ.Filters;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HWYZ.Controllers
{
    [UserAuthorize]
    public class BaseController : Controller
    {
        public FileResult ExportExcel(HSSFWorkbook book, string fileName)
        {
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", string.Format("{0}.xls", fileName));
        }
    }
}
