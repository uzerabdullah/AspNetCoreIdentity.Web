using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.Models
{
    public class UserGroupMatch
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int UserGroupId { get; set; }
    }
}
