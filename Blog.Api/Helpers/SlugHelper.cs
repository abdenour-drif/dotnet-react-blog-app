using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
namespace Blog.Api.Helpers;
public static partial class SlugHelper
{
  [GeneratedRegex(@"[^a-z0-9\s-_]")]
  private static partial Regex NonAlphaNumericRegex();

  [GeneratedRegex(@"[\s-_]+")]
  private static partial Regex MultipleDashesRegex();
  public static string CreateUniqueSlug(string title, IEnumerable<string> existingSlugs)
  {
    var baseSlug = Slugify(title);

    var existingSlugSet = existingSlugs.ToHashSet(StringComparer.OrdinalIgnoreCase);

    var slug = baseSlug;
    var counter = 2;

    while (existingSlugSet.Contains(slug))
    {
      slug = $"{baseSlug}-{counter++}";
    }

    return slug;
  }

  static string Slugify(string title)
  {
    if (string.IsNullOrWhiteSpace(title)) return Guid.NewGuid().ToString("N");

    string normalized = title.Normalize(NormalizationForm.FormD);
    var sb = new StringBuilder(normalized.Length);

    foreach (char ch in normalized)
    {
      var category = CharUnicodeInfo.GetUnicodeCategory(ch);
      if (category != UnicodeCategory.NonSpacingMark)
      {
        sb.Append(ch);
      }
    }
    string result = sb
      .ToString()
      .Normalize(NormalizationForm.FormC)
      .ToLowerInvariant();

    result = NonAlphaNumericRegex().Replace(result, "");
    result = MultipleDashesRegex().Replace(result, "-").Trim('-');

    return string.IsNullOrEmpty(result) ? Guid.NewGuid().ToString("N") : result;
  }
}
