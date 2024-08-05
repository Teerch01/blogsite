using blogsite.Data;
using blogsite.Models;
using blogsite.Models.DTO.ResponseDTO;
using blogsite.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace blogsite.Services;

public class BlogService(BlogContext context) : IBlogService
{
    private readonly BlogContext _context = context;

    public async Task<User> AutenticateUserAsync(string usernameoremail, string password)
    {

        var user = await _context.Users.Where(u =>
            (u.Username == usernameoremail || u.Email == usernameoremail) && u.Password == password).FirstOrDefaultAsync();

        if (user == null)
        {
            return null;
        }

        return user;
    }

    public async Task CreatePostAsync(string title, string content, Guid userid, string username)
    {
        Posts post = new()
        {
            Title = title,
            Content = content,
            UserId = userid,
            Username = username
        };
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();


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

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task DeletePostAsync(int postid)
    {
        var post = await _context.Posts.FindAsync(postid);
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<Posts> EditPostAsync(int postid, string title, string content)
    {
        var updatedpost = await _context.Posts.FindAsync(postid);
        updatedpost.Title = title;
        updatedpost.Content = content;
        await _context.SaveChangesAsync();
        return updatedpost;
    }

    public async Task<bool> EmailAlreadyExists(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<Posts> GetPostByIdAsync(int postid)
    {
        return await _context.Posts.AsNoTracking().SingleAsync(u => u.Id == postid);
    }

    public async Task<IEnumerable<Posts>> GetPostsAsync()
    {
        return await _context.Posts.AsNoTracking().OrderBy(p => p.CreatedOn).ToListAsync();
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

    public async Task<User> GetUserByUserNameAsync(string username)
    {
        return await _context.Users.AsNoTracking().FirstAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context.Users.AsNoTracking().ToListAsync();
    }

    public async Task<bool> UserAlreadyExists(string userName)
    {
        return await _context.Users.AnyAsync(u => u.Username == userName);
    }

    public async Task LikePost(int id, Guid userid)
    {
        var post = await _context.Posts.FindAsync(id);
        var existingLike = await _context.Likes.FirstOrDefaultAsync(u => u.PostId == post.Id && u.UserId == userid);

        if (existingLike == null)
        {
            var like = new Likes
            {
                PostId = post.Id,
                UserId = userid
            };
            _context.Likes.Add(like);
            post.LikeCount++;
            post.LikedByCurrentUser = true;
        }

        else
        {
            // User has already liked the post, remove the like
            _context.Likes.Remove(existingLike);
            post.LikeCount--;
            post.LikedByCurrentUser = false;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasUserLikedPost(int postid, Guid userid)
    {
        var post = await _context.Posts.FindAsync(postid);
        var existingLike = await _context.Likes.FirstOrDefaultAsync(u => u.PostId == post.Id && u.UserId == userid);
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
}
