using blogsite.Models;

namespace blogsite.Data;

public class BlogContext(DbContextOptions<BlogContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; }
	public DbSet<Posts> Posts { get; set; }
	public DbSet<Likes> Likes { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>().ToTable(nameof(User));
		modelBuilder.Entity<Posts>().ToTable(nameof(Posts));
		modelBuilder.Entity<Likes>().ToTable(nameof(Likes));


		modelBuilder.Entity<User>().HasKey(x => x.Id);
		modelBuilder.Entity<User>().Property(x => x.Id).ValueGeneratedOnAdd();
		modelBuilder.Entity<User>().Property(x => x.Email).IsRequired();
		modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
		modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
		modelBuilder.Entity<User>().Property(x => x.Password).IsRequired();
		modelBuilder.Entity<User>().Property(x => x.FirstName).IsRequired();
		modelBuilder.Entity<User>().Property(x => x.LastName).IsRequired();

		
		modelBuilder.Entity<Posts>().HasKey(x => x.Id);
		modelBuilder.Entity<Posts>().Property(x => x.Id).ValueGeneratedOnAdd();
		modelBuilder.Entity<Posts>().Property(x => x.Title).IsRequired();
		modelBuilder.Entity<Posts>().Property(x => x.Content).IsRequired();


		modelBuilder.Entity<Likes>().HasKey(x => x.Id);
		modelBuilder.Entity<Posts>().Property(x => x.Id).ValueGeneratedOnAdd();


		// RELATIONSHIP
		modelBuilder
			.Entity<User>()
			.HasMany(x => x.Posts)
			.WithOne(x => x.User)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Restrict);
	}
	
}
