namespace Main.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.OpenApi.Models;

    using Swashbuckle.AspNetCore.SwaggerGen;

    internal class SwaggerConsumesOperationFilter : IOperationFilter
    {
        /// <summary>
        ///     Apply the filter
        /// </summary>
        /// <param name="operation">Swagger description</param>
        /// <param name="context">Context of the operation filter</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            ControllerActionDescriptor action = context?.ApiDescription?.ActionDescriptor as ControllerActionDescriptor;
            CustomAttributeData attribute = action?.MethodInfo.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(SwaggerConsumesAttribute));

            if (attribute != null)
            {
                string primaryContentType = attribute.ConstructorArguments[0].Value as string;
                string[] secondaryContentTypes = (attribute.ConstructorArguments[1].Value as IReadOnlyList<CustomAttributeTypedArgument>).Select(s => s.Value.ToString()).ToArray();

                List<string> contentTypes = new List<string> { primaryContentType };
                contentTypes.AddRange(secondaryContentTypes);
            }
        }
    }
}