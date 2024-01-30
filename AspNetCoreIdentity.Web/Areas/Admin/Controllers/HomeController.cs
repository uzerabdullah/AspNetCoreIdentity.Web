using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Areas.Admin.Controllers
{
   // [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        public HomeController(UserManager<AppUser> userManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _context = appDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var userList = await _userManager.Users.ToListAsync();
            var userViewModelList = userList.Select(p => new Models.UserViewModel()
            {
                Email = p.Email,
                Name = p.UserName,
                Id = p.Id
            }).ToList();
            return View(userViewModelList);
        }

        public async Task<IActionResult> UserGroupList()
        {
            var userList = await _userManager.Users.ToListAsync();
            var userViewModelList = userList.Select(p => new Models.UserViewModel()
            {
                Email = p.Email,
                Name = p.UserName,
                Id = p.Id
            }).ToList();
            return View(userViewModelList);
        }

    }
}
