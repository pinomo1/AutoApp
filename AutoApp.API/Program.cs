using System.Text.Json.Serialization;
using AutoApp.API.Extensions;
using AutoApp.Application.Services;
using AutoApp.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

#region Services
services.AddDbContextPool<AutoDbContext>(options => options.UseSqlServer(config.GetConnectionString("ResourcesHost")));
// TODO: Check FluentValidation
services.AddCors();
services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
services.AddEndpointsApiExplorer();
services.AddSwagger();
services.AddScoped<ICarService, CarService>();
services.AddScoped<IAutoDbContext, AutoDbContext>();
builder.Services.AddOpenApi();
#endregion

var app = builder.Build();

#region Configure
app.UseCors(options =>
{
    options.WithOrigins()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials();
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Site API");
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();