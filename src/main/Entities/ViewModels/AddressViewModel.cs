namespace main.Entities.ViewModels
{
    using System;

    /// <summary>
    ///     Model for someone's address
    /// </summary>
    public class AddressViewModel
    {
        /// <summary>
        ///     Gets or sets the city name
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///     Gets or sets the Coordinates in TBD format
        /// </summary>
        public string Coordinates { get; set; }

        /// <summary>
        ///     Gets or sets the internal id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the street
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        ///     Gets or sets the ZIP code
        /// </summary>
        public string Zip { get; set; }
    }
}