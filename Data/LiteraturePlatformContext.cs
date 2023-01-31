using LiteraturePlatformWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LiteraturePlatformWebApi.Data
{
    public class LiteraturePlatformContext : DbContext
    {
        public LiteraturePlatformContext(DbContextOptions<LiteraturePlatformContext> options)
            :base(options)
        {
            
        }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Text> Texts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Composition> Composition { get; set; }
        public DbSet<Rating> Rating { get; set; }

    }
}
