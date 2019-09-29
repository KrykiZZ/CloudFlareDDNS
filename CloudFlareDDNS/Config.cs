using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace CloudFlareDDNS
{
    public class Config
    {
        [JsonProperty("api_endpoint")]
        public readonly string apiEndpoint;

        [JsonProperty("email")]
        public readonly string email;

        [JsonProperty("api_key")]
        public readonly string apiKey;

        [JsonProperty("zone_id")]
        public readonly string zoneId;

        [JsonProperty("cooldown")]
        public readonly int cooldown;

        [JsonProperty("log_to_file")]
        public readonly bool logToFile;

        [JsonProperty("colored_logging")]
        public readonly bool coloredLogging;

        [JsonProperty("records")]
        [DefaultValue(default(List<string>))]
        public readonly List<string> records;
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
