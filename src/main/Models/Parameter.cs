namespace seed_dotnet.Models
{
    using System.Collections.Generic;

    using Swashbuckle.AspNetCore.Swagger;

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