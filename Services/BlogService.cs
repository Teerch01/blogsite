using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using blogsite.Data;
using blogsite.Models;
using blogsite.Repository;
using Ganss.Xss;
using Hangfire;

namespace blogsite.Services;

public class BlogService(BlogContext context, BackgroundJobClient backgroundJob, IWebHostEnvironment webHost) : IBlogService
{
	public async Task<User> AutenticateUserAsync(string usernameoremail, string password)
	{

		var user = await context.Users.Where(u =>
			(u.Username == usernameoremail || u.Email == usernameoremail) && u.Password == password).FirstOrDefaultAsync();

		if (user == null)
		{
			return null;
		}

		return user;
	}
	public async Task<User> CreateUserAsync(
		string firstname,
		string lastname,
		string username,
		string password,
		string email
	)
	{
		User user =
			new()
			{
				FirstName = firstname,
				LastName = lastname,
				Username = username,
				Password = password,
				Email = email
			};

		await context.Users.AddAsync(user);
		await context.SaveChangesAsync();

		await SendOtpToUserEmail(email);

		return user;
	}

	public async Task CreatePostAsync(string title, string content, Guid userid, string username, string imageurl)
	{
		Posts post = new()
		{
			Title = title,
			Content = content,
			UserId = userid,
			Username = username,
			ImageUrl = imageurl
		};
		await context.Posts.AddAsync(post);
		await context.SaveChangesAsync();


	}


	public async Task DeletePostAsync(int postid)
	{
		var post = await context.Posts.FindAsync(postid);
		context.Posts.Remove(post);
		await context.SaveChangesAsync();
	}

	public async Task DeleteUserAsync(Guid id)
	{
		var user = await context.Users.FindAsync(id);
		context.Users.Remove(user);
		await context.SaveChangesAsync();
	}

	public async Task<Posts> EditPostAsync(int postid, string title, string content, string imageurl)
	{
		var updatedpost = await context.Posts.FindAsync(postid);
		updatedpost.Title = title;
		updatedpost.Content = content;
		updatedpost.ImageUrl = imageurl;
		await context.SaveChangesAsync();
		return updatedpost;
	}

	public async Task<bool> EmailAlreadyExists(string email)
	{
		return await context.Users.AnyAsync(u => u.Email == email);
	}

	public async Task<IEnumerable<Posts>> GetPostsAsync()
	{
		return await context.Posts.AsNoTracking().OrderBy(p => p.CreatedOn).ToListAsync();
	}
	public async Task<IEnumerable<Posts>> GetPostsbyTagAsync(string tag)
	{
		var posts = await context.Posts.AsNoTracking().Where(x => x.Tag == tag).ToListAsync();
		if(posts == null)
		{
			return null;
		}
		
		return posts;
	}

	public async Task<Posts> GetPostByIdAsync(int postid)
	{
		return await context.Posts.AsNoTracking().SingleAsync(u => u.Id == postid);
	}

	public async Task<IEnumerable<Posts>> GetPostsOfUserById(Guid id)
	{
		var posts = await context.Posts.AsNoTracking().Where(u => u.UserId == id).ToListAsync();
		if (posts.Count == 0)
		{
			return null;
		}

		return posts;
	}

	public async Task<User> GetUserByUserNameAsync(string username)
	{
		return await context.Users.AsNoTracking().FirstAsync(u => u.Username == username);
	}
	public async Task<User> GetUserByEmailAsync(string email)
	{
		return await context.Users.AsNoTracking().FirstAsync(u => u.Email == email);
	}

	public async Task<IEnumerable<User>> GetUsersAsync()
	{
		return await context.Users.AsNoTracking().ToListAsync();
	}

	public async Task<bool> UserAlreadyExists(string userName)
	{
		return await context.Users.AnyAsync(u => u.Username == userName);
	}

	public async Task LikePost(int id, Guid userid)
	{
		var post = await context.Posts.FindAsync(id);
		var existingLike = await context.Likes.FirstOrDefaultAsync(u => u.PostId == post.Id && u.UserId == userid);

		if (existingLike == null)
		{
			var like = new Likes
			{
				PostId = post.Id,
				UserId = userid
			};
			context.Likes.Add(like);
			post.LikeCount++;
			post.LikedByCurrentUser = true;
		}

		else
		{
			// User has already liked the post, remove the like
			context.Likes.Remove(existingLike);
			post.LikeCount--;
			post.LikedByCurrentUser = false;
		}
		await context.SaveChangesAsync();
	}

	public async Task<bool> HasUserLikedPost(int postid, Guid userid)
	{
		var post = await context.Posts.FindAsync(postid);
		var existingLike = await context.Likes.FirstOrDefaultAsync(u => u.PostId == post.Id && u.UserId == userid);
		if (existingLike == null)
		{
			post.LikedByCurrentUser = false;
			return false;
		}
		else
		{
			post.LikedByCurrentUser = true;
			return true;
		}
	}

