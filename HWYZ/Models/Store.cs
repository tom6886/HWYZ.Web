using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utils;

namespace HWYZ.Models
{
    [Table("Store")]
    public class Store : BaseObj
    {
        [NotMapped, Display(Name = "门店名"), MaxLength(100)]
        public string Name
        {
            set
            {
                this.StoreName = value;
                this.PinYin = Pinyin.GetPinyin(value);
                this.PinYin1 = Pinyin.GetInitials(value);
            }
        }

        [ForeignKey("UserId")]
        public virtual Guser User { get; set; }

        [MaxLength(200)]
        public string UserId { get; set; }

        [Display(Name = "门店名"), MaxLength(0x40), Required]
        public string StoreName { get; set; }

        [Display(Name = "门店名全拼"), MaxLength(100)]
        public string PinYin { get; set; }

        [Display(Name = "门店名首字母"), MaxLength(100)]
        public string PinYin1 { get; set; }

        [Display(Name = "门店编号"), MaxLength(5), Required]
        public string StoreCode { get; set; }

        [Display(Name = "省"), MaxLength(0x40), Required]
        public string Province { get; set; }

        [Display(Name = "市"), MaxLength(0x40), Required]
        public string City { get; set; }

        [Display(Name = "县"), MaxLength(0x40), Required]
        public string Country { get; set; }

        [Display(Name = "地区编号"), MaxLength(4), Required]
        public string CityCode { get; set; }

        [Display(Name = "地址"), MaxLength(100), Required]
        public string Address { get; set; }

        [Display(Name = "经度"), Required]
        public string Lng { get; set; }

        [Display(Name = "纬度"), Required]
        public string Lat { get; set; }

        [Display(Name = "推荐人"), MaxLength(100)]
        public string Recommender { get; set; }

        [Display(Name = "负责人"), MaxLength(100)]
        public string Presider { get; set; }

        [Display(Name = "联系电话"), MaxLength(100)]
        public string Tel { get; set; }

        [Display(Name = "门店类型"), Required]
        public StoreType StoreType { get; set; }

        [Display(Name = "折扣系数"), Required]
        public decimal Discount { get; set; }

        [Display(Name = "支付宝账号"), MaxLength(100)]
        public string Alipay { get; set; }

        [Display(Name = "微信账号"), MaxLength(100)]
        public string WeiXin { get; set; }

        [Display(Name = "银行账号"), MaxLength(100)]
        public string Bank { get; set; }

        [Display(Name = "开户行"), MaxLength(100)]
        public string BankName { get; set; }

        [Display(Name = "开户人"), MaxLength(100)]
        public string BankAccount { get; set; }

        public Status Status { get; set; }
    }
}
