
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Data;

public static class DataExtensions
{
  public static void MigrateDb(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BlogStoreContext>();
    dbContext.Database.Migrate();
  }

  public static void AddBlogStoreDb(this WebApplicationBuilder builder)
  {
    var connString = builder.Configuration.GetConnectionString("BlogStore")
      ?? throw new InvalidOperationException(
        "Connection string 'BlogStore' is missing.");

    // Ensure the SQLite database directory exists.
    var sqliteBuilder = new SqliteConnectionStringBuilder(connString);

    if (!string.IsNullOrWhiteSpace(sqliteBuilder.DataSource)
        && sqliteBuilder.DataSource != ":memory:")
    {
      var databasePath = Path.GetFullPath(sqliteBuilder.DataSource);
      var directory = Path.GetDirectoryName(databasePath);

      if (!string.IsNullOrEmpty(directory))
      {
        Directory.CreateDirectory(directory);
      }
    }

    builder.Services.AddScoped<BlogStoreContext>();
    builder.Services.AddSqlite<BlogStoreContext>(connString);
  }
}
