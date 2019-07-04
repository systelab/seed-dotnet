﻿namespace main.Entities.Models
{
    using Common;

    public class Address : BaseEntity
    {
        public string City { get; set; }

        public string Coordinates { get; set; }

        public string Street { get; set; }

        public string Zip { get; set; }
    }
}