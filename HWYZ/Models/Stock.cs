using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utils;

namespace HWYZ.Models
{
    [Table("Stock")]
    public class Stock
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "商品编码")]
        public string ProdCode { get; set; }

        [Display(Name = "商品名称")]
        public string NameCh { get; set; }

        [Display(Name = "入库数量")]
        public int In { get; set; }

        [Display(Name = "入库单价")]
        public decimal RKDJ { get; set; }

        [Display(Name = "总价")]
        public decimal ZJ { get; set; }

        [Display(Name = "分店ID")]
        public string StationID { get; set; }

        [Display(Name = "入库标志")]
        public Flag Flag { get; set; }

        [Display(Name = "入库时间")]
        public DateTime CreateDate { get; set; }
    }
}
