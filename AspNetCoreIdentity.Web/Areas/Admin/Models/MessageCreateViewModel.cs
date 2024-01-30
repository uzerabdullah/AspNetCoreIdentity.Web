using AspNetCoreIdentity.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.Areas.Admin.Models
{
    public class MessageCreateViewModel
    {
        [Display(Name = "Mesaj")]
        [Required]
        public string Message { get; set; }
        public DateTime SendDate { get; set; }
        [Display(Name = "Kullanıcı Seçiniz")]
        public string UserId { get; set; }

        [Display(Name = "Grup Seçiniz")]
        public int SiteUserGroupId { get; set; }
    }
}
