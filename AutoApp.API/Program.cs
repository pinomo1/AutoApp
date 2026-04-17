using AutoApp.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddApiServices();

var app = builder.Build();
app.UseApiPipeline();
app.ApplyDatabaseMigrations();

app.Run();