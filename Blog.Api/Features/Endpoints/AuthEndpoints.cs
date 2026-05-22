using System.Security.Claims;
using Blog.Api.Dtos.Users;
using Blog.Api.Features.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Blog.Api.Features.Endpoints;

public static class AuthEndpoints
{
  public static void MapAuthEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/auth").WithTags("Auth");

    group.MapPost("/register", async (
      RegisterUserDto dto,
      IAuthService service,
      HttpContext httpContext,
      CancellationToken ct) =>
    {
      try
      {
        var user = await service.RegisterAsync(dto, ct);

        var claims = new List<Claim>
        {
          new(ClaimTypes.NameIdentifier, user.Id.ToString()),
          new(ClaimTypes.Email, user.Email),
          new("contributorId", user.ContributorId.ToString()),
          new(ClaimTypes.Name, user.DisplayName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(
          CookieAuthenticationDefaults.AuthenticationScheme,
          principal,
          new AuthenticationProperties
          {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
          });

        return Results.Ok(user);
      }
      catch (InvalidOperationException ex)
      {
        return Results.BadRequest(new { message = ex.Message });
      }
    });

    group.MapPost("/login", async (
      LoginUserDto dto,
      IAuthService service,
      HttpContext httpContext,
      CancellationToken ct) =>
    {
      var user = await service.LoginAsync(dto, ct);
      if (user is null)
        return Results.BadRequest(new { message = "Invalid credentials." });

      var claims = new List<Claim>
      {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email),
        new("contributorId", user.ContributorId.ToString()),
        new(ClaimTypes.Name, user.DisplayName)
      };

      var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      var principal = new ClaimsPrincipal(identity);

      await httpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        new AuthenticationProperties
        {
          IsPersistent = true,
          ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        });

      return Results.Ok(user);
    });

    group.MapPost("/logout", async (HttpContext httpContext) =>
    {
      await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      return Results.Ok(new { message = "Logged out." });
    });

    group.MapGet("/me", (ClaimsPrincipal user) =>
    {
      if (user.Identity?.IsAuthenticated != true)
        return Results.Unauthorized();

      return Results.Ok(new
      {
        userId = user.FindFirstValue(ClaimTypes.NameIdentifier),
        email = user.FindFirstValue(ClaimTypes.Email),
        contributorId = user.FindFirstValue("contributorId"),
        displayName = user.Identity?.Name
      });
    }).RequireAuthorization();
  }
}