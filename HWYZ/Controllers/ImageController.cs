using HWYZ.Context;
using HWYZ.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utils;

namespace HWYZ.Controllers
{
    public class ImageController : BaseController
    {
        public FileResult getInitial(string docId)
        {
            return getImage(docId, false);
        }

        public FileResult getThumb(string docId)
        {
            return getImage(docId, true);
        }

        private FileResult getImage(string docId, bool isThumb)
        {
            DBContext db = new DBContext();

            Doc doc = db.Doc.Where(q => q.ID.Equals(docId)).FirstOrDefault();

            if (doc == null) { return null; }

            string _fileName = isThumb ? string.Format("{0}.thumb", doc.Name) : doc.Name;

            string _path = string.Format(@"{0}Upload\{1}{2}.{3}", Server.MapPath("/"), doc.DirPath, _fileName, doc.DocType);

            if (!System.IO.File.Exists(_path)) { return null; }

            FileStream fs = new FileStream(_path, FileMode.Open);

            byte[] byData = new byte[fs.Length];

            fs.Read(byData, 0, byData.Length);

            fs.Close();

            return File(byData, "image/jpg");
        }


        #region 保存图片

        public JsonResult savePicture()
        {
            try
            {
                Store store = UserContext.store;

                //随机生成文件名
                string fileName = StringUtil.UniqueID();

                string basePath = string.Format(@"{0}Upload\", Server.MapPath("/"));

                string detailPath = string.Format(@"{0}\{1}\", store == null ? "base" : store.ID, fileName);

                string path = string.Format("{0}{1}", basePath, detailPath);

                //string path = string.Format(@"{0}\Upload\{1}\{2}\", Server.MapPath("/"), string.IsNullOrEmpty(storeId) ? "base" : storeId, fileName);

                if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;

                var suffix = files[0].ContentType.Split('/');
                //获取文件格式
                var _suffix = suffix[1];

                string fullPath = string.Format("{0}{1}.{2}", path, fileName, _suffix);

                files[0].SaveAs(fullPath);

                GetPicThumbnail(fullPath, string.Format("{0}{1}.thumb.jpeg", path, fileName), 960, 480, 50);

                Doc doc = Doc.save(fileName, _suffix, store == null ? null : store.ID, detailPath);

                return Json(new { code = 1, docId = doc.ID });
            }
            catch (Exception e)
            {
                return Json(new { code = -1, msg = string.Format("文件上传失败，请联系管理员，失败原因：{0}", e.Message) });
            }
        }

        #endregion

        #region 压缩图片

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="dHeight">高度</param>
        /// <param name="dWidth"></param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <returns></returns>
        public bool GetPicThumbnail(string sFile, string dFile, int dHeight, int dWidth, int flag)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);

            ImageFormat tFormat = iSource.RawFormat;

            int sW = 0, sH = 0;

            //按比例缩放
            Size tem_size = new Size(iSource.Width, iSource.Height);

            if (tem_size.Width > dHeight || tem_size.Width > dWidth) //将**改成c#中的或者操作符号
            {
                if ((tem_size.Width * dHeight) > (tem_size.Height * dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }

            Bitmap ob = new Bitmap(sW, sH);
            //Bitmap ob = new Bitmap(dWidth, dHeight);

            Graphics g = Graphics.FromImage(ob);

            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
            g.DrawImage(iSource, new Rectangle(0, 0, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
            g.Dispose();

            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();

            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;

            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;

                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }

                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }

        #endregion
    }
}
