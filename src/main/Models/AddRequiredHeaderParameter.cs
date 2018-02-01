using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace seed_dotnet.Models
{
    //This internal class is needed to authorize the swagger to send also the header parameters
    internal class AddRequiredHeaderParameter : IOperationFilter
    {
        void IOperationFilter.Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new Parameter
            {
                Name = "Authorization",
                @In = "header",
                Type = "string",
                Required = false
            });
        }
    }
}
