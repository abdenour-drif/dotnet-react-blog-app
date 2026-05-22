using System.Text.Json.Serialization;
using Blog.Api.Features.Endpoints;
using Blog.Api.Features.Services;
using Blog.Api.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
  options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie(options =>
  {
    options.Cookie.Name = "blog.auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.MaxAge = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;

    options.Events.OnRedirectToLogin = context =>
    {
      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
      context.Response.StatusCode = StatusCodes.Status403Forbidden;
      return Task.CompletedTask;
    };
  });

builder.Services.AddAuthorization();
builder.Services.AddValidation();

builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IContributorService, ContributorService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.AddBlogStoreDb();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlogsEndpoints();
app.MapContributorsEndpoints();
app.MapAuthEndpoints();

app.MapFallback(() => Results.NotFound(new
{
  message = "Endpoint not found."
}));

app.MigrateDb();
app.Run();
