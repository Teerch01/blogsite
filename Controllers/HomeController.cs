using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blogsite.Models;
using blogsite.Models.DTO.ResponseDTO;
using blogsite.Services;
using AutoMapper;

namespace blogsite.Controllers;

public class HomeController(BlogService service, IMapper mapper) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index(string tag)
	{
		try
		{
			var posts = string.IsNullOrEmpty(tag)
			? await service.GetPostsAsync()
			: await service.GetPostsbyTagAsync(tag);

			var tags = await service.GetAllTagsAsync();
			var viewModel = new IndexViewModel

			{
				Posts = posts.Select(mapper.Map<PostResponseDTO>),
				Tags = tags
			};

			if (viewModel != null)
			{

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

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
