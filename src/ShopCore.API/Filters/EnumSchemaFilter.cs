using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ShopCore.API.Filters;

public class EnumSchemaFilter : ISchemaFilter
{
    private static string BuildDescription(Type enumType) =>
        string.Join(", ",
            Enum.GetValues(enumType)
                .Cast<object>()
                .Select(e => $"{(int)e} = {e}"));

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;

        if (schema.Enum != null)
        {
            schema.Enum.Clear();
        }
        schema.Type = JsonSchemaType.Integer;
        schema.Format = "int32";
        schema.Description = BuildDescription(context.Type);
    }

    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;

        if (schema is OpenApiSchema concreteSchema)
        {
            concreteSchema.Enum.Clear();
            concreteSchema.Type = JsonSchemaType.Integer;
            concreteSchema.Format = "int32";
        }

        schema.Description = BuildDescription(context.Type);
    }
}