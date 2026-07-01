using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Api.Start.Swagger
{
    public static class SecuritySchemes
    {
        public static OpenApiSecurityScheme GetAuthenticationScheme()
        {

            return new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            };
        }

        public static OpenApiSecurityRequirement GetAuthenticationRequirement(OpenApiSecurityScheme scheme)
        {
            return new OpenApiSecurityRequirement
            {
                { scheme, Array.Empty<string>() }
            };
        }


    }
}
