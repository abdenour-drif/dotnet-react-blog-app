using Blog.Api.Dtos.Contributors;
using Blog.Api.Features.Services;
using System.Security.Claims;

namespace Blog.Api.Features.Endpoints;

public static class ContributorsEndpoints
{
  private const string GetContributorEndpointName = "GetContributorById";

  private static bool TryGetCurrentContributorId(ClaimsPrincipal user, out int contributorId)
  {
    contributorId = 0;
    var value = user.FindFirst("contributorId")?.Value;
    return int.TryParse(value, out contributorId);
  }

  public static void MapContributorsEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/contributors").WithTags("Contributors");

    // GET /contributors
    group.MapGet("/", async (IContributorService service, CancellationToken ct) =>
     Results.Ok(await service.GetContributorsAsync(ct)));

    // GET /contributors/{id}/blogs
    group.MapGet("/{id}/blogs", async (int id, IContributorService service, CancellationToken ct) =>
    {
      if (id <= 0)
        return Results.BadRequest("Invalid contributor id.");

      var contributor = await service.GetContributorByIdAsync(id, ct);
      if (contributor is null)
        return Results.NotFound();

      return Results.Ok(await service.GetContributorBlogsAsync(id, 10, ct));
    });

    // GET /contributors/{id}
    group.MapGet("/{id}", async (int id, IContributorService service, CancellationToken ct) =>
    {
      var contributor = await service.GetContributorByIdAsync(id, ct);
      return contributor is not null ? Results.Ok(contributor) : Results.NotFound();
    }).WithName(GetContributorEndpointName);



    // POST /contributors
    group.MapPost("/", async (CreateContributorDto newContributor, IContributorService service, CancellationToken ct) =>
    {

      var contributor = await service.CreateAsync(newContributor, ct);
      return Results.CreatedAtRoute(GetContributorEndpointName, new { id = contributor.Id }, contributor);
    }).RequireAuthorization();

    // PATCH /contributors/{id}
    group.MapPatch("/{id}", async (int id, UpdateContributorDto updatedContributor, IContributorService service, CancellationToken ct) =>
    {
      var contributor = await service.UpdateAsync(id, updatedContributor, ct);
      return contributor is not null ? Results.Ok(contributor) : Results.NotFound();
    }).RequireAuthorization();

    // DELETE /contributors/{id}
    group.MapDelete("/{id}", async (int id, IContributorService service, CancellationToken ct) =>
    {
      await service.DeleteAsync(id, ct);
      return Results.NoContent();
    }).RequireAuthorization();
  }
}
