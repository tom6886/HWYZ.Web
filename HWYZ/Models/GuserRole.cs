using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HWYZ.Models
{
    [Table("GuserRole")]
    public class GuserRole : BaseObj
    {
        [Display(Name = "角色名"), MaxLength(0x40), Required]
        public string RoleName { get; set; }

        [Display(Name = "角色权限值"), MaxLength(0x40), Required]
        public string RoleVal { get; set; }

        public Status Status { get; set; }
    }
}
