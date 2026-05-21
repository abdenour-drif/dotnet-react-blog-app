using FluentValidation;
using Blog.Api.Dtos.Blogs;
using Blog.Api.Dtos.Contributors;

namespace Blog.Api.Validators;

public class UpdateBlogDtoValidator : AbstractValidator<UpdateBlogDto>
{
  private const string AppContributorsContextKey = "AppContributors";

  public UpdateBlogDtoValidator()
  {
    RuleFor(x => x.Title)
      .NotEmpty().WithMessage("Title is required.")
      .MaximumLength(150).WithMessage("Title is too long.")
      .When(x => x.Title is not null);

    RuleFor(x => x.Content)
      .NotEmpty().WithMessage("Content cannot be empty.")
      .When(x => x.Content is not null);

    RuleFor(x => x.Contributors)
      .NotEmpty().WithMessage("Blog has to have at least one contributor.")
      .Must(HaveUniqueContributors).WithMessage("Cannot have duplicate contributors.")
      .Must((_, contributors, context) => ListAppContributorsExist(contributors, context))
      .When(x => x.Contributors is not null)
      .WithMessage("Some of the assigned contributors do not exist in the system.");
  }

  private static bool HaveUniqueContributors(IReadOnlyList<BlogContributorAssignmentDto>? contributors)
  {
    if (contributors == null) return true;
    var ids = contributors.Select(c => c.ContributorId);
    return ids.Count() == ids.Distinct().Count();
  }

  private static bool ListAppContributorsExist(
    IReadOnlyList<BlogContributorAssignmentDto>? incomingContributors,
    ValidationContext<UpdateBlogDto> context)
  {
    if (incomingContributors == null || !incomingContributors.Any()) return true;

    if (!context.RootContextData.TryGetValue(AppContributorsContextKey, out var obj)) return false;

    var appContributors = obj as List<BlogContributorDto>;
    if (appContributors is null) return false;

    var existingIds = appContributors.Select(c => c.Id).ToHashSet();
    return incomingContributors.All(c => existingIds.Contains(c.ContributorId));
  }
}