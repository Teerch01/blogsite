using AutoMapper;
using blogsite.Models;
using blogsite.Models.DTO.RequestDTO;
using blogsite.Models.DTO.ResponseDTO;
using blogsite.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace blogsite.Controllers
{
	public class LoginController(BlogService service, IMapper mapper, IConfiguration configuration) : Controller
	{
		private readonly IConfiguration _configuration = configuration;
		private readonly BlogService _service = service;
		private readonly IMapper _mapper = mapper;
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
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

				if (user.Verified)
				{
					// Success, create cookie
					var claims = new[]{
						new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
						new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
						new Claim(ClaimTypes.Name, user.Username),
						new Claim(ClaimTypes.Email, user.Email),
						new Claim(ClaimTypes.Role, "User")
					};

					var tokenValue = _service.GenerateJwtToken(claims, _configuration
					);
					var cookieOptions = new CookieOptions
					{
						HttpOnly = true,
						Secure = true,
						SameSite = SameSiteMode.Strict,
						Expires = DateTime.Now.AddMinutes(120),
					};
					Response.Cookies.Append("token", tokenValue, cookieOptions);

					var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

					return RedirectToAction("UserAccount");
				}
				else
				{
					ModelState.AddModelError("", "User not Verified. Please Verify Account");
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
		public async Task<IActionResult> UserAccount(string tag)
		{
			try
			{
				var username = HttpContext.User.Identity.Name;
				ViewBag.Name = username;
				var user = await _service.GetUserByUserNameAsync(username);

				var posts = string.IsNullOrEmpty(tag)
			? await service.GetPostsAsync()
			: await service.GetPostsbyTagAsync(tag);

				var tags = await service.GetAllTagsAsync();

				if (posts != null)
				{
					foreach (var post in posts)
					{
						post.LikedByCurrentUser = await _service.HasUserLikedPost(post.Id, user.Id);
					}
					var viewModel = new IndexViewModel

					{
						Posts = posts.Select(_mapper.Map<PostResponseDTO>),
						Tags = tags
					};
					return View(viewModel);
				}
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "unable to get posts");
				return View();
			}

			return View();
		}
	}
}
