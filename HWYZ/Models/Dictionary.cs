using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HWYZ.Models
{
    [Table("Dictionary")]
    public class Dictionary : BaseObj
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string SortOrder { get; set; }

        public string ParentCode { get; set; }
    }
}
