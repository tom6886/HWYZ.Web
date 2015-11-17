using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HWYZ.Models
{
    [Table("Store")]
    public class Store : BaseObj
    {
        [Display(Name = "门店名"), MaxLength(0x40), Required]
        public string StoreName { get; set; }

        [Display(Name = "门店编号"), MaxLength(0x40), Required]
        public string StoreCode { get; set; }

        [Display(Name = "省"), MaxLength(0x40), Required]
        public string Province { get; set; }

        [Display(Name = "市"), MaxLength(0x40), Required]
        public string City { get; set; }

        [Display(Name = "县"), MaxLength(0x40), Required]
        public string County { get; set; }

        [Display(Name = "地址"), MaxLength(100), Required]
        public string Address { get; set; }

        [Display(Name = "经度"), Required]
        public decimal Lng { get; set; }

        [Display(Name = "纬度"), Required]
        public decimal Lat { get; set; }

        [Display(Name = "推荐人"), MaxLength(100)]
        public string Recommender { get; set; }

        [Display(Name = "负责人"), MaxLength(100)]
        public string Presider { get; set; }

        [Display(Name = "联系电话"), MaxLength(100)]
        public string Tel { get; set; }

        [Display(Name = "门店类型"), Required]
        public StoreType StoreType { get; set; }

        [Display(Name = "折扣系数"), Required]
        public int Discount { get; set; }

        [Display(Name = "支付宝账号"), MaxLength(100)]
        public string Alipay { get; set; }

        [Display(Name = "微信账号"), MaxLength(100)]
        public string WeiXin { get; set; }

        [Display(Name = "银行账号"), MaxLength(100)]
        public string Bank { get; set; }

        public Status Status { get; set; }
    }
}
