using System.Text.Json.Serialization;
using AutoApp.API.Extensions;
using AutoApp.Application.Services;
using AutoApp.Application.Services.Interfaces;
using AutoApp.Infrastructure.Persistence.DbContexts;
using AutoApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

#region Services
services.AddDbContextPool<AutoDbContext>(options => 
    options
        .UseSqlServer(config.GetConnectionString("ResourcesHost"))
        .AddInterceptors(new AutoSaveChangesInterceptor())
    );
services.AddCors();
services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
services.AddEndpointsApiExplorer();
services.AddSwagger();
services.AddScoped<ICountryService, CountryService>();
services.AddScoped<IBrandService, BrandService>();
services.AddScoped<ICarService, CarService>();
services.AddScoped<IFeatureService, FeatureService>();
services.AddScoped<IAutoDbContext, AutoDbContext>();
services.AddExceptionHandler<AutoExceptionHandler>();
services.AddOpenApi();
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

app.UseExceptionHandler("/error");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetService<AutoDbContext>())context?.Database.Migrate();
#endregion

app.Run();