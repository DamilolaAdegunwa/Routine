using BlockSms.Core.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BlockSms.Core.Extension
{
    public static class CustomExtensionsMethods
    {

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(configuration["Name"], new Info
                {
                    Title = configuration["Title"],
                    Version = configuration["Version"],
                    Description = configuration["Description"],
                    Contact = new Contact
                    {
                        Name = configuration["Contact:Name"],
                        Email = configuration["Contact:Email"]
                    }
                });
                options.OperationFilter<AddTokenHeaderParameter>();
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                if (Directory.Exists(basePath))
                {
                    var xmlPath = Path.Combine(basePath, $"{configuration["Name"]}.xml");
                    if (File.Exists(xmlPath))
                        options.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

    }
}
