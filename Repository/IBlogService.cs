using blogsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace blogsite.Repository;

    public interface IBlogService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<Posts>> GetPostsOfUserById(Guid id);
        Task<User> CreateUserAsync([FromBody] string firstname, string lastname, string username, string password, string email);
        Task<bool> UserAlreadyExists(string userName);
        Task<bool> EmailAlreadyExists(string email);
        Task<User> AutenticateUserAsync(string username, string password);
        Task DeleteUserAsync(Guid id);
        Task CreatePostAsync([FromBody] string title, string content, Guid userid);
        Task<Posts> GetPostByIdAsync(int id);
        Task<Posts> EditPostAsync(int postId, string post);
        Task DeletePostAsync(int id);
        Task<int> LikePost(int postid);
    }
