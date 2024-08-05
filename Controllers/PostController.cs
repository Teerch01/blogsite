using AutoMapper;
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

        [HttpGet]
        public async Task<IActionResult> ViewPost()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = HttpContext.User.Identity.Name;
                    var user = await _service.GetUserByUserNameAsync(username);

                    var posts = await _service.GetPostsAsync();
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
            }

            return RedirectToAction("UserAccount", "Login");
        }
    }
}
