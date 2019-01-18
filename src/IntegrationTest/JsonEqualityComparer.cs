namespace IntegrationTest
{
    using System.Collections.Generic;
    using main.Entities.ViewModels;
    using Newtonsoft.Json;

    public class JsonEqualityComparer : IEqualityComparer<PatientViewModel>
    {
        public bool Equals(PatientViewModel x, PatientViewModel y)
        {
            return JsonConvert.SerializeObject(x).Equals(JsonConvert.SerializeObject(y));
        }

        public int GetHashCode(PatientViewModel obj)
        {
            return JsonConvert.SerializeObject(obj).GetHashCode();
        }
    }
}