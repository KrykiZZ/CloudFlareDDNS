using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CloudFlareDDNS
{
    public class DNSRecord
    {
        public string id;
        public string name;
        public string content;
        public int ttl;
        public bool proxied;

        public DNSRecord(string id, string name, string content, int ttl, bool proxied)
        {
            this.id = id;
            this.name = name;
            this.content = content;
            this.ttl = ttl;
            this.proxied = proxied;
        }
    }

    public class CloudFlareAPI
    {
        private readonly HttpClient httpClient;
        private readonly Logger logger;
        public CloudFlareAPI(string apiEndpoint, string email, string apiKey, Logger logger)
        {
            this.logger = logger;
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(120d);
            httpClient.BaseAddress = new Uri(apiEndpoint);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            httpClient.DefaultRequestHeaders.Add("X-Auth-Email", email);
            httpClient.DefaultRequestHeaders.Add("X-Auth-Key", apiKey);
        }

        public async Task<List<DNSRecord>> GetRecords(string zoneId)
        {
            string query = await new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("type", "A"),
                new KeyValuePair<string, string>("page", "1"),
                new KeyValuePair<string, string>("per_page", "100")
            }).ReadAsStringAsync();

            HttpResponseMessage responseMessage = await httpClient.GetAsync($"zones/{zoneId}/dns_records?" + query);
            string response = await responseMessage.Content.ReadAsStringAsync();

            JToken token = JToken.Parse(response);
            List<DNSRecord> records = new List<DNSRecord>();
            if ((bool)token.SelectToken("$.success"))
            {
                foreach (JToken record in token.SelectTokens("$.result.[*]"))
                    records.Add(new DNSRecord(
                                                (string)record["id"], 
                                                (string)record["name"], 
                                                (string)record["content"],
                                                (int)record["ttl"],
                                                (bool)record["proxied"]
                                             ));

                return records;
            }
            else
            {
                foreach (JToken error in token.SelectTokens("$.errors.[*]"))
                    logger.Error($"{error.SelectToken("$.code")}, {error.SelectToken("$.message")}");

                return null;
            }
        }

        public async Task<bool> UpdateRecord(string zoneId, string recordId, string newName, string newContent, int newTtl, bool newProxied)
        {
            var jsonContent = JsonConvert.SerializeObject(new
            {
                type = "A",
                name = newName,
                content = newContent,
                ttl = newTtl,
                proxied = newProxied
            });

            HttpResponseMessage responseMessage = await httpClient.PutAsync($"zones/{zoneId}/dns_records/{recordId}", new StringContent(jsonContent, Encoding.UTF8, "application/json"));
            string response = await responseMessage.Content.ReadAsStringAsync();

            JToken token = JToken.Parse(response);
            if (!(bool)token.SelectToken("$.success"))
            {
                foreach (JToken error in token.SelectTokens("$.errors.[*]"))
                    logger.Error($"{error.SelectToken("$.code")}, {error.SelectToken("$.message")}");

                return false;
            }

            return true;
        }
    }
}
