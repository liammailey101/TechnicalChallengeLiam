using Microsoft.AspNetCore.Authentication.Cookies;
using TechnicalChallenge.BusinessService;

var builder = WebApplication.CreateBuilder(args);

// Configure logging to log to the console
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddBusinessConfiguration();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Security/Login";
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

// Set up demo data
using (var scope = app.Services.CreateScope())
{
    BusinessConfiguration.SetUpDemoData(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Security}/{action=Login}")
    .WithStaticAssets();


app.Run();
