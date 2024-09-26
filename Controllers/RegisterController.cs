using blogsite.Data;
using blogsite.Models.DTO.RequestDTO;
using blogsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blogsite.Controllers;

public class RegisterController(BlogService service, BlogContext context) : Controller
{

	private readonly BlogService _service = service;

	[HttpGet]
	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Register(UserRequestDTO user)
	{
		if (ModelState.IsValid)

		{
			var checkemail = await _service.EmailAlreadyExists(user.Email);
			var checkuser = await _service.UserAlreadyExists(user.UserName);

			if (checkemail)
			{
				ModelState.AddModelError("", "Email already used");
				return View();

			}
			if (checkuser)
			{
				ModelState.AddModelError("", "username already taken");
				return View();
			}

			try
			{
				await _service.CreateUserAsync(user.FirstName, user.LastName, user.UserName, user.Password, user.Email);
				ModelState.Clear();
				return RedirectToAction("VerifyAccount");
			}
			catch (DbUpdateException e)
			{
				ModelState.AddModelError("", e.ToString());
				return View(user);
			}
		}
		return View(user);
	}


	[HttpGet]
	public IActionResult VerifyAccount()
	{
		return View();
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> VerifyAccount(VerifyAccountRequestDTO request)

	{
		if (ModelState.IsValid)
		{
			var verified = await _service.VerifyOTPAsync(request.Code);
			if (!verified)
			{
				ModelState.AddModelError("", "Invalid code");
				return View();
			}

			return RedirectToAction("Login", "Login");
		}

		return View();
	}
}