	public async Task<IEnumerable<Posts>> Search(string searchQuery)
	{
		var searchResult = await context.Posts
		.AsNoTracking()
		.Where(p => p.Title.Contains(searchQuery) || p.Content.Contains(searchQuery)).ToListAsync();

		return searchResult;
	}

	public string GenerateJwtToken(IEnumerable<Claim> claims, IConfiguration configuration)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:key"]));
		var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
		var token = new JwtSecurityToken(
			configuration["Jwt:Issuer"],
			configuration["Jwt:Audience"],
			claims,
			expires: DateTime.UtcNow.AddMinutes(120),
			signingCredentials: signIn);

		string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
		return tokenValue;
	}

	public string GenerateRandomOTP()
	{
		Random random = new();
		var number = random.Next(100000, 999999).ToString();
		return number;
	}

	public bool ElaspedOTP(DateTime dateTime)
	{
		TimeSpan elasped = DateTime.UtcNow - dateTime;

		if (elasped.TotalMinutes > 30)
		{
			return true;
		}

		return false;
	}

	public async Task DeleteOtp(string email)
	{
		var response = await context.OTPs.Where(x => x.Useremail == email).ToListAsync();
		if (response == null)
		{
			return;
		}
		response.ForEach(item => context.Remove(item));
		await context.SaveChangesAsync();
		return;

	}

	public async Task<string> CreateOTPAsync(string email)
	{
		try
		{
			var user = await context.Users.FirstAsync(x => x.Email == email);

			if (user == null)
			{
				return null;
			}

			var otp = GenerateRandomOTP();

			OTP NewOtp = new()
			{
				Code = otp,
				UserId = user.Id,
				Useremail = user.Email
			};

			await context.OTPs.AddAsync(NewOtp);
			await context.SaveChangesAsync();

			return otp;
		}
		catch (Exception)
		{
			return null;
		}

	}

	public async Task<string> SendOtpToUserEmail(string email)
	{
		try
		{
			var mail = "api";
			var emailPass = "3bcdde0a97e501165cca7a6f97f4d0cf";

			var otp = await CreateOTPAsync(email);

			if (otp == null)
			{
				return null;
			}

			var emailData = new EmailData
			{
				From = "hello@demomailtrap.com",
				To = email,
				Subject = "Your OTP Code",
				Body = $"Your otp code is: {otp}"
			};


			BackgroundJob.Schedule(() => SendEmail(mail, emailPass, emailData), TimeSpan.FromSeconds(1));

			await context.SaveChangesAsync();
			return otp;
		}
		catch (Exception)
		{
			return null;
		}

	}


	public async Task SendEmail(string mail, string emailPass, EmailData emailData)
	{
		var client = new SmtpClient("live.smtp.mailtrap.io", 587)
		{
			Credentials = new NetworkCredential(mail, emailPass),
			EnableSsl = true
		};

		try
		{

			using MailMessage mailMessage = new(emailData.From, emailData.To)
			{
				Subject = emailData.Subject,
				Body = emailData.Body
			};

			await client.SendMailAsync(mailMessage);

		}
		catch (Exception)
		{

		}
		client.Dispose();
	}

	public async Task<bool> VerifyOTPAsync(string otp)
	{
		try

		{
			var Otp = await context.OTPs.FirstAsync(u => u.Code == otp);

			var existingUser = await context.Users.FirstAsync(u => u.Email == Otp.Useremail);

			var elaspedOTPTime = ElaspedOTP(Otp.CreatedOn);

			if (elaspedOTPTime == true)
			{
				throw new Exception("otp has expired");
			}

			if (existingUser == null)
			{
				return false;
			}

			if (Otp.Code != otp)
			{
				return false;
			}
			backgroundJob.Schedule(() => DeleteOtp(Otp.Useremail), TimeSpan.FromMinutes(30));
			existingUser.Verified = true;
			await context.SaveChangesAsync();

			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public string SanitizeHtml(string htmlContent)
	{
		var sanitizer = new HtmlSanitizer();
		return sanitizer.Sanitize(htmlContent);
	}

	public async Task<string> SaveImageAsync(IFormFile imageFile)
	{
		if (imageFile == null)
		{
			return null;
		}
		string webRootPath = webHost.WebRootPath;
		string fileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
		string path = Path.Combine(webRootPath, "images", fileName);

		using (var fileStream = new FileStream(path, FileMode.Create))
		{
			await imageFile.CopyToAsync(fileStream);
		}

		return "/images/" + fileName;
	}
	
	public async Task<IEnumerable<string>> GetAllTagsAsync()
{
    return await context.Posts
        .Select(p => p.Tag)
        .Distinct()
        .ToListAsync();
}
}
