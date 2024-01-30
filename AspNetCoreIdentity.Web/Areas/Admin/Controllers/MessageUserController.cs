using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class MessageUserController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;


        public MessageUserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var groups = await _context.UserGroup.ToListAsync();

            return View(groups);
        }

        [HttpGet]
        public IActionResult GroupCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GroupCreate(UserGroup model)
        {
            var result = await _context.AddAsync(new UserGroup() { GroupName = model.GroupName });
            _context.SaveChangesAsync();
            return RedirectToAction(nameof(MessageUserController.Index));
        }


        public async Task<IActionResult> GroupUpdate(int id)
        {
            var groupToUpdate = await _context.UserGroup.FirstOrDefaultAsync(p => p.Id == id);
            if (groupToUpdate == null)
            {
                throw new Exception("Güncellenecek grup bulunamamıştır.");
            }

            return View(new UserGroup() { GroupName = groupToUpdate.GroupName });
        }

        [HttpPost]
        public async Task<IActionResult> GroupUpdate(UserGroup model)
        {
            var groupToUpdate = await _context.UserGroup.FirstOrDefaultAsync(p => p.Id == model.Id);
            if (groupToUpdate == null)
            {
                throw new Exception("Güncellenecek grup bulunamamıştır.");
            }
            groupToUpdate.GroupName = model.GroupName;
            _context.UserGroup.Update(groupToUpdate);
            _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Güncelleme işlemi başarılı.";
            return View();
        }

        public async Task<IActionResult> GroupDelete(int id)
        {
            var groupToUpdate = await _context.UserGroup.FirstOrDefaultAsync(p => p.Id == id);
            if (groupToUpdate == null)
            {
                throw new Exception("Güncellenecek üye grubu bulunamamıştır.");
            }
            _context.UserGroup.Remove(groupToUpdate);
            _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Üye Grubu silinmiştir.";
            return RedirectToAction(nameof(RolesController.Index));
        }


        public async Task<IActionResult> AssignGroupToUser(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            ViewBag.userId = id;

            var roles = await _context.UserGroup.ToListAsync();

            var roleViewModelList = new List<AssignGrupToUserViewModel>();

            var userRoles = await _context.UserGroupMatch.Where(p => p.UserId == currentUser.Id).ToListAsync();

            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignGrupToUserViewModel() { Id = role.Id, Name = role.GroupName };

                if (userRoles.Any(p => p.UserGroupId == role.Id))
                {
                    assignRoleToUserViewModel.Exist = true;
                }

                roleViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(roleViewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignGroupToUser(string userId, List<AssignGrupToUserViewModel> modelList)
        {
            var currentUser = await _userManager.FindByIdAsync(userId);

            foreach (var role in modelList)
            {
                var resultGroup = _context.UserGroup.FirstOrDefault(p => p.GroupName == role.Name);
                if (role.Exist)
                {
                    await _context.UserGroupMatch.AddAsync(new UserGroupMatch { UserGroupId = resultGroup.Id, UserId = currentUser.Id });
                }
                else
                {
                    var record = _context.UserGroupMatch.FirstOrDefault(p => p.UserGroupId == resultGroup.Id && p.UserId == currentUser.Id);
                    if (record != null)
                        _context.UserGroupMatch.Remove(record);

                }
            }
            _context.SaveChangesAsync();

            return RedirectToAction(nameof(HomeController.UserGroupList), "Home");
        }

        public async Task<IActionResult> MessageSend()
        {
            var userGroupList = await _context.UserGroup.ToListAsync();
            ViewBag.userGroupList = new SelectList(userGroupList, "Id", "GroupName");


            var userList = await _userManager.Users.ToListAsync();
            ViewBag.userList = new SelectList(userList, "Id", "UserName");

            MessageCreateViewModel messageCreateViewModel = new MessageCreateViewModel();
            return View(messageCreateViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> MessageSend(MessageCreateViewModel model)
        {
            if (model.UserId == "Seçiniz" && model.SiteUserGroupId == 0)
            {
                ModelState.AddModelError(String.Empty, "Kullanıcı veya grup seçiniz.");

                var userGroupList = await _context.UserGroup.ToListAsync();
                ViewBag.userGroupList = new SelectList(userGroupList, "Id", "GroupName");


                var userList = await _userManager.Users.ToListAsync();
                ViewBag.userList = new SelectList(userList, "Id", "UserName");

                MessageCreateViewModel messageCreateViewModel = new MessageCreateViewModel();
                return View();
            }

            if (model.UserId != "Seçiniz" && model.SiteUserGroupId != 0)
            {
                ModelState.AddModelError(String.Empty, "Kullanıcı ve grup aynı anda seçilemez.");

                var userGroupList = await _context.UserGroup.ToListAsync();
                ViewBag.userGroupList = new SelectList(userGroupList, "Id", "GroupName");


                var userList = await _userManager.Users.ToListAsync();
                ViewBag.userList = new SelectList(userList, "Id", "UserName");

                MessageCreateViewModel messageCreateViewModel = new MessageCreateViewModel();
                return View();
            }

            if (model.UserId != "Seçiniz")
            {
                Messages messages = new Messages();
                messages.CreateDate = DateTime.Now;
                messages.Message = model.Message;
                messages.UserId = model.UserId;
                messages.GroupMessage = "";
                _context.Messages.Add(messages);
                await _context.SaveChangesAsync();
            }

            if (model.SiteUserGroupId > 0)
            {
                var userGroupMatching = await _context.UserGroupMatch.Where(p => p.UserGroupId == model.SiteUserGroupId).ToListAsync();
                foreach (var item in userGroupMatching)
                {
                    Messages messages = new Messages();
                    messages.CreateDate = DateTime.Now;
                    messages.Message = model.Message;
                    messages.UserId = item.UserId;
                    messages.GroupMessage= "X";
                    _context.Messages.Add(messages);
                }
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "İşlemi başarıyla gerçekleştirildi.";
            return View();
        }
    }
}
