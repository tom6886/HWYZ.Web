using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HWYZ.Models
{
    [Table("Location")]
    public class Location : BaseObj
    {
        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; }

        [Display(Name = "分店ID"), MaxLength(0x40), Required]
        public string StoreId { get; set; }

        [Display(Name = "客户姓名"), MaxLength(100)]
        public string Customer { get; set; }

        [Display(Name = "客户电话"), MaxLength(100)]
        public string CustomerTel { get; set; }

        [Display(Name = "送餐地址"), MaxLength(100)]
        public string Address { get; set; }
    }
}
