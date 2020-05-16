namespace main.Contracts
{
    using main.Entities.Common;
    using System.Threading.Tasks;

    public interface IMailService
    {
        /// <summary>
        /// Send the email configured
        /// </summary>
        /// <param name="emailConfig"></param>
        /// <returns></returns>
        Task SendEmail(Email emailConfig);

        /// <summary>
        /// Get the body of the email example
        /// </summary>
        /// <returns></returns>
        string GetEmailTest();
    }
}