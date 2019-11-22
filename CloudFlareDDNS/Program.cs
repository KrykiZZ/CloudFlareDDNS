using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CloudFlareDDNS
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        public async Task MainAsync()
        {
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
                "     `-'                                                     \n" +
                $"\nVersion: {Assembly.GetExecutingAssembly().GetName().Version.ToString()}\n");
            Console.ResetColor();

            List<Config> configs = ConfigReader.ReadAll();
            if (configs == null || configs.Count == 0)
            {
                Console.WriteLine("[!] Domain configs not found. A default one has been generated.");
                Console.WriteLine("[!] Please edit your domain.com.json and run the service again.");
                Console.WriteLine("[?] Note: You can name it to whatever you want (used only for logging).");

                Environment.Exit(0);
            }

            List<Task> tasks = new List<Task>();
            foreach (Config config in configs)
            {
                Logger logger = new Logger(config.Name, config.LogToFile, config.ColoredLogging);
                CloudFlareAPI api = new CloudFlareAPI(config.ApiEndpoint, config.Email, config.ApiKey, logger);

                tasks.Add(new CloudFlareDDNS(api, config, logger).Start());
            }

            await Task.WhenAll(tasks);
        }
    }
}
