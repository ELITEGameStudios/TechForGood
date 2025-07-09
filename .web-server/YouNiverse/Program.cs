using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using YouNiverse.Models;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
	builder.Services.AddDbContext<UserContext>(options =>
		options.UseSqlite(builder.Configuration.GetConnectionString("UserContext")));

	builder.Services.AddDbContext<TimesheetContext>(options =>
		options.UseSqlite(builder.Configuration.GetConnectionString("TimesheetContext")));
}
else
{
	builder.Services.AddDbContext<UserContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("ProductionUserContext")));

	builder.Services.AddDbContext<TimesheetContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("ProductionTimesheetContext")));
}

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Signin";
	});
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Signin}/{action=Index}/{id?}");

app.Run();
