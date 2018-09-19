using System;
using Microsoft.AspNetCore.Mvc;

namespace main.Models
{
    /// <summary>
    /// Specifies the content types accepted for the annoted method 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal class SwaggerConsumesAttribute : ConsumesAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerConsumesAttribute"/> class
        /// </summary>
        /// <param name="contentType">Primary content type</param>
        /// <param name="otherContentTypes">Other content types</param>
        public SwaggerConsumesAttribute(string contentType, params string[] otherContentTypes)
            : base(contentType, otherContentTypes)
        {
        }
    }
}
