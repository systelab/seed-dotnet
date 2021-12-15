namespace Main.Services
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Main.Contracts;
    using Main.Entities.Models;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class MailService : IMailService
    {
        private readonly IConfiguration config;

        private readonly ILogger<Email> logger;

        public MailService(IConfiguration config, ILogger<Email> logger)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string GetEmailTest()
        {
            StringBuilder cadena = new StringBuilder();
            cadena.Append(this.GetHeader("Test of Send Email"));
            cadena.Append(
                "<table cellpadding=\"10\" style=\"min-height:150px;width: 100%;margin-left: auto;margin-right: auto;margin-top:20px;\" role=\"presentation\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            cadena.Append("<tr style=\"width:100%\"><td style=\"padding: 10px;width:45%;\"></td><td style=\"padding: 10px;width:55%;\"></td></tr>");
            cadena.Append("<tr style=\"width:100%\">");
            cadena.Append("<td style=\"padding: 10px;text-align:right;width:50%;\">");
            cadena.Append("<p>");
            cadena.Append("<b style=\"font-size:10pt;font-family:Helvetica,sans-serif;\">Hello AccuTrak 2.0</b>");
            cadena.Append("</p>");
            cadena.Append("</td>");
            cadena.Append("</tr>");
            cadena.Append("<tr style=\"width:100%\"><td style=\"padding: 10px;width:45%;\"></td><td style=\"padding: 10px;width:55%;\"></td></tr>");
            cadena.Append("</table>");

            cadena.Append(this.GetFooter());
            return cadena.ToString();
        }

        /// <summary>
        /// Send the email
        /// </summary>
        /// <param name="emailConfig"></param>
        /// <returns></returns>
        public async Task SendEmail(Email emailConfig)
        {
            try
            {
                string SMTP_USERNAME = this.config["MailSettings:smtpUsername"];
                string SMTP_PASSWORD = this.config["MailSettings:smtpPassword"];

                using (SmtpClient client = new SmtpClient())
                {
                    client.Host = this.config["MailSettings:host"];
                    client.Port = int.Parse(this.config["MailSettings:port"]);
                    using (MailMessage emailMessage = new MailMessage())
                    {
                        emailMessage.To.Add(new MailAddress(emailConfig.emailTo));
                        emailMessage.From = new MailAddress(this.config["MailSettings:fromAddress"], this.config["MailSettings:fromName"]);
                        emailMessage.Subject = emailConfig.subject;
                        emailMessage.IsBodyHtml = true;
                        emailMessage.BodyEncoding = Encoding.GetEncoding("utf-8");
                        emailMessage.SubjectEncoding = Encoding.GetEncoding("utf-8");
                        AlternateView plainView = AlternateView.CreateAlternateViewFromString(Regex.Replace(emailConfig.body, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(emailConfig.body, null, "text/html");
                        emailMessage.AlternateViews.Add(plainView);
                        emailMessage.AlternateViews.Add(htmlView);

                        try
                        {
                            if (!string.IsNullOrEmpty(SMTP_USERNAME) && !string.IsNullOrEmpty(SMTP_PASSWORD))
                            {
                                client.Credentials = new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
                                client.EnableSsl = true;
                            }

                            client.Send(emailMessage);
                        }
                        catch (SmtpFailedRecipientsException ex)
                        {
                            for (int i = 0; i < ex.InnerExceptions.Length; i++)
                            {
                                SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;

                                if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
                                {
                                    Thread.Sleep(5000);
                                    client.Send(emailMessage);
                                }
                                else
                                {
                                    //Add to blacklist emails
                                    this.logger.LogWarning("Add the email to the black list");
                                }
                            }
                        }
                        catch (SmtpException ex)
                        {
                            //Add to blacklist emails
                            this.logger.LogWarning("Add the email to the black list");
                        }
                        catch (Exception ex)
                        {
                            //Add error log
                            this.logger.LogError($"Failed sending email: {ex}");
                        }
                    }

                    await Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                //Add error log
                this.logger.LogError($"Failed sending email: {ex}");
            }
        }

        private string GetFooter()
        {
            StringBuilder cadena = new StringBuilder();
            cadena.Append("</div>");

            cadena.Append("<table cellpadding=\"5\" style=\"width:100%;margin-top:25px;\" role=\"presentation\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");

            cadena.Append("<tr style=\"width:100%\">");
            cadena.Append("<td  style=\"width:100%;text-align:center;\">");
            cadena.Append("<p style =\"font-size:11pt;font-family:Helvetica,sans-serif;text-align:center;margin:0;\">");
            cadena.Append("<span style=\"font-size:11pt;font-family:Helvetica,sans-serif;\"></span>");
            cadena.Append("</p>");
            cadena.Append("</td>");
            cadena.Append("</tr>");
            cadena.Append("<tr style=\"width:100%\">");
            cadena.Append("<td  style=\"width:100%;text-align:center;\">");
            cadena.Append("<p style =\"font-size:11pt;font-family:Helvetica,sans-serif;text-align:center;margin:0;\">");
            cadena.Append("<a style=\"font-size:11pt;font-family:Helvetica,sans-serif;\" href=\"https://google.com\">Google</a>");
            cadena.Append("</p>");
            cadena.Append("</td>");
            cadena.Append("</tr>");

            cadena.Append("</table>");
            cadena.Append("</body></html>");
            return cadena.ToString();
        }

        private string GetHeader(string title)
        {
            StringBuilder cadena = new StringBuilder();
            cadena.Append("<html><head>");

            cadena.Append(
                "<style>html,body {margin: 0 auto!important;padding: 0!important;height: 100% !important;width: 100% !important;} * {-ms-text-size-adjust: 100 %;} table,td {mso-table-lspace: 0pt!important; mso-table-rspace: 0pt!important;} img {-ms-interpolation-mode:bicubic;} a {text-decoration: none;}</style>");
            cadena.Append("</head><body width=\"100%\" style=\"margin: 0; padding: 0 !important; mso-line-height-rule: exactly;\">");
            cadena.Append("<table cellpadding=\"10\" style=\"width:100%\" role=\"presentation\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            cadena.Append("<tr style=\"width:100%;background-color: #2379BF;border-radius: 3px; padding: 15px;border-style: none;align-items: center;\">");
            cadena.Append("<td colspan=\"3\" style=\"width:100%\">");
            cadena.Append("<p style =\"font-size:11pt;font-family:Helvetica,sans-serif;text-align:center;margin:0;\">");
            cadena.Append("<span style=\"color:white;font-size:13.5pt;font-family:Helvetica,sans-serif;\">" + title + "</span>");
            cadena.Append("</p>");
            cadena.Append("</td>");
            cadena.Append("</tr>");
            cadena.Append("</table>");
            cadena.Append("<div>");
            return cadena.ToString();
        }
    }
}