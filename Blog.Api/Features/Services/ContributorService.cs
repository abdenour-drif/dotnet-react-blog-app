using Blog.Api.Data;
using Blog.Api.Dtos.Blogs;
using Blog.Api.Dtos.Contributors;
using Blog.Api.Helpers;
using Microsoft.EntityFrameworkCore;
using BlogModel = Blog.Api.Models.Blog;
using ContributorModel = Blog.Api.Models.Contributor;

namespace Blog.Api.Features.Services;

public class ContributorService(BlogStoreContext dbContext) : IContributorService
{
  public async Task<IReadOnlyList<BlogContributorDto>> GetContributorsAsync(CancellationToken ct = default)
  {
    return await dbContext.Contributors
      .AsNoTracking()
      .Select(c => new BlogContributorDto(
        c.Id,
        c.DisplayName,
        c.Email,
        c.FirstName,
        c.MiddleNames.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList(),
        c.LastName,
        c.AvatarUrl ?? string.Empty,
        c.Bio ?? string.Empty,
        c.CreatedAt,
        c.UpdatedAt
      ))
      .ToListAsync(ct);
  }

  public async Task<BlogContributorDto?> GetContributorByIdAsync(int id, CancellationToken ct = default)
  {
    return await dbContext.Contributors
      .AsNoTracking()
      .Where(c => c.Id == id)
      .Select(c => new BlogContributorDto(
        c.Id,
        c.DisplayName,
        c.Email,
        c.FirstName,
        c.MiddleNames.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList(),
        c.LastName,
        c.AvatarUrl ?? string.Empty,
        c.Bio ?? string.Empty,
        c.CreatedAt,
        c.UpdatedAt
      ))
      .FirstOrDefaultAsync(ct);
  }

  public async Task<IReadOnlyList<ContributorBlogSummaryDto>> GetContributorBlogsAsync(int id, int take = 10, CancellationToken ct = default)
  {
    return await dbContext.BlogContributors
      .Where(bc => bc.ContributorId == id)
      .OrderByDescending(bc => bc.Blog.Id)
      .Take(take)
      .Select(bc => new ContributorBlogSummaryDto(
        bc.Blog.Slug,
        bc.Blog.Title,
        bc.Role
      ))
      .ToListAsync(ct);
  }
  public async Task<BlogContributorDto> CreateAsync(CreateContributorDto newContributor, CancellationToken ct = default)
  {
    ContributorModel contributor = new()
    {
      DisplayName = newContributor.DisplayName,
      Email = newContributor.Email,
      FirstName = newContributor.FirstName ?? string.Empty,
      MiddleNames = string.Join(",", newContributor.MiddleNames ?? []),
      LastName = newContributor.LastName ?? string.Empty,
      AvatarUrl = newContributor.AvatarUrl,
      Bio = newContributor.Bio,
      CreatedAt = DateTimeOffset.UtcNow,
      UpdatedAt = DateTimeOffset.UtcNow
    };

    dbContext.Contributors.Add(contributor);
    await dbContext.SaveChangesAsync(ct);

    return new BlogContributorDto(
      contributor.Id,
      contributor.DisplayName,
      contributor.Email,
      contributor.FirstName,
      [.. contributor.MiddleNames.Split(",", StringSplitOptions.RemoveEmptyEntries)],
      contributor.LastName,
      contributor.AvatarUrl,
      contributor.Bio,
      contributor.CreatedAt,
      contributor.UpdatedAt
    );
  }

  public async Task<BlogContributorDto?> UpdateAsync(int id, UpdateContributorDto updatedContributor, CancellationToken ct = default)
  {
    var contributor = await dbContext.Contributors.FindAsync([id], ct);
    if (contributor is null)
      return null;

    contributor.DisplayName = updatedContributor.DisplayName;
    contributor.Email = updatedContributor.Email;
    contributor.FirstName = updatedContributor.FirstName ?? contributor.FirstName;
    contributor.MiddleNames = updatedContributor.MiddleNames?.ToString() ?? contributor.MiddleNames;
    contributor.LastName = updatedContributor.LastName ?? contributor.LastName;
    contributor.AvatarUrl = updatedContributor.AvatarUrl;
    contributor.Bio = updatedContributor.Bio;
    contributor.UpdatedAt = DateTimeOffset.UtcNow;

    await dbContext.SaveChangesAsync(ct);

    return new BlogContributorDto(
      contributor.Id,
      contributor.DisplayName,
      contributor.Email,
      contributor.FirstName,
      [.. contributor.MiddleNames.Split(",", StringSplitOptions.RemoveEmptyEntries)],
      contributor.LastName,
      contributor.AvatarUrl,
      contributor.Bio,
      contributor.CreatedAt,
      contributor.UpdatedAt
    );
  }

  public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
  {
    var contributor = await dbContext.Contributors.FindAsync([id], ct);
    if (contributor is null)
      return false;

    dbContext.Contributors.Remove(contributor);
    await dbContext.SaveChangesAsync(ct);
    return true;
  }
}