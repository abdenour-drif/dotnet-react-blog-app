using System.ComponentModel.DataAnnotations;

namespace Blog.Api.Dtos.Contributors;

public record BlogContributorDto(
  int Id,
  [Required] string DisplayName,
  string Email,
  string? FirstName,
  IReadOnlyList<string> MiddleNames,
  string? LastName,
  string? AvatarUrl,
  string? Bio,
  DateTimeOffset CreatedAt,
  DateTimeOffset UpdatedAt
);