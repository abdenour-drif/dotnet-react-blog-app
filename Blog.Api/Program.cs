using System.Text.Json.Serialization;
using Blog.Api.Validators;
using Blog.Api.Features.Endpoints;
using Blog.Api.Features.Services;
using Blog.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
  options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddValidation();
builder.Services.AddScoped<CreateBlogDtoValidator>();
builder.Services.AddScoped<UpdateBlogDtoValidator>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.AddBlogStoreDb();

var app = builder.Build();
app.MapBlogsEndpoints();
app.MapContributorsEndpoints();

app.MigrateDb();
app.Run();
