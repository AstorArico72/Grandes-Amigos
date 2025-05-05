using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

//Fuente: https://stackoverflow.com/a/73972346
public class ReemplazaVersion : IDocumentFilter {
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) {
        var paths = swaggerDoc.Paths;

        swaggerDoc.Paths = new OpenApiPaths();

        foreach (var path in paths) {
            var key = path.Key.Replace("v{version}", context.DocumentName);

            var value = path.Value;

            swaggerDoc.Paths.Add(key, value);
        }
    }
}