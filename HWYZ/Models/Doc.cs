using HWYZ.Context;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace HWYZ.Models
{
    [Table("Doc")]
    public class Doc : BaseObj
    {
        [MaxLength(100), Required]
        public string Name { get; set; }

        [Required]
        public string DocType { get; set; }

        public string StoreId { get; set; }

        [Required]
        public string DirPath { get; set; }

        #region method

        public static Doc save(string name, string type, string storeId, string dir)
        {
            using (DBContext db = new DBContext())
            {
                Doc doc = new Doc()
                {
                    Creator = UserContext.user.DisplayName,
                    CreatorID = UserContext.user.ID,
                    Name = name,
                    DocType = type,
                    StoreId = storeId,
                    DirPath = dir
                };

                db.Doc.Add(doc);
                db.SaveChanges();

                return doc;
            }
        }

        public static void delete(string id)
        {
            using (DBContext db = new DBContext())
            {
                Doc doc = db.Doc.Where(q => q.ID.Equals(id)).FirstOrDefault();

                if (doc == null) { return; }

                DirectoryInfo dir = new DirectoryInfo(doc.DirPath);
                dir.Delete(true);

                db.Doc.Remove(doc);
                db.SaveChanges();
            }
        }

        #endregion
    }
}
