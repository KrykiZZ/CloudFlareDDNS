using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace CloudFlareDDNS
{
    public class Config
    {
        [JsonIgnore]
        public string Name { get; private set; }

        [JsonProperty("api_endpoint")]
        public readonly string ApiEndpoint;

        [JsonProperty("email")]
        public readonly string Email;

        [JsonProperty("api_key")]
        public readonly string ApiKey;

        [JsonProperty("zone_id")]
        public readonly string ZoneId;

        [JsonProperty("cooldown")]
        public readonly int Cooldown;

        [JsonProperty("log_to_file")]
        public readonly bool LogToFile;

        [JsonProperty("colored_logging")]
        public readonly bool ColoredLogging;

        [JsonProperty("records")]
        [DefaultValue(default(List<string>))]
        public readonly List<string> Records;

        public Config SetName(string newName)
        {
            Name = newName;
            return this;
        }
    }

    public class ConfigReader
    {
        public static List<Config> ReadAll()
        {
            if (!Directory.Exists("domains"))
            {
                Directory.CreateDirectory("domains");
                File.WriteAllText(Path.Combine("domains", "domain.com.json"), JsonConvert.SerializeObject(new Config(), Formatting.Indented));
                
                return null;
            }

            List<Config> domains = new List<Config>();
            foreach (string domain in Directory.GetFiles("domains"))
            {
                domains
                    .Add(JsonConvert.DeserializeObject<Config>(File.ReadAllText(domain))
                            .SetName(domain.Remove(domain.Length-5).Remove(0, 8)));
            }

            return domains;
        }
    }
}
