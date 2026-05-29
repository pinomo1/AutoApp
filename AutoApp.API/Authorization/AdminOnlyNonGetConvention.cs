using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace AutoApp.API.Authorization;

/// <summary>
/// Adds an admin-only authorization filter to every non-GET controller action.
/// </summary>
public sealed class AdminOnlyNonGetConvention : IApplicationModelConvention
{
    /// <inheritdoc />
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var action in controller.Actions)
            {
                if (action.Attributes.OfType<AllowAnonymousAttribute>().Any())
                {
                    continue;
                }

                var httpMethods = action.Selectors
                    .SelectMany(selector => selector.ActionConstraints.OfType<HttpMethodActionConstraint>())
                    .SelectMany(constraint => constraint.HttpMethods)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (httpMethods.Length == 0 || httpMethods.All(HttpMethods.IsGet))
                {
                    continue;
                }

                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireRole("Admin")
                    .Build();

                action.Filters.Add(new AuthorizeFilter(policy));
            }
        }
    }
}
