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
    var connString = builder.Configuration.GetConnectionString("BlogStore");
    builder.Services.AddScoped<BlogStoreContext>();
    builder.Services.AddSqlite<BlogStoreContext>(connString);
  }
}