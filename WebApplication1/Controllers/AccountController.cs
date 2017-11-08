using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.DataTransferObjects;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto user)
        {
            // wybieramy usera z bazy, jesli istnieje to cisniemy
            var userExists = true;

            if (userExists)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name,"username usera z bazy"),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.Role, "User")
                };

                var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "SuperSecureLogin"));
                await AuthenticationHttpContextExtensions.SignInAsync(HttpContext, 
                                                                    CookieAuthenticationDefaults.AuthenticationScheme, 
                                                                    userPrincipal, 
                                                                    new AuthenticationProperties
                                                                    {
                                                                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                                                                        IsPersistent = false,
                                                                        AllowRefresh = false
                                                                    });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrMsg = "UserName or Password is invalid";

                return View();
            }
        }

        [HttpPost]
        public IActionResult Register(RegisterUserDto user)
        {
            if (user.Password.Equals(user.PasswordConfirmation, StringComparison.Ordinal))
            {
                //dodajemy usera do bazy jesli takiego nie ma

                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.ErrMsg = "Passwords are different!";

                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
