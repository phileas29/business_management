using Newtonsoft.Json;

namespace AddressFinder.Models
{
    public class OpenStreetMapResult
    {
        [JsonProperty("lat")]
        public string Lat { get; set; }

        [JsonProperty("lon")]
        public string Lon { get; set; }

        [JsonProperty("display_name")]
        public string Display_Name { get; set; }

        [JsonProperty("address")]
        public OpenStreetMapAddress Address { get; set; }
    }

    public class OpenStreetMapAddress
    {
        [JsonProperty("town")]
        public string Town { get; set; }
    }
}
