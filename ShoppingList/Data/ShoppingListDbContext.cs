using Microsoft.EntityFrameworkCore;
using SharedTypes.Entities;

namespace ShoppingList.Data
{
    public class ShoppingListDbContext : DbContext
    {
        public ShoppingListDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ItemDataEntity> ItemDataEntities { get; set; } = null!;
        public DbSet<ShoppingListEntity> ShoppingListEntities { get; set; } = null!;

        public DbSet<ShopWaypointsEntity> ShopWaypointsEntities { get; set; } = null!;
        public DbSet<UserEntity> UserEntities { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemDataEntity>()
                .HasOne(i => i.ShoppingListEntity)
                .WithMany(j => j.ItemDataEntities)
                .HasForeignKey(i => i.ShoppingListEntityRefId);

            modelBuilder.Entity<ShoppingListEntity>()
                .HasOne(i => i.ShopWaypointsEntity)
                .WithMany(i => i.ShoppingListEntities)
                .HasForeignKey(i => i.ShopWaypointsEntityId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            modelBuilder.Entity<ShoppingListEntity>()
                .HasOne(i => i.UserEntity)
                .WithMany(i => i.ShoppingListEntities)
                .HasForeignKey(i => i.UserEntityId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<UserEntity>()
                .HasIndex(entity => entity.Login).IsUnique();

            modelBuilder.Entity<UserEntity>()
                .HasMany(i => i.ShoppingListEntities);
        }
    }
}