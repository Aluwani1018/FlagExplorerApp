using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlagExplorerApp.Api.Filters;

public class SwaggerPathOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.RelativePath == "countries/{name}")
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "name",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }
}
