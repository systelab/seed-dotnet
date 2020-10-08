namespace Main.Entities
{
    using System;

    public class JsonWebToken
    {
        public string AccessToken { get; set; }

        public DateTime Expiration { get; set; }

        public long Expires { get; set; }

        public string RefreshToken { get; set; }
    }
}