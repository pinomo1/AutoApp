using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using AutoApp.Application.Services;
using AutoApp.Application.Services.Interfaces;
using AutoApp.Application.Validators;
using AutoApp.Infrastructure.Persistence.DbContexts;
using AutoApp.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

namespace AutoApp.API.Extensions;

/// <summary>
/// Various extensions for startup
/// </summary>
public static class StartupExtension
{
    /// <param name="builder">Web application builder.</param>
    extension(WebApplicationBuilder builder)
    {
        /// <summary>
        /// Registers API services and dependencies.
        /// </summary>
        public void AddApiServices()
        {
            var services = builder.Services;
            var config = builder.Configuration;

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

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<PaginatedQueryValidator>();
            services.AddEndpointsApiExplorer();
            services.AddSwagger();
            services.AddAppServices();
            services.AddOptions<BrandLogoStorageOptions>()
                .Bind(config.GetSection(BrandLogoStorageOptions.SectionName))
                .ValidateDataAnnotations();
            services.AddExceptionHandler<AutoExceptionHandler>();
            services.AddOpenApi();
            services.AddHealthChecks();
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(policyName: "fixed", configureOptions: policy =>
                {
                    policy.PermitLimit = 100;
                    policy.Window = TimeSpan.FromMinutes(1);
                    policy.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    policy.QueueLimit = 0;
                });
            });
        }
    }

    /// <param name="app">Web application instance.</param>
    extension(WebApplication app)
    {
        /// <summary>
        /// Configures middleware and endpoint mapping for the API.
        /// </summary>
        public void UseApiPipeline()
        {
            app.UseCors(options =>
            {
                options.WithOrigins()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Site API"); });

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseExceptionHandler("/error");
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();
            app.MapHealthChecks("/health");
        }

        /// <summary>
        /// Applies pending Entity Framework migrations at startup.
        /// </summary>
        public void ApplyDatabaseMigrations()
        {
            if (app.Environment.IsEnvironment("Testing"))
            {
                return;
            }

            using var scope = app.Services.CreateScope();
            using var context = scope.ServiceProvider.GetService<AutoDbContext>();
            context?.Database.Migrate();
        }
    }

    /// <param name="services">Service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds swagger to the services
        /// </summary>
        private void AddSwagger()
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AutoApp", Version = "v1" });
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.AllDirectories).ToList();
                xmlFiles.ForEach(xmlFile => c.IncludeXmlComments(xmlFile));
            });
        }
        
        /// <summary>
        /// Add application services to the services
        /// </summary>
        private void AddAppServices()
        {
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICarService, CarService>();
            services.AddScoped<IFeatureService, FeatureService>();
            services.AddScoped<IAutoDbContext, AutoDbContext>();
            services.AddScoped<IBrandLogoStorage, SftpBrandLogoStorage>();
        }
    }
}