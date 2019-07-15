using Microsoft.EntityFrameworkCore;
 
namespace TheWallCSharp.Models
{
    public class HomeContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public HomeContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users {get;set;}
        public DbSet<Message> Messages {get;set;}
        public DbSet<Comment> Comments {get;set;}
        public DbSet<Like> Likes {get;set;}
    }
}