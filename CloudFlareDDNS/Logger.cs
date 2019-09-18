using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CloudFlareDDNS
{
    public class Logger
    {
        private readonly bool logToFile;
        private readonly bool coloredLogging;
        public Logger(bool logToFile, bool coloredLogging)
        {
            this.logToFile = logToFile;
            this.coloredLogging = coloredLogging;
        }

        private void Log(string level, ConsoleColor color, string message)
        {
            message = $"[{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")}] [{level}] {message}";

            if (coloredLogging)
                Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();

            if (logToFile)
                File.AppendAllText("CloudFlareDDNS.log", message);
        }

        public void Info(string message) => Log("INFO", ConsoleColor.Green, message);
        public void Error(string message) => Log("ERROR", ConsoleColor.Red, message);
    }
}
