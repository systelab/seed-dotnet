namespace main.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using AutoMapper;

    using main.Contracts;
    using main.Entities.Common;
    using main.Entities.ViewModels;

    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiVersion("1")]
    [ApiController]
    [EnableCors("MyPolicy")]
    [Route("seed/v{version:apiVersion}/emails")]
    public class EmailController : Controller
    {
        private readonly IMailService emailService;

        private readonly ILogger<EmailController> logger;

        private readonly IMapper mapper;

        public EmailController(ILogger<EmailController> logger, IMapper mapper, IMailService emailService)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        ///     Send email example
        /// </summary>
        /// <param name="email">Email model</param>
        /// <returns>Boolean</returns>
        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailViewModel email)
        {
            Email newEmail = this.mapper.Map<Email>(email);
            newEmail.body = this.emailService.GetEmailTest();
            await this.emailService.SendEmail(newEmail);
            this.logger.LogDebug($"Email sent to {email.emailTo}");
            return this.Ok();
        }
    }
}