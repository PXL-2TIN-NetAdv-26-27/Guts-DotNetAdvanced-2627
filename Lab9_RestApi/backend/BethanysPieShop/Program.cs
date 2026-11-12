using BethanysPieShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

//builder.Services.AddScoped<ICategoryRepository, MockCategoryRepository>();
//builder.Services.AddScoped<IPieRepository, MockPieRepository>();

builder.Services.AddScoped<ICategoryRepository, CategoryDbRepository>();
builder.Services.AddScoped<IPieRepository, PieDbRepository>();
builder.Services.AddScoped<IOrderRepository, OrderDbRepository>();

builder.Services.AddScoped<IShoppingCart, ShoppingCart>(provider =>
{
    IHttpContextAccessor httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    ISession? session = httpContextAccessor.HttpContext?.Session;
    BethanysPieShopDbContext dbContext = provider.GetRequiredService<BethanysPieShopDbContext>();

    return ShoppingCart.GetCart(session, dbContext);
});
builder.Services.AddSession(); 
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<BethanysPieShopDbContext>(options => {
    options.UseSqlServer(
        builder.Configuration["ConnectionStrings:BethanysPieShopDbContextConnection"]);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseSession();

//app.MapDefaultControllerRoute();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

DbInitializer.Seed(app);
app.Run();

public partial class Program { }
