using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CloudFlareDDNS
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        public async Task MainAsync()
        {
            Config config = ConfigReader.Read();
            if (config == null)
            {
                Console.WriteLine("[!] config.json not found. A default one has been generated.");
                Console.WriteLine("[!] Please edit your config.json and run the service again.");

                Environment.Exit(0);
            }

            Logger logger = new Logger(config.logToFile, config.coloredLogging);
            CloudFlareAPI api = new CloudFlareAPI(config.apiEndpoint, config.email, config.apiKey, logger);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(
                " .--..             . .---..              .--. .--. .   . .-. \n" +
                ":    |             | |    |              |   :|   :|\\  |(   )\n" +
                "|    | .-. .  . .-.| |--- | .-.  .--..-. |   ||   || \\ | `-. \n" +
                ":    |(   )|  |(   | |    |(   ) |  (.-' |   ;|   ;|  \\|(   )\n" +
                " `--'`-`-' `--`-`-'`-'    `-`-'`-'   `--''--' '--' '   ' `-' \n" +
                ".--.       .   .              .   .                          \n" +
                "|   )      |  /   o           |  /                           \n" +
                "|--: .  .  |-'    .  .--..-.  |-'  .  . .--.                 \n" +
                "|   )|  |  |  \\   |  |  (   ) |  \\ |  | |  |                 \n" +
                "'--' `--|  '   `-' `-'   `-'`-'   ``--`-'  `-                \n" +
                "        ;                                                    \n" +
                "     `-'                                                     \n");
            Console.ResetColor();

            logger.Info($"Cooldown: {config.cooldown} seconds");
            logger.Info($"Records ({config.records.Count}): {string.Join(", ", config.records)}");

            while (true)
            {
                List<DNSRecord> records = await api.GetRecords(config.zoneId);
                if (records != null)
                {
                    foreach (DNSRecord record in records)
                    {
                        if (config.records.Contains(record.name))
                        {
                            string ip = new WebClient().DownloadString("https://ipv4.icanhazip.com/").Trim();
                            if (record.content != ip)
                            {
                                await api.UpdateRecord(config.zoneId, record.id, record.name, ip, record.ttl, record.proxied);
                                logger.Info($"Updated {record.name} | {record.content} -> {ip} | TTL: {record.ttl} | Proxied: {record.proxied}");
                            }
                        }
                    }
                }

                Thread.Sleep(TimeSpan.FromSeconds(config.cooldown));
            }
        }
    }
}
