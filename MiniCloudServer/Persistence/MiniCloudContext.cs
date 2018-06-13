using Microsoft.EntityFrameworkCore;
using MiniCloudServer.Entities;

namespace MiniCloudServer.Persistence
{
    public class MiniCloudContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ResourceAccess> ResourceAccesses { get; set; }

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

            var resourceAccessEntity=builder.Entity<ResourceAccess>();
            resourceAccessEntity.HasKey(x=>x.Id);
            resourceAccessEntity.Property(x=>x.Path).IsRequired();
            resourceAccessEntity.HasOne(x=>x.DoneeUser).WithMany(x=>x.ResourceAccesses);
            resourceAccessEntity.HasOne(x=>x.OwnerUser).WithMany().OnDelete(DeleteBehavior.Restrict);
        }
    }
}
