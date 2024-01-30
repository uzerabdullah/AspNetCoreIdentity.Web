using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AspNetCoreIdentity.Web.TagHelpers
{
    public class UserGroupNamesTagHelper: TagHelper
    {
        public string UserId { get; set; }
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;


        public UserGroupNamesTagHelper(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
           

            var user = await _userManager.FindByIdAsync(UserId);

            var userRoles = await _context.UserGroupMatch.Where(p => p.UserId == user.Id).ToListAsync();

            var stringBuilder = new StringBuilder();

            userRoles.ToList().ForEach(x =>
            {
                var groupToUpdate = _context.UserGroup.FirstOrDefault(p => p.Id == x.UserGroupId);
                stringBuilder.Append(@$"
                                       <span class='badge bg-secondary'>{groupToUpdate.GroupName.ToLower()} </span>");
            });

            output.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}
