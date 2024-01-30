﻿using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Web.Services;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentity.Web.Areas.Admin.Controllers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;  // Kullanıcı cookie olusturması ile ilgili önemli işlemler (login-logout- facebook ile giriş gibi)
        private readonly IEmailService _emailService;
        AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, AppDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(SignInViewModel signInViewModel, string retunrurl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            retunrurl ??= Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(signInViewModel.Email);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya Şifre yanlış");
                return View();
            }


            var result = await _signInManager.PasswordSignInAsync(hasUser.UserName, signInViewModel.Password, signInViewModel.RememberMe, true);
            if (result.Succeeded)
            {
                return Redirect(retunrurl);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "10 dakika boyunca giriş yapamazsınız !" });
                return View();
            }

            ModelState.AddModelErrorList(new List<string>() { "Email veya şifre hatalı !",
            $"Başarısız giriş sayısı={await _userManager.GetAccessFailedCountAsync(hasUser)}"});
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpViewModel)
        {
            if (ModelState.IsValid)
            {

                var identityResult = await _userManager.CreateAsync(new() { UserName = signUpViewModel.UserName, PhoneNumber = signUpViewModel.Phone, Email = signUpViewModel.Mail }, signUpViewModel.Password);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());

            }
            return RedirectToAction("Index","Home");
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            var hasuser = await _userManager.FindByEmailAsync(model.Email);

            if (hasuser == null)
            {
                ModelState.AddModelError(String.Empty, "Böyle bir mail adresi bulunamadı !");
                return View();
            }

            string passwordToken = await _userManager.GeneratePasswordResetTokenAsync(hasuser);
            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasuser.Id, Token = passwordToken }, HttpContext.Request.Scheme);

            await _emailService.SendResetPasswordEmail(passwordResetLink, hasuser.Email);


            TempData["SuccessMessage"] = "Şifre yenileme linki e-posta adresinize gönderilmiştir.";

            return RedirectToAction(nameof(ForgetPassword));
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;



            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var userID = TempData["userId"];
            var token = TempData["token"];

            if (token == null || userID == null)
            {
                throw new Exception("Bir hata meydana geldi.");
            }

            var hasUser = await _userManager.FindByIdAsync(userID.ToString()!);
            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Kullanıcı bulunamamıştır.");
                return View();

            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, model.Password);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarı ile yenilenmiştir.";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(p => p.Description).ToList());
            }

            return View();

        }

    }
}