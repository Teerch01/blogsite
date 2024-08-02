using blogsite.Models.DTO.RequestDTO;
using blogsite.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace blogsite.Controllers
{
    public class LoginController(BlogService service) : Controller
    {
        private readonly BlogService _service = service;
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO login)
        {
            if (ModelState.IsValid)
            {
                var user = await _service.AutenticateUserAsync(login.UsernameOrEmail, login.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Username/Email or password is incorrect");
                    return View();
                }
                else
                {
                    // Success, create cookie
                    var claims = new List<Claim> {
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Sid, user.Id.ToString()),
                    new(ClaimTypes.Role, "User")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    return RedirectToAction("UserAccount");
                }
                
                
            }
            return View();
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> UserAccount()
        {
            ViewBag.Name = HttpContext.User.Identity.Name;
            return View();
        }
    }
}
