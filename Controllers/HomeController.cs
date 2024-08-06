using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using blogsite.Models;
using blogsite.Models.DTO.ResponseDTO;
using blogsite.Services;
using AutoMapper;

namespace blogsite.Controllers;

public class HomeController(ILogger<HomeController> logger, BlogService service, IMapper mapper) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly BlogService _service = service;

    public async Task<IActionResult> Index()
    {
        if (ModelState.IsValid)
        {
            try
            {
                var posts = await _service.GetPostsAsync();
                if (posts != null)
                {
                    return View(posts.Select(_mapper.Map<PostResponseDTO>));
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "unable to get posts");
                return View();
            }
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
