using Blog.Api.Dtos.Contributors;

namespace Blog.Api.Features.Services;

public interface IContributorService
{
  Task<IReadOnlyList<BlogContributorDto>> GetContributorsAsync(CancellationToken ct = default);
  Task<BlogContributorDto?> GetContributorByIdAsync(int id, CancellationToken ct = default);
  Task<BlogContributorDto> CreateAsync(CreateContributorDto newContributor, CancellationToken ct = default);
  Task<BlogContributorDto?> UpdateAsync(int id, UpdateContributorDto updatedContributor, CancellationToken ct = default);
  Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}