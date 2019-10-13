using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using bookstore.Entities;
using bookstore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace bookstore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<AppUser> _userManage;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;
        public AccountController(ILogger<AccountController> logger, 
                                 UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 IConfiguration configuration)
        {
            _logger = logger;
            _userManage = userManager;
            _signInManager = signInManager;
            _config = configuration;
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
        
        [HttpPost("api/token")]
        public async  Task<IActionResult>  CreateToken([FromBody] LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManage.FindByNameAsync(model.UserName);
                if(user != null)
                {
                    var result = await _signInManager
                                       .CheckPasswordSignInAsync(user, model.Password, false);

                    if(result.Succeeded)
                    {
                        var claims = new []
                                    {
                                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                                        new Claim(JwtRegisteredClaimNames.Jti, new Guid().ToString())
                                    };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
                        var token = new JwtSecurityToken(
                             _config["Tokes:Issuer"],
                             _config["Tokens:Audience"],
                             claims,
                             expires: DateTime.UtcNow.AddMinutes(30),
                            signingCredentials: creds
                          );

                        var writeToken = new
                                        {
                                            token = new JwtSecurityTokenHandler().WriteToken(token),
                                            exparation = token.ValidTo
                                        };

                        return Created("",writeToken);
                    }
                }
            }
            return BadRequest("Fail to create token");
        }
    }
}