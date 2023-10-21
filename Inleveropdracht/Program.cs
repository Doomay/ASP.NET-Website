using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Inleveropdracht.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

// Configure your database connection and context.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "contact",
    pattern: "{controller=Contact}/{action=Contact}/{id?}");

app.MapControllerRoute(
    name: "placeOrder",
    pattern: "{controller=Orders}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "createProduct",
    pattern: "{controller=Orders}/{action=CreateProduct}/{id?}");

app.MapControllerRoute(
    name: "customer",
    pattern: "{controller=Customer}/{action=Customer}/{id?}");

app.MapControllerRoute(
    name: "createCustomer",
    pattern: "{controller=Customer}/{action=CreateCustomer}/{id?}");

app.MapControllerRoute(
    name: "placeOrderItem",
    pattern: "{controller=Orders}/{action=PlaceOrderItem}/{id?}");




app.Run();
