using Blog.Api.Dtos.Blogs;

namespace Blog.Api.Features.Services;

public interface IBlogService
{
  Task<IReadOnlyList<BlogDto>> GetBlogsAsync(CancellationToken ct = default);
  Task<BlogDto?> GetBlogBySlugAsync(string slug, CancellationToken ct = default);
  Task<BlogDto> CreateAsync(CreateBlogDto newBlog, CancellationToken ct = default);
  Task<BlogDto?> UpdateAsync(string slug, UpdateBlogDto updatedBlog, CancellationToken ct = default);
  Task<bool> DeleteAsync(string slug, CancellationToken ct = default);
}