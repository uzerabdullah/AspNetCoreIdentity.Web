using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.Models
{
    public class UserGroup
    {
        [Key]
        public int Id { get; set; }
        public string GroupName { get; set; }
    }
}
