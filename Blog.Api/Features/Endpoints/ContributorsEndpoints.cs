using Blog.Api.Dtos.Contributors;
using Blog.Api.Features.Services;

namespace Blog.Api.Features.Endpoints;

public static class ContributorsEndpoints
{
  private const string GetContributorEndpointName = "GetContributorById";

  public static void MapContributorsEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/contributors").WithTags("Contributors");

    // GET /contributors
    group.MapGet("/", async (IContributorService service, CancellationToken ct) =>
     Results.Ok(await service.GetContributorsAsync(ct)));

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
      return Results.CreatedAtRoute("GetContributorById", new { id = contributor.Id }, contributor);
    }).WithName(GetContributorEndpointName);

    // PATCH /contributors/{id}
    group.MapPatch("/{id}", async (int id, UpdateContributorDto updatedContributor, IContributorService service, CancellationToken ct) =>
    {
      var contributor = await service.UpdateAsync(id, updatedContributor, ct);
      return contributor is not null ? Results.Ok(contributor) : Results.NotFound();
    });

    // DELETE /contributors/{id}
    group.MapDelete("/{id}", async (int id, IContributorService service, CancellationToken ct) =>
    {
      await service.DeleteAsync(id, ct);
      return Results.NoContent();
    });

    app.MapFallback(() => Results.NotFound(new
    {
      message = "Endpoint not found."
    }));
  }
}
