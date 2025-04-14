using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlagExplorerApp.Api.Filters;

public class SwaggerPathOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.RelativePath == "countries/{name}")
        {
            if (operation.Parameters.All(p => p.Name != "name"))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "name",
                    In = ParameterLocation.Path,
                    Required = true,
                    Description = "The name of the country to retrieve details for.",
                    Schema = new OpenApiSchema { Type = "string" }
                });
            }
        }
    }
}
