namespace Main.Contracts
{
    using System.Threading.Tasks;

    using Main.Entities.Models;

    /// <summary>
    /// Service for managing emails
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Get the body of the email example
        /// </summary>
        /// <returns></returns>
        string GetEmailTest();

        /// <summary>
        /// Send the email configured
        /// </summary>
        /// <param name="emailConfig"></param>
        /// <returns></returns>
        Task SendEmail(Email emailConfig);
    }
}