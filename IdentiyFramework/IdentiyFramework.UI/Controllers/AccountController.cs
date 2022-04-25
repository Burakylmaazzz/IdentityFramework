using IdentiyFramework.UI.Identity;
using IdentiyFramework.UI.Models.IdentityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentiyFramework.UI.Controllers
{
    public class AccountController : Controller
    {
   
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel userModel)
        {
            if (ModelState.IsValid)
            {
                User user = new User();
                user.UserName = userModel.UserName;
                user.FullName = userModel.FullName;
                user.Email = userModel.Mail;

                var result = await _userManager.CreateAsync(user, userModel.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("İndex", "Home");
                }
            }
            ModelState.AddModelError("", "Please check your values");
            return View(userModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModels loginModel, string Returnurl)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "your mail or password is wrong");
                return View(loginModel);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);
            if (result.Succeeded)
            {
                ViewBag.url = Returnurl;
                return Redirect(string.IsNullOrEmpty(Returnurl) ? "/" : Returnurl);
            }
            ModelState.AddModelError("", "Your username or password is incorrect");
            return View();
        }

        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            ViewBag.url = ReturnUrl;
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDeniedPath()
        {
            return View();
        }
    }
}
