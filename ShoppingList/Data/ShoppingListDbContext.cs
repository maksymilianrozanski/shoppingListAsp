using Microsoft.EntityFrameworkCore;
using ShoppingList.Entities;

namespace ShoppingList.Data
{
    public class ShoppingListDbContext : DbContext
    {
        public ShoppingListDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ItemDataEntity> ItemDataEntities { get; set; } = null!;
        public DbSet<ShoppingListEntity> ShoppingListEntities { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<ItemDataEntity>()
                .HasOne(i => i.ShoppingListEntity)
                .WithMany(j => j.ItemDataEntities)
                .HasForeignKey(i => i.ShoppingListEntityRefId);
    }
}