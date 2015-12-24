using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utils;

namespace HWYZ.Models
{
    [Table("Product")]
    public class Product : BaseObj
    {
        [NotMapped, Display(Name = "商品名"), MaxLength(100)]
        public string Name
        {
            set
            {
                this.ProductName = value;
                this.PinYin = Pinyin.GetPinyin(value);
                this.PinYin1 = Pinyin.GetInitials(value);
            }
        }

        [Display(Name = "商品名"), MaxLength(0x40), Required]
        public string ProductName { get; set; }

        [Display(Name = "商品名全拼"), MaxLength(100)]
        public string PinYin { get; set; }

        [Display(Name = "商品名首字母"), MaxLength(100)]
        public string PinYin1 { get; set; }

        [Display(Name = "商品编码"), MaxLength(0x40), Required]
        public string ProductCode { get; set; }

        [Display(Name = "商品类别"), MaxLength(0x40)]
        public string ProductType { get; set; }

        [Display(Name = "条形码"), MaxLength(0x40)]
        public string BarCode { get; set; }

        [Display(Name = "价格")]
        public decimal Price { get; set; }

        [Display(Name = "允许退换货")]
        public bool AllowReturn { get; set; }

        [Display(Name = "备注"), MaxLength(200)]
        public string Remark { get; set; }

        [Display(Name = "图片id")]
        public string DocId { get; set; }

        [Display(Name = "商品所属门店ID")]
        public string StoreId { get; set; }

        public Status Status { get; set; }
    }
}
