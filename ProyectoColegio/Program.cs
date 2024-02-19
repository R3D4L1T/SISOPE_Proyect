using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProyectoColegio.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. establecer conexion
builder.Services.AddDbContext<ColegioDBContext>(
                  options => options.UseSqlServer(
                      builder.Configuration.GetConnectionString("ColegioConnection")));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllersWithViews();

//Agregar autenticaci�n por Cookies-->permisos
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(option =>
	{
		option.LoginPath = "/Access/Login";
		//option.AccessDeniedPath = "/Home/Error";
	});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Access}/{action=Login}/{id?}");

app.Run();
