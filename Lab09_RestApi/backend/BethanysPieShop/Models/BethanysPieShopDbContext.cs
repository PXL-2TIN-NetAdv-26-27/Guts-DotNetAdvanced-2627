using Microsoft.EntityFrameworkCore;

namespace BethanysPieShop.Models;

public class BethanysPieShopDbContext: DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Pie> Pies { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    public DbSet<Order> Orders { get; set; }

    public BethanysPieShopDbContext(DbContextOptions<BethanysPieShopDbContext> options) : base(options)
    {

    }
}