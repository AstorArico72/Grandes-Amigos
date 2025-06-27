using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

//Fuente: https://stackoverflow.com/a/73972346

public class QuitaVersion : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters != null)
        {
            var versionParameter = operation.Parameters.SingleOrDefault(p => p.Name == "version");

            if (versionParameter != null)
                operation.Parameters.Remove(versionParameter);
        }
    }
}
