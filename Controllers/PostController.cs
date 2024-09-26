using AutoMapper;
using blogsite.Models.DTO.RequestDTO;
using blogsite.Models.DTO.ResponseDTO;
using blogsite.Services;
using Microsoft.AspNetCore.Mvc;

namespace blogsite.Controllers
{
	public class PostController(BlogService service, IMapper mapper) : Controller
	{

		[HttpGet]
		public IActionResult CreatePost()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreatePost(PostRequestDTO newPost)
		{
			if (ModelState.IsValid)
			{
				try
				{
					string imageUrl = await service.SaveImageAsync(newPost.ImageFile);

					var username = HttpContext.User.Identity.Name;
					var user = await service.GetUserByUserNameAsync(username);

					var content = Request.Form["content"];

					// Sanitize the HTML content
					var sanitizedContent = service.SanitizeHtml(content);

					await service.CreatePostAsync(
						newPost.Title.ToUpper(),
						sanitizedContent, // Use the sanitized content
						user.Id,
						user.Username,
						imageUrl
					);
					ModelState.Clear();
					ViewBag.Message = "Post created successfully";
				}
				catch (DbUpdateException)
				{
					ModelState.AddModelError("", $"error");
				}
				return View();
			}

			return View(newPost);
		}
		
		[HttpPost]
		public async Task<IActionResult> EditPost(int id)
		{
			if (ModelState.IsValid)
			{
				var post = await service.GetPostByIdAsync(id);
				return View(mapper.Map<PostResponseDTO>(post));
			}
			return View();
		}
		
		[HttpPost]
		public async Task<IActionResult> EditPostConfirmed(PostRequestDTO post)
		{
			if (ModelState.IsValid)
			{
				try
				{
					string imageUrl = await service.SaveImageAsync(post.ImageFile);

					var initialpost = await service.GetPostByIdAsync(post.Id);
					var updatedpost = await service.EditPostAsync(post.Id, post.Title, post.Content, imageUrl);

				}
				catch (DbUpdateException)
				{
					ModelState.AddModelError("", "Edit error");
					return RedirectToAction("UserAccount", "Login");
				}
			}

			return RedirectToAction("UserAccount", "Login");
		}
		[HttpPost]
		public async Task<IActionResult> DeletePost(int id)
		{
			if (ModelState.IsValid)
			{
				var post = await service.GetPostByIdAsync(id);
				return View(mapper.Map<PostResponseDTO>(post));
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> DeletePostConfirmed(int id)
		{
			if (ModelState.IsValid)
			{
				await service.DeletePostAsync(id);
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
					var post = await service.GetPostByIdAsync(id);
					if (post != null)
					{
						return View(mapper.Map<PostResponseDTO>(post));
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
					var user = await service.GetUserByUserNameAsync(username);

					var posts = await service.GetPostsOfUserById(user.Id);
					if (posts != null)
					{
						foreach (var post in posts)
						{
							post.LikedByCurrentUser = await service.HasUserLikedPost(
								post.Id,
								user.Id
							);
						}
						return View(posts.Select(mapper.Map<PostResponseDTO>));
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
			var user = await service.GetUserByUserNameAsync(username);
			if (ModelState.IsValid)
			{
				await service.LikePost(id, user.Id);
				return Json(new { success = true });
			}

			return Json(new { success = false });
		}

		[HttpPost]
		public async Task<IActionResult> Search(string searchQuery)
		{
			try
			{
				var result = await service.Search(searchQuery.ToUpper());
				if (result != null)
				{
					return View(result.Select(mapper.Map<PostResponseDTO>));
				}
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "No results found");
				return View();
			}
			return View();
		}
	}
}
