using AutoMapper;
using blogsite.Models;
using blogsite.Models.DTO.RequestDTO;
using blogsite.Models.DTO.ResponseDTO;
using blogsite.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace blogsite.Controllers
{
    public class PostController(BlogService service, IMapper mapper) : Controller
    {
        private readonly BlogService _service= service;
        private readonly IMapper _mapper= mapper;
        public IActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostRequestDTO newPost)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    var email = HttpContext.User.Identity.Name;
                    var user = await _service.GetUserByEmailAsync(email);

                    await _service.CreatePostAsync(newPost.Title, newPost.Content, user.Id);
                    ModelState.Clear();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "error");
                }
                return View(); 
            }

            return View(newPost);
        }

        [HttpGet]
        public async Task<IActionResult> ViewPost()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = HttpContext.User.Identity.Name;
                    var user = await _service.GetUserByEmailAsync(email);

                    var posts = await _service.GetPostsOfUserById(user.Id);
                    if (posts != null)
                    {
                        return View(posts);
                    }

                    return View();
                }
                catch (DbException)
                {
                    ModelState.AddModelError("", "unable to get posts");
                }
            }
            
            return View();
        }

    }
}
