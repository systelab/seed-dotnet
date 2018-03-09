
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace main.Models
{
    public class SwaggerConsumesOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Apply the filter
        /// </summary>
        /// <param name="operation">Swagger description</param>
        /// <param name="context">Context of the operation filter</param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var action = context?.ApiDescription?.ActionDescriptor as ControllerActionDescriptor;
            var attribute = action?.MethodInfo.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(SwaggerConsumesAttribute));

            if (attribute != null)
            {
                var primaryContentType = attribute.ConstructorArguments[0].Value as string;
                var secondaryContentTypes = (attribute.ConstructorArguments[1].Value as IReadOnlyList<CustomAttributeTypedArgument>).Select(s => s.Value.ToString()).ToArray();

                var contentTypes = new List<string>() { primaryContentType };
                contentTypes.AddRange(secondaryContentTypes);

                operation.Consumes = contentTypes;
            }
        }
    }
}
