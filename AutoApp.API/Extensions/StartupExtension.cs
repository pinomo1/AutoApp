using Microsoft.OpenApi;

namespace AutoApp.API.Extensions;

/// <summary>
/// Various extensions for startup
/// </summary>
public static class StartupExtension
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds swagger to the services
        /// </summary>
        public void AddSwagger()
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AutoApp", Version = "v1" });
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory,"*.xml",SearchOption.AllDirectories).ToList();
                xmlFiles.ForEach(xmlFile => c.IncludeXmlComments(xmlFile)); 
            });
        }
    }
}