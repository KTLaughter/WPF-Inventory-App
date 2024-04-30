using InventoryAppV2;
using Microsoft.EntityFrameworkCore;

namespace InventoryAppV1
{
    public class InventoryDbContext : DbContext
    {
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Data Source=inventories.db"; //database file path
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}