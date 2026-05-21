using Blog.Api.Dtos.Blogs;
using Blog.Api.Validators;
using FluentValidation;
using Blog.Api.Features.Services;

namespace Blog.Api.Features.Endpoints;

public static class BlogsEndpoints
{
  private const string GetBlogEndpointName = "GetBlogBySlug";

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

    // POST /blogs
    group.MapPost("/", async (CreateBlogDto newBlog, CreateBlogDtoValidator validator, IBlogService service, CancellationToken ct) =>
    {
      var result = await validator.ValidateAsync(newBlog, ct);
      if (!result.IsValid)
        return Results.ValidationProblem(result.ToDictionary());

      var blog = await service.CreateAsync(newBlog, ct);
      return Results.CreatedAtRoute("GetBlogBySlug", new { slug = blog.Slug }, blog);
    }).WithName(GetBlogEndpointName);

    // PATCH /blogs/{slug}
    group.MapPatch("/{slug}", async (string slug, UpdateBlogDto updatedBlog, IBlogService service, UpdateBlogDtoValidator validator, CancellationToken ct) =>
    {
      var validationResult = await validator.ValidateAsync(updatedBlog, ct);
      if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());

      var blog = await service.UpdateAsync(slug, updatedBlog, ct);
      return blog is not null ? Results.Ok(blog) : Results.NotFound();
    });

    // DELETE /blogs/{slug}
    group.MapDelete("/{slug}", async (string slug, IBlogService service, CancellationToken ct) =>
    {
      await service.DeleteAsync(slug, ct);
      return Results.NoContent();
    });

    app.MapFallback(() => Results.NotFound(new
    {
      message = "Endpoint not found."
    }));
  }
}
