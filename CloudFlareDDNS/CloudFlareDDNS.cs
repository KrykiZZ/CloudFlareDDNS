using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CloudFlareDDNS
{
    public class CloudFlareDDNS
    {
        private readonly CloudFlareAPI api;
        private readonly Logger logger;
        private readonly Config config;

        public CloudFlareDDNS(CloudFlareAPI api, Config config, Logger logger)
        {
            this.api = api;
            this.logger = logger;
            this.config = config;

            logger.Info($"Cooldown: {config.Cooldown} seconds");
            logger.Info($"Records ({config.Records.Count}): {string.Join(", ", config.Records)}");
        }

        public async Task Start()
        {
            while (true)
            {
                List<DNSRecord> records = await api.GetRecords(config.ZoneId);
                if (records != null)
                {
                    foreach (DNSRecord record in records)
                    {
                        if (config.Records.Contains(record.name))
                        {
                            string ip = new WebClient().DownloadString("https://ipv4.icanhazip.com/").Trim();
                            if (record.content != ip)
                            {
                                await api.UpdateRecord(config.ZoneId, record.id, record.name, ip, record.ttl, record.proxied);
                                logger.Info($"Updated {record.name} | {record.content} -> {ip} | TTL: {record.ttl} | Proxied: {record.proxied}");
                            }
                        }
                    }
                }

                Thread.Sleep(TimeSpan.FromSeconds(config.Cooldown));
            }
        }
    }
}
