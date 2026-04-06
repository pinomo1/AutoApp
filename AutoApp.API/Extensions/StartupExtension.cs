using System.Reflection;
using Microsoft.OpenApi;

namespace AutoApp.API.Extensions;

public static class StartupExtension
{
    extension(IServiceCollection services)
    {
        public void AddSwagger()
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AutoApp", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}