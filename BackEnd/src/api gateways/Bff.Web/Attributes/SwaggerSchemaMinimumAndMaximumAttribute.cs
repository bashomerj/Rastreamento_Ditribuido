using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace Bff.Web.Attributes
{

    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Struct |
        AttributeTargets.Enum |
        AttributeTargets.Parameter |
        AttributeTargets.Property,
        AllowMultiple = false)]
    public class SwaggerSchemaMinimumAndMaximumAttribute : Attribute
    {
        public int? minimum { get; set; }
        public int? maximum { get; set; }
        public SwaggerSchemaMinimumAndMaximumAttribute(int minimum = -1, int maximum = -1)
        {
            if (minimum == -1) this.minimum = null; else this.minimum = minimum;
            if (maximum == -1) this.minimum = null; else this.maximum = maximum;
        }
    }

    public class SwaggerSchemaMinimumAndMaximumFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.MemberInfo != null)
            {
                var schemaAttribute = context.MemberInfo.GetCustomAttributes<SwaggerSchemaMinimumAndMaximumAttribute>()
                    .FirstOrDefault();
                if (schemaAttribute != null)
                    ApplySchemaAttribute(schema, schemaAttribute);
            }
        }

        private void ApplySchemaAttribute(OpenApiSchema schema, SwaggerSchemaMinimumAndMaximumAttribute schemaAttribute)
        {
            //if (schemaAttribute.minimum != null) schema.MinLength = schemaAttribute.minimum;
            //if (schemaAttribute.maximum != null) schema.MaxLength = schemaAttribute.maximum;
            schema.MinLength = schemaAttribute.minimum;
            schema.MaxLength = schemaAttribute.maximum;
        }
    }
}
