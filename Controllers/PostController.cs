using AutoMapper;
using blogsite.Models;
using blogsite.Models.DTO.RequestDTO;
using blogsite.Models.DTO.ResponseDTO;
using blogsite.Services;
using Microsoft.AspNetCore.Mvc;

namespace blogsite.Controllers
{
    public class PostController(BlogService service, IMapper mapper) : Controller
    {
        private readonly BlogService _service = service;
        private readonly IMapper _mapper = mapper;

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
                    var username = HttpContext.User.Identity.Name;
                    var user = await _service.GetUserByUserNameAsync(username);

                    await _service.CreatePostAsync(
                        newPost.Title,
                        newPost.Content,
                        user.Id,
                        user.Username
                    );
                    ModelState.Clear();
                    ViewBag.Message = "Post created successfully";
                }
                catch (DbUpdateException e)
                {
                    ModelState.AddModelError("", $"{e}error");
                }
                return View();
            }

            return View(newPost);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            if (ModelState.IsValid)
            {
                var post = await _service.GetPostByIdAsync(id);
                return View(_mapper.Map<PostResponseDTO>(post));
            }
            return View();
        }

        public async Task<IActionResult> EditPostConfirmed(PostRequestDTO post)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var initialpost = await _service.GetPostByIdAsync(post.Id);
                    var updatedpost = await _service.EditPostAsync(post.Id, post.Title, post.Content);
                    
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Edit error");
                    return RedirectToAction("UserAccount", "Login");
                }
            }

            return RedirectToAction("UserAccount", "Login");
        }

        public async Task<IActionResult> DeletePost(int id) 
        {
            if (ModelState.IsValid)
            {
                var post = await _service.GetPostByIdAsync(id);
                return View(_mapper.Map<PostResponseDTO>(post));
            }
            return View();
        }

        
        public async Task<IActionResult> DeletePostConfirmed(int id)
        {
            if (ModelState.IsValid)
            {
                await _service.DeletePostAsync(id);
            }
            return RedirectToAction("UserAccount", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> ViewPost(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var post = await _service.GetPostByIdAsync(id);
                    if (post != null)
                    {
                        return View(_mapper.Map<PostResponseDTO>(post));
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

        [HttpGet]
        public async Task<IActionResult> UserPosts()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = HttpContext.User.Identity.Name;
                    var user = await _service.GetUserByUserNameAsync(username);

                    var posts = await _service.GetPostsOfUserById(user.Id);
                    if (posts != null)
                    {
                        foreach (var post in posts)
                        {
                            post.LikedByCurrentUser = await _service.HasUserLikedPost(
                                post.Id,
                                user.Id
                            );
                        }
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

        [HttpPost]
        public async Task<IActionResult> LikePost(int id)
        {
            var username = HttpContext.User.Identity.Name;
            var user = await _service.GetUserByUserNameAsync(username);
            if (ModelState.IsValid)
            {
                await _service.LikePost(id, user.Id);
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
    }
}
