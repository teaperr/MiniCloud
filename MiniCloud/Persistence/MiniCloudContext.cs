using Microsoft.EntityFrameworkCore;
using MiniCloud.Entities;

namespace MiniCloudServer.Persistence
{
    public class MiniCloudContext: DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MiniCloudDb;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            var userEnity= builder.Entity<User>();
            userEnity.HasKey(x=>x.Id);
            userEnity.Property(x=>x.UserName).IsRequired();
            userEnity.Property(x=>x.HashedPassword).IsRequired();
            userEnity.Property(x=>x.Salt).IsRequired();
        }
    }
}
