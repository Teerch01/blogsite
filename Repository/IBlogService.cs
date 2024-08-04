using blogsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace blogsite.Repository;

public interface IBlogService
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User> GetUserByUserNameAsync(string username);
    Task<IEnumerable<Posts>> GetPostsOfUserById(Guid id);
    Task<User> CreateUserAsync([FromBody] string firstname, string lastname, string username, string password, string email);
    Task<bool> UserAlreadyExists(string username);
    Task<bool> EmailAlreadyExists(string email);
    Task<User> AutenticateUserAsync(string usernameoremail, string password);
    Task DeleteUserAsync(Guid id);
    Task CreatePostAsync([FromBody] string title, string content, Guid userid, string username);
    Task<Posts> GetPostByIdAsync(int postId);
    Task<IEnumerable<Posts>> GetPostsAsync();
    Task<Posts> EditPostAsync(int postId, string title, string content);
    Task DeletePostAsync(int postid);
    Task LikePost(int postid, Guid userid);
    Task<bool> HasUserLikedPost(int postid, Guid userid);
}
