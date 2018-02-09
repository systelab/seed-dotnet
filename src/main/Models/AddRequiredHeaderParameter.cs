namespace Main.Models
{
    using System.Collections.Generic;

    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    // This internal class is needed to authorize the swagger to send also the header parameters
    internal class AddRequiredHeaderParameter : IOperationFilter
    {
        void IOperationFilter.Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(
                new Parameter { Name = "Authorization", In = "header", Type = "string", Required = false });
        }
    }
}