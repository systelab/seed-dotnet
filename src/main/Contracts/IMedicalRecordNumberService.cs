namespace main.Contracts
{
    /// <summary>
    /// Service for retrieving a MRN
    /// </summary>
    public interface IMedicalRecordNumberService
    {
        /// <summary>
        /// Retrieves a MRN from an external service
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetMedicalRecordNumber(string url);
    }
}