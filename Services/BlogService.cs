using blogsite.Data;
using blogsite.Models;
using blogsite.Repository;
using Microsoft.AspNetCore.Mvc;

namespace blogsite.Services;

public class BlogService(BlogContext context) : IBlogService
{
    private readonly BlogContext _context = context;

    public async Task<User> AutenticateUserAsync(string usernameorpassword, string password)
    {

        var user = await _context.Users.Where(u =>
            (u.Username == usernameorpassword || u.Email == usernameorpassword) && u.Password == password).FirstOrDefaultAsync();

        if (user == null)
        {
            return null;
        }

        return user;
    }

    public async Task CreatePostAsync(string title, string content, Guid userid)
    {
        Posts post = new()
        {
            Title = title,
            Content = content,
            UserId = userid
        };
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();


    }

    public async Task<User> CreateUserAsync(
        [FromBody] string firstname,
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

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task DeletePostAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<Posts> EditPostAsync(int postId, string post)
    {
        var updatedpost = await _context.Posts.FindAsync(postId);
        updatedpost.Content = post;
        await _context.SaveChangesAsync();
        return updatedpost;
    }

    public async Task<bool> EmailAlreadyExists(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<Posts> GetPostByIdAsync(int id)
    {
        return await _context.Posts.AsNoTracking().SingleAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<Posts>> GetPostsOfUserById(Guid id)
    {
        var posts = await _context.Posts.AsNoTracking().Where(u => u.UserId == id).ToListAsync();
        if (posts.Count != 0)
        {
            return posts;
        }

        return null;
        
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users.AsNoTracking().FirstAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context.Users.AsNoTracking().ToListAsync();
    }

    public async Task<bool> UserAlreadyExists(string userName)
    {
        return await _context.Users.AnyAsync(u => u.Username == userName);
    }

    public async Task<int> LikePost(int postid)
    {
        var post = await _context.Posts.FindAsync(postid);
        var like = post.Likes;
        like = like += 1;

        post.Likes = like;
        await _context.SaveChangesAsync();
        return like;
    }
}
