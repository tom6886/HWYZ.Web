using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HWYZ.Models
{
    [Table("Area")]
    public class Area
    {
        [MaxLength(6), Required]
        public string ID { get; set; }

        [MaxLength(100), Required]
        public string Name { get; set; }

        [MaxLength(6), Required]
        public string ParentId { get; set; }

        [MaxLength(100), Required]
        public string ShortName { get; set; }

        [MaxLength(1), Required]
        public string LevelType { get; set; }

        [MaxLength(4), Required]
        public string CityCode { get; set; }

        [MaxLength(6), Required]
        public string ZipCode { get; set; }

        [MaxLength(200), Required]
        public string MergerName { get; set; }

        [MaxLength(15), Required]
        public string Lng { get; set; }

        [MaxLength(15), Required]
        public string Lat { get; set; }

        [MaxLength(30), Required]
        public string Pinyin { get; set; }
    }
}
