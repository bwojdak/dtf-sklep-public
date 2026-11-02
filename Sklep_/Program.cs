using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sklep_.Data;
using Sklep_.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHostedService<VintedWatcher>();
builder.Services.AddHttpClient<OpenAiService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=sklep.db"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService
        <UserManager<IdentityUser>>();
    var userName = app.Configuration["Admin:UserName"];
    var password = app.Configuration["Admin:Password"];

    if (!string.IsNullOrEmpty(userName) &&
        !string.IsNullOrEmpty(password)
        && await userManager.FindByNameAsync(userName) ==
        null)
    {
        var result = await userManager.CreateAsync(new
            IdentityUser { UserName = userName }, password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ",
                result.Errors.Select(e => e.Description)));
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets().AllowAnonymous();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();