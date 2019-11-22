using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CloudFlareDDNS
{
    public class Logger
    {
        private readonly string name;
        private readonly bool logToFile;
        private readonly bool coloredLogging;

        public Logger(string name, bool logToFile, bool coloredLogging)
        {
            this.name = name;
            this.logToFile = logToFile;
            this.coloredLogging = coloredLogging;
        }

        private void Log(string level, ConsoleColor color, string message)
        {
            message = $"[{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")}] [{level}] [{name}] {message}";

            if (coloredLogging)
                Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();

            if (logToFile)
            {
                if (!Directory.Exists("logs")) Directory.CreateDirectory("logs");
                File.AppendAllText(Path.Join("logs", $"{name}.log"), message + "\n");
            }
        }

        public void Info(string message) => Log("INFO", ConsoleColor.Green, message);
        public void Error(string message) => Log("ERROR", ConsoleColor.Red, message);
    }
}
