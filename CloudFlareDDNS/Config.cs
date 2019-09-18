using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace CloudFlareDDNS
{
    public class Config
    {
        [JsonProperty("api_endpoint")]
        public readonly string apiEndpoint = "https://api.cloudflare.com/client/v4/";

        [JsonProperty("email")]
        public readonly string email = "admin@example.com";

        [JsonProperty("api_key")]
        public readonly string apiKey = "m3m3";

        [JsonProperty("zone_id")]
        public readonly string zoneId = "an0th3rm3m3";

        [JsonProperty("cooldown")]
        public readonly int cooldown = 30;

        [JsonProperty("log_to_file")]
        public readonly bool logToFile = true;

        [JsonProperty("colored_logging")]
        public readonly bool coloredLogging = true;

        [JsonProperty("records")]
        public readonly List<string> records = new List<string>() { "example.com" };
    }

    public class ConfigReader
    {
        public static Config Read()
        {
            if (!File.Exists("config.json"))
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new Config(), Formatting.Indented));
                return null;
            }

            return JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
        }
    }
}
