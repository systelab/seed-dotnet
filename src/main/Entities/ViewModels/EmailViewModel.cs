﻿namespace main.Entities.ViewModels
{
    /// <summary>
    ///     Model for someone's address
    /// </summary>
    public class EmailViewModel
    {
        /// <summary>
        ///     Gets or sets the email to send
        /// </summary>
        public string emailTo { get; set; }

        /// <summary>
        ///     Gets or sets the subject of the email
        /// </summary>
        public string subject { get; set; }
    }
}