using Blog.Api.Dtos.Blogs;
using Blog.Api.Enums;

namespace Blog.Api.Features.Services;

public interface IBlogService
{
  // ---------------
  // GET
  // ---------------
  // /blogs
  Task<IReadOnlyList<BlogDto>> GetBlogsAsync(CancellationToken ct = default);

  // /blogs/{slug}
  Task<BlogDto?> GetBlogBySlugAsync(string slug, CancellationToken ct = default);

  // ---------------
  // POST 
  // ---------------
  // /blogs
  Task<CreateBlogResultDto> CreateAsync(CreateBlogDto newBlog, int currentContributorId, CancellationToken ct = default);

  //  /blogs/{slug}/contributors
  Task<AddBlogContributorResultDto> AddBlogContributorAsync(string slug, int currentContributorId, BlogContributorAssignmentDto contributorAssignment, CancellationToken ct = default);

  // ---------------
  // PATCH
  // ---------------
  // /blogs/{slug}
  Task<UpdateBlogResultDto> UpdateAsync(string slug, int currentContributorId, UpdateBlogDto updatedBlog, CancellationToken ct = default);

  // /blogs/{slug}/contributors/{contributorId}
  Task<UpdateBlogContributorResultDto> UpdateBlogContributorAsync(string slug, int contributorId, string role, int currentContributorId, CancellationToken ct = default);

  // ---------------
  // DELETE
  // ---------------
  // /blogs/{slug}
  Task<bool> DeleteAsync(string slug, int currentContributorId, CancellationToken ct = default);

  // /blogs/{slug}/contributors/{contributorId}
  Task<bool> RemoveBlogContributorAsync(string slug, int contributorId, int currentContributorId,  CancellationToken ct = default);
}