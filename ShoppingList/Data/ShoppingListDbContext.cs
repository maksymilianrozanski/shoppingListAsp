using Microsoft.EntityFrameworkCore;
using ShoppingList.Entities;

namespace ShoppingList.Data
{
    public class ShoppingListDbContext : DbContext
    {
        public ShoppingListDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ItemDataEntity> ItemDataEntities { get; set; }
        public DbSet<ShoppingListEntity> ShoppingListEntities { get; set; }
    }
}