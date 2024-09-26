using System.Security.Claims;
using blogsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace blogsite.Repository;

public interface IBlogService
{
	Task<IEnumerable<User>> GetUsersAsync();
	Task<User> GetUserByUserNameAsync(string username);
	Task<User> GetUserByEmailAsync(string email);
	Task<IEnumerable<Posts>> GetPostsOfUserById(Guid id);
	Task<User> CreateUserAsync([FromBody] string firstname, string lastname, string username, string password, string email);
	Task<bool> UserAlreadyExists(string username);
	Task<bool> EmailAlreadyExists(string email);
	Task<User> AutenticateUserAsync(string usernameoremail, string password);
	Task DeleteUserAsync(Guid id);
	Task CreatePostAsync([FromBody] string title, string content, Guid userid, string username, string imageurl);
	Task<Posts> GetPostByIdAsync(int postId);
	Task<IEnumerable<Posts>> GetPostsAsync();
	Task<IEnumerable<Posts>> Search(string query);
	Task<Posts> EditPostAsync(int postId, string title, string content, string imageurl);
	Task DeletePostAsync(int postid);
	Task LikePost(int postid, Guid userid);
	Task<bool> HasUserLikedPost(int postid, Guid userid);
	string GenerateJwtToken(IEnumerable<Claim> claims, IConfiguration configuration);
	string GenerateRandomOTP();
	bool ElaspedOTP(DateTime dateTime);
	Task DeleteOtp(string email);
	Task<string> CreateOTPAsync(string email);
	Task<string> SendOtpToUserEmail(string email);
	Task SendEmail(string mail, string emailPass, EmailData emailData);
	Task<bool> VerifyOTPAsync(string otp);
	string SanitizeHtml(string htmlContent);
	Task<string> SaveImageAsync(IFormFile imageFile);
}
