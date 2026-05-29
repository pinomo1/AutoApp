using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using AutoApp.API.Authorization;
using AutoApp.API.Identity;
using AutoApp.Application.Services;
using AutoApp.Application.Services.Interfaces;
using AutoApp.Application.Validators;
using AutoApp.Infrastructure.Identity;
using AutoApp.Infrastructure.Persistence.DbContexts;
using AutoApp.Infrastructure.Persistence.Seed;
using AutoApp.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;

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
        /// Configures Serilog as the application's logging provider.
        /// </summary>
        public void AddSerilogLogging()
        {
            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });
        }

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
            services.AddControllers(options =>
                {
                    options.Conventions.Add(new AdminOnlyNonGetConvention());
                })
                .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<PaginatedQueryValidator>();
            services.AddEndpointsApiExplorer();
            services.AddSwagger();
            services.AddOptions<JwtOptions>()
                .Bind(config.GetSection(JwtOptions.SectionName))
                .ValidateDataAnnotations();
            services.AddOptions<IdentitySeedAdminOptions>()
                .Bind(config.GetSection(IdentitySeedAdminOptions.SectionName));
            services.AddScoped<TokenService>();
            services.AddIdentityCore<ApplicationUser>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AutoDbContext>()
                .AddSignInManager();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtOptions = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1),
                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role
                    };
                });
            services.AddAuthorization();
            services.AddAppServices();
            services.AddOptions<BrandLogoStorageOptions>()
                .Bind(config.GetSection(BrandLogoStorageOptions.SectionName))
                .ValidateDataAnnotations();
            services.AddOptions<CarPhotoStorageOptions>()
                .Bind(config.GetSection(CarPhotoStorageOptions.SectionName))
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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();
            app.MapHealthChecks("/health");
        }

        /// <summary>
        /// Applies pending Entity Framework migrations at startup.
        /// </summary>
        public async Task ApplyDatabaseMigrationsAsync()
        {
            if (app.Environment.IsEnvironment("Testing"))
            {
                return;
            }

            await using var scope = app.Services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<AutoDbContext>();
            await context.Database.MigrateAsync();
            await WebApplication.SeedIdentityAsync(scope.ServiceProvider, app.Configuration);
            await SampleDataSeeder.SeedAsync(scope.ServiceProvider);
        }

        private static async Task SeedIdentityAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var seedOptions = configuration.GetSection(IdentitySeedAdminOptions.SectionName).Get<IdentitySeedAdminOptions>() ?? new IdentitySeedAdminOptions();
            if (!seedOptions.Enabled)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(seedOptions.UserName) ||
                string.IsNullOrWhiteSpace(seedOptions.Email) ||
                string.IsNullOrWhiteSpace(seedOptions.Password))
            {
                throw new InvalidOperationException("Identity seed admin settings are enabled but incomplete.");
            }

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync(seedOptions.Role))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(seedOptions.Role));
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", roleResult.Errors.Select(error => error.Description)));
                }
            }

            var user = await userManager.FindByNameAsync(seedOptions.UserName)
                ?? await userManager.FindByEmailAsync(seedOptions.Email);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = seedOptions.UserName,
                    Email = seedOptions.Email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, seedOptions.Password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", createResult.Errors.Select(error => error.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(user, seedOptions.Role))
            {
                var addToRoleResult = await userManager.AddToRoleAsync(user, seedOptions.Role);
                if (!addToRoleResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", addToRoleResult.Errors.Select(error => error.Description)));
                }
            }
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
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Paste a JWT access token here. Example: Bearer eyJhbGciOi...",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(document =>
                {
                    var requirement = new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecuritySchemeReference("Bearer", document),
                            [] // no scopes for JWT
                        }
                    };
                    return requirement;
                });
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
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
            services.AddScoped<ICarPhotoService, CarPhotoService>();
            services.AddScoped<IFeatureService, FeatureService>();
            services.AddScoped<IAutoDbContext, AutoDbContext>();

            // Register storage implementations based on configured protocol
            services.AddScoped<IBrandLogoStorage>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<BrandLogoStorageOptions>>();
                return options.Value.Protocol switch
                {
                    StorageProtocol.Ftp or StorageProtocol.Ftps => new FtpBrandLogoStorage(options),
                    _ => new SftpBrandLogoStorage(options) // Default to SFTP
                };
            });

            services.AddScoped<ICarPhotoStorage>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<CarPhotoStorageOptions>>();
                return options.Value.Protocol switch
                {
                    StorageProtocol.Ftp or StorageProtocol.Ftps => new FtpCarPhotoStorage(options),
                    _ => new SftpCarPhotoStorage(options) // Default to SFTP
                };
            });
        }
    }
}
