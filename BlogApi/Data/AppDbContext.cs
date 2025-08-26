using Microsoft.EntityFrameworkCore;
using BlogApi.Models;
namespace BlogApi.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
    }

}