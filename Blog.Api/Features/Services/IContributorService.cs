using Blog.Api.Dtos.Contributors;
namespace Blog.Api.Features.Services;

public interface IContributorService
{

  // ---------------
  // GET
  // ---------------
  // /contributors
  Task<IReadOnlyList<BlogContributorDto>> GetContributorsAsync(CancellationToken ct = default);

  // /contributors/{id} 
  Task<BlogContributorDto?> GetContributorByIdAsync(int id, CancellationToken ct = default);

  // /contributors/{id}/blogs
  Task<IReadOnlyList<ContributorBlogSummaryDto>> GetContributorBlogsAsync(int id, int take = 10, CancellationToken ct = default);

  // ---------------
  // POST
  // ---------------
  // /contributors
  Task<BlogContributorDto> CreateAsync(CreateContributorDto newContributor, CancellationToken ct = default);

  // ---------------
  // PATCH
  // ---------------
  // /contributors/{id}
  Task<BlogContributorDto?> UpdateAsync(int id, UpdateContributorDto updatedContributor, CancellationToken ct = default);

  // ---------------
  // DELETE
  // ---------------
  // /contributors/{id}
  Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}