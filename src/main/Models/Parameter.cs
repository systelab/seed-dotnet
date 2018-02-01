using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace seed_dotnet.Models
{
    public class Parameter : IParameter
    {
        public string Description { get; set; }

        public Dictionary<string, object> Extensions { get; }

        public string In { get; set; }

        public string Name { get; set; }

        public bool Required { get; set; }

        public string Type { get; set; }
    }
}
