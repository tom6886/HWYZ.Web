using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HWYZ.Models
{
    [Table("Product")]
    public class Product : BaseObj
    {
        [Display(Name = "产品名"), MaxLength(0x40), Required]
        public string ProductName { get; set; }

        [Display(Name = "产品编码"), MaxLength(0x40), Required]
        public string ProductCode { get; set; }

        [Display(Name = "条形码"), MaxLength(0x40)]
        public string BarCode { get; set; }

        [Display(Name = "价格")]
        public decimal Price { get; set; }

        [Display(Name = "允许退换货")]
        public bool AllowReturn { get; set; }

        [Display(Name = "备注"), MaxLength(200)]
        public string Remark { get; set; }

        [Display(Name = "图片")]
        public string Image { get; set; }

        [Display(Name = "商品所属门店ID")]
        public string StoreId { get; set; }

        public Status Status { get; set; }
    }
}
