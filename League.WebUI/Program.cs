using System.Globalization;
using League.BusinessLayer.Abstract;
using League.BusinessLayer.Concrete;
using League.DataAccessLayer.Abstract;
using League.DataAccessLayer.Context;
using League.DataAccessLayer.EntityFramework;
using League.DataAccessLayer.Seed;
using League.WebUI.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("tr-TR");
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("LeagueConnection") ?? throw new InvalidOperationException("ConnectionStrings:LeagueConnection yapılandırılmalıdır.");
builder.Services.AddDbContext<LeagueContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IMatchDal, EfMatchDal>();
builder.Services.AddScoped<ILeagueDal, EfLeagueDal>();
builder.Services.AddScoped<IMatchService, MatchManager>();
builder.Services.AddSingleton(new AdminCredentials(builder.Configuration["Admin:Username"] ?? "admin", builder.Configuration["Admin:Password"] ?? throw new InvalidOperationException("Admin:Password yapılandırılmalıdır. Geliştirme için Development ortamını kullanın.")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.SlidingExpiration = true;
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "local", _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = 10,
        Window = TimeSpan.FromMinutes(1),
        QueueLimit = 0
    }));
});
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LeagueContext>();
    await SeedData.InitializeAsync(context);
}

app.Run();
