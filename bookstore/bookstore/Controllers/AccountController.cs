using System.Linq;
using System.Threading.Tasks;
using bookstore.Entities;
using bookstore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace bookstore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<AppUser> _userManage;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(ILogger<AccountController> logger, 
                                 UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManage = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Wrong login information");
                _logger.LogInformation("Wrong login information");
                return View();
            }
            var user =  await _userManage.FindByNameAsync(login.UserName);
            if (user == null) return RedirectToAction("Register", "Account");

            var returnUrl = Request.Query.Keys.Contains("ReturnUrl")? Request.Query.Keys.First() : string.Empty;

            if (this.User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction("Register", "Account");

                return Redirect(returnUrl);

            }
                var signIn = await _signInManager
                                    .PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, false);
                if(signIn.Succeeded)
                {
                    if(string.IsNullOrEmpty(returnUrl)) return RedirectToAction("Index", "Home");
      
                      return Redirect(Request.Query.Keys.First());  
                }   
            
            ModelState.AddModelError("", "Fail to login");
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async  Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View();

           var user = await _userManage.FindByNameAsync(model.Username);
            if (user != null) { ModelState.AddModelError("", "Username is taken"); return View(); }

            var result = await  _userManage.CreateAsync(new AppUser
                                            {
                                                UserName = model.Username,
                                                FirstName = model.FirstName,
                                                LastName = model.LastName,
                                                Email = model.Email,
                                                PhoneNumber = model.Phonenumber
                                            }, model.Password);

            if(result.Succeeded)
            {
                var returnUrl = Request.Query.Keys.Contains("ReturnUrl") ? Request.Query.Keys.First() : string.Empty;

                if(string.IsNullOrEmpty(returnUrl)) return RedirectToAction("Index", "Home");
                  return Redirect(Request.Query.Keys.First());
            }
            ModelState.AddModelError("", "Fail to Register");
            _logger.LogInformation("Fail to Register");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Login", "Account");
        }
    }
}