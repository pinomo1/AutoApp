using AutoApp.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.AddApiServices();

var app = builder.Build();
app.UseApiPipeline();
await app.ApplyDatabaseMigrationsAsync();

app.Run();
