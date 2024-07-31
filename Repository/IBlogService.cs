using blogsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace blogsite.Repository;

    public interface IBlogService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserByEmailAsync(string email);
        Task<ActionResult<IEnumerable<Posts>>> GetPostsOfUserById(Guid id);
        Task<User> CreateUserAsync([FromBody] string firstname, string lastname, string username, string password, string email);
        Task<bool> UserAlreadyExists(string userName);
        Task<bool> EmailAlreadyExists(string email);
        Task<User> AutenticateUserAsync(string username, string password);
        Task DeleteUserAsync(Guid id);
        Task CreatePostAsync([FromBody] string title, string content, string username);
        Task<Posts> GetPostByIdAsync(Guid id);
        Task<Posts> EditPostAsync(Guid postId, string post);
        Task DeletePostAsync(Guid id);
    }
