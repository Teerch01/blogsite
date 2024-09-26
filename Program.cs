global using Microsoft.EntityFrameworkCore;
global using System.Text;
global using Microsoft.IdentityModel.Tokens;
using blogsite.Data;
using blogsite.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Hangfire;
using Hangfire.MySql;
using System.Transactions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
	options =>
{
	options.Cookie.HttpOnly = true;
	options.Cookie.SameSite = SameSiteMode.Strict;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
	};
}
);

var conn = builder.Configuration.GetConnectionString("DefaultConnection");

var hangconn = builder.Configuration.GetConnectionString("HangfireString");

builder.Services.AddDbContext<BlogContext>(options => options.UseNpgsql(conn));

builder.Services.AddScoped<BackgroundJobClient>();

builder.Services.AddHangfire(
	config => config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
	.UseSimpleAssemblyNameTypeSerializer()
	.UseRecommendedSerializerSettings()
	.UseStorage(new MySqlStorage(hangconn,
	new MySqlStorageOptions
	{
		TransactionIsolationLevel = IsolationLevel.ReadCommitted,
		QueuePollInterval = TimeSpan.FromSeconds(15),
		JobExpirationCheckInterval = TimeSpan.FromHours(1),
		CountersAggregateInterval = TimeSpan.FromMinutes(5),
		PrepareSchemaIfNecessary = true,
		DashboardJobListLimit = 50000,
		TransactionTimeout = TimeSpan.FromMinutes(1),
		TablesPrefix = "Hangfire"
	})));

builder.Services.AddScoped<BlogService>();
builder.Services.AddHangfireServer();

builder.Services.AddAutoMapper(typeof(Program).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHangfireDashboard("/hangfire");
app.UseHttpsRedirection();

BackgroundJob.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHangfireDashboard("/hangfire");

app.Run();
