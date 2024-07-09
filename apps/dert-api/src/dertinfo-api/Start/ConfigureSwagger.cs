using DertInfo.Api.Start.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace DertInfo.Api.Start
{
    public static class ConfigureSwagger
    {
        public static void Apply(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc(configuration["ApiInfo:Version"], new OpenApiInfo
                {
                    Version = configuration["ApiInfo:Version"],
                    Title = configuration["ApiInfo:Title"],
                    Description = configuration["ApiInfo:Description"],
                    TermsOfService = new Uri(configuration["ApiInfo:TermsOfUse"]),
                    Contact = new OpenApiContact
                    {
                        Name = configuration["ApiInfo:ContactName"],
                        Email = configuration["ApiInfo:ContactEmail"]
                    }
                });

                //Add Security Defintion and Requirement
                //var securityScheme = Start.Swagger.SecuritySchemes.GetBearerScheme();
                //var securityRequirement = Start.Swagger.SecuritySchemes.GetBearerRequirement(securityScheme);
                //c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                //c.AddSecurityRequirement(securityRequirement);

                c.OperationFilter<Start.Swagger.AuthenticationFilter>();
            });
        }
    }
}
