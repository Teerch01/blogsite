using blogsite.Models.DTO.RequestDTO;
using blogsite.Services;
using Microsoft.AspNetCore.Mvc;

namespace blogsite.Controllers
{
    public class PostController(BlogService service) : Controller
    {
        private readonly BlogService _service= service;
        public IActionResult CreatePost()
        {
            return View();
        }

        public async Task<IActionResult> CreatePost(PostRequestDTO newPost)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.CreatePostAsync(newPost.Title, newPost.Content, newPost.UserName);
                    ModelState.Clear();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "error");
                    return View(newPost);
                }
                return View(); 
            }

            return View(newPost);
        }
    }
}
