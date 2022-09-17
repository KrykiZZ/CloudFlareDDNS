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
        private readonly HttpClient _http;
        private readonly Logger _logger;

        private readonly string _apiEndpoint;
        private readonly string _email;
        private readonly string _apiKey;

        public CloudFlareAPI(HttpClient http, string apiEndpoint, string email, string apiKey, Logger logger)
        {
            _http = http;
            _logger = logger;

            _apiEndpoint = apiEndpoint;
            _email = email;
            _apiKey = apiKey;
        }

        public async Task<List<DNSRecord>> GetRecords(string zoneId)
        {
            string query = await new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("type", "A"),
                new KeyValuePair<string, string>("page", "1"),
                new KeyValuePair<string, string>("per_page", "100")
            }).ReadAsStringAsync();

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_apiEndpoint}zones/{zoneId}/dns_records?" + query)
            };

            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            message.Headers.Add("X-Auth-Email", _email);
            message.Headers.Add("X-Auth-Key", _apiKey);

            HttpResponseMessage responseMessage = await _http.SendAsync(message);
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
                    _logger.Error($"{error.SelectToken("$.code")}, {error.SelectToken("$.message")}");

                return null;
            }
        }

        public async Task<bool> UpdateRecord(string zoneId, string recordId, string newName, string newContent, int newTtl, bool newProxied)
        {
            string jsonContent = JsonConvert.SerializeObject(new
            {
                type = "A",
                name = newName,
                content = newContent,
                ttl = newTtl,
                proxied = newProxied
            });

            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{_apiEndpoint}zones/{zoneId}/dns_records/{recordId}"),
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            message.Headers.Add("X-Auth-Email", _email);
            message.Headers.Add("X-Auth-Key", _apiKey);

            HttpResponseMessage responseMessage = await _http.SendAsync(message);
            string response = await responseMessage.Content.ReadAsStringAsync();

            JToken token = JToken.Parse(response);
            if (!(bool)token.SelectToken("$.success"))
            {
                foreach (JToken error in token.SelectTokens("$.errors.[*]"))
                    _logger.Error($"{error.SelectToken("$.code")}, {error.SelectToken("$.message")}");

                return false;
            }

            return true;
        }
    }
}
