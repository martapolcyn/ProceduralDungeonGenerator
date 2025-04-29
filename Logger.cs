using System;
using System.IO;

namespace ProceduralDungeonGenerator
{
    public static class Logger
    {
        private static readonly string logFilePath = "log.txt";

        static Logger()
        {
            // Clear existing log file
            File.WriteAllText(logFilePath, $"=== Log Start: {DateTime.Now} ==={Environment.NewLine}");
        }

        public static void Log(string message)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
    }
}
