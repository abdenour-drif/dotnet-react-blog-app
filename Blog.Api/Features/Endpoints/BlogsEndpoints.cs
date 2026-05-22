using Blog.Api.Dtos.Blogs;
using Blog.Api.Features.Services;
using Blog.Api.Enums;
using System.Security.Claims;

namespace Blog.Api.Features.Endpoints;

public static class BlogsEndpoints
{
  private const string GetBlogEndpointName = "GetBlogBySlug";

  private static bool TryGetCurrentContributorId(ClaimsPrincipal user, out int contributorId)
  {
    contributorId = 0;
    var value = user.FindFirst("contributorId")?.Value;
    return int.TryParse(value, out contributorId);
  }

  public static void MapBlogsEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/blogs").WithTags("Blogs");

    // GET /blogs
    group.MapGet("/", async (IBlogService service, CancellationToken ct) =>
     Results.Ok(await service.GetBlogsAsync(ct)));

    // GET /blogs/{slug}
    group.MapGet("/{slug}", async (string slug, IBlogService service, CancellationToken ct) =>
    {
      var blog = await service.GetBlogBySlugAsync(slug, ct);
      return blog is not null ? Results.Ok(blog) : Results.NotFound();
    }).WithName(GetBlogEndpointName);

    // POST /blogs/{slug}/contributors
    group.MapPost("/{slug}/contributors", async (string slug, BlogContributorAssignmentDto contributorAssignment, ClaimsPrincipal user, IBlogService service, CancellationToken ct) =>
    {
      if (!TryGetCurrentContributorId(user, out var currentContributorId))
        return Results.Unauthorized();

      var result = await service.AddBlogContributorAsync(slug, currentContributorId, contributorAssignment, ct);

      return result.Error switch
      {
        AddBlogContributorErrorEnum.None => Results.Ok(result.Contributors!),
        AddBlogContributorErrorEnum.BlogNotFound => Results.BadRequest(new
        {
          message = "Blog not found."
        }),
        AddBlogContributorErrorEnum.ContributorNotFound => Results.BadRequest(new
        {
          message = "Contributor do not exist."
        }),
        AddBlogContributorErrorEnum.ContributorAlreadyAssigned => Results.BadRequest(new
        {
          message = "Contributor is already assigned to this blog."
        }),
        AddBlogContributorErrorEnum.RoleNotFound => Results.BadRequest(new
        {
          message = "Role not found."
        }),
        AddBlogContributorErrorEnum.Forbidden => Results.Forbid(),
        AddBlogContributorErrorEnum.SaveFailed => Results.Problem("Failed to save blog."),
        _ => Results.Problem("Unknown error.")
      };

    }).RequireAuthorization();

    // POST /blogs
    group.MapPost("/", async (CreateBlogDto newBlog, ClaimsPrincipal user, IBlogService service, CancellationToken ct) =>
    {
      if (!TryGetCurrentContributorId(user, out var currentContributorId))
        return Results.Unauthorized();

      var result = await service.CreateAsync(newBlog, currentContributorId, ct);

      return result.Error switch
      {
        CreateBlogError.None => Results.CreatedAtRoute(GetBlogEndpointName,
        new { slug = result.Blog!.Slug },
        result.Blog),

        CreateBlogError.ContributorsNotFound => Results.BadRequest(new
        {
          message = "Some contributors do not exist.",
          missingContributorIds = result.MissingContributorIds
        }),
        CreateBlogError.Forbidden => Results.Forbid(),
        CreateBlogError.SaveFailed => Results.Problem("Failed to save blog."),
        _ => Results.Problem("Unknown error.")
      };

    }).RequireAuthorization();

    // PATCH /blogs/{slug}/contributors/{contributorId}
    group.MapPatch("/{slug}/contributors/{contributorId:int}", async (string slug, int contributorId, string role, ClaimsPrincipal user, IBlogService service, CancellationToken ct) =>
    {
      if (!TryGetCurrentContributorId(user, out var currentContributorId))
        return Results.Unauthorized();

      var result = await service.UpdateBlogContributorAsync(slug, contributorId, role, currentContributorId, ct);
      return result.Error switch
      {
        UpdateBlogContributorRoleErrorEnum.None => Results.Ok(result.Role!),
        UpdateBlogContributorRoleErrorEnum.BlogNotFound => Results.BadRequest(new
        {
          message = "Blog not found."
        }),
        UpdateBlogContributorRoleErrorEnum.ContributorNotFound => Results.BadRequest(new
        {
          message = "Contributor not found."
        }),
        UpdateBlogContributorRoleErrorEnum.ContributorNotAssignedToBlog => Results.BadRequest(new { message = "Contributor is not assigned to this blog." }),
        UpdateBlogContributorRoleErrorEnum.LastAuthorCannotBeChanged => Results.BadRequest(new
        {
          message = "The last author cannot be changed."
        }),
        UpdateBlogContributorRoleErrorEnum.RoleNotFound => Results.BadRequest(new
        {
          message = "Role not found."
        }),
        UpdateBlogContributorRoleErrorEnum.Forbidden => Results.Forbid(),
        UpdateBlogContributorRoleErrorEnum.SaveFailed => Results.Problem("Failed to save blog."),
        _ => Results.Problem("Unknown error.")
      };
    }).RequireAuthorization();

    // PATCH /blogs/{slug}
    group.MapPatch("/{slug}", async (string slug, ClaimsPrincipal user, UpdateBlogDto updatedBlog, IBlogService service, CancellationToken ct) =>
    {
      if (!TryGetCurrentContributorId(user, out var currentContributorId))
        return Results.Unauthorized();

      var result = await service.UpdateAsync(slug, currentContributorId, updatedBlog, ct);
      return result.Error switch
      {
        UpdateBlogError.None => Results.Ok(result.Blog!),
        UpdateBlogError.BlogNotFound => Results.BadRequest(new
        {
          message = "Blog not found."
        }),
        UpdateBlogError.ContributorsUpdateNotAllowed => Results.BadRequest(new
        {
          message = "Updating contributors is not allowed",
        }),
        UpdateBlogError.SaveFailed => Results.Problem("Failed to save blog."),
        UpdateBlogError.Forbidden => Results.Forbid(),
        _ => Results.Problem("Unknown error.")
      };
    }).RequireAuthorization();

    // DELETE /blogs/{slug}/contributors/{contributorId}
    group.MapDelete("/{slug}/contributors/{contributorId:int}", async (string slug, int contributorId, ClaimsPrincipal user, IBlogService service, CancellationToken ct) =>
    {
      if (!TryGetCurrentContributorId(user, out var currentContributorId))
        return Results.Unauthorized();

      var removed = await service.RemoveBlogContributorAsync(slug, contributorId, currentContributorId, ct);
      return removed ? Results.NoContent() : Results.BadRequest();
    }).RequireAuthorization();

    // DELETE /blogs/{slug}
    group.MapDelete("/{slug}", async (string slug, ClaimsPrincipal user, IBlogService service, CancellationToken ct) =>
    {
      if (!TryGetCurrentContributorId(user, out var currentContributorId))
        return Results.Unauthorized();

      var removed = await service.DeleteAsync(slug, currentContributorId, ct);
      return removed ? Results.NoContent() : Results.BadRequest();
    }).RequireAuthorization();
  }
}
