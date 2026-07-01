using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DertInfo.Api.Start.Swagger
{
    public class AuthenticationFilter : IOperationFilter
    {
        private bool HasAttribute(MethodInfo methodInfo, Type type, bool inherit)
        {
            // inhertit = true also checks inherited attributes
            var actionAttributes = methodInfo.GetCustomAttributes(inherit);
            var controllerAttributes = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(inherit);
            var actionAndControllerAttributes = actionAttributes.Union(controllerAttributes);

            return actionAndControllerAttributes.Any(attr => attr.GetType() == type);
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            bool hasAuthorizeAttribute = HasAttribute(context.MethodInfo, typeof(AuthorizeAttribute), true);
            bool hasAnonymousAttribute = HasAttribute(context.MethodInfo, typeof(AllowAnonymousAttribute), true);

            var requiredScopes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Select(attr => attr.Policy)
            .Distinct();

            if (requiredScopes.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Description = "POLICIES:" + String.Concat(requiredScopes.Select(rs => rs + ":"));
            }

            // If there is no authorize or an allow anonymous
            bool isAuthenticated = hasAuthorizeAttribute && !hasAnonymousAttribute;
            if (!isAuthenticated)
            {
                operation.Description = "Unprotected Endpoint:" + operation.Description;
            }
            else
            {
                var generalScheme = Swagger.SecuritySchemes.GetAuthenticationScheme();
                var generalRequirement = Swagger.SecuritySchemes.GetAuthenticationRequirement(generalScheme);
                operation.Security.Add(generalRequirement);
            }

            // Remove the tags that Swagger is adding to the definition
            operation.Tags = new List<OpenApiTag>();
        }
    }
}
