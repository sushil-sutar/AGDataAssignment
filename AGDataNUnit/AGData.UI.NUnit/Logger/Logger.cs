using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework
{
    public enum LogType
    {
        Info,
        Warning,
        Error
    }
    public static class Logger
    {
        private static string _logFilePath;
        private static StreamWriter _streamWriter;
        public static void InitializeLog()
        {
            _logFilePath = Directory.GetCurrentDirectory() + "\\Automation_Log.Log";
            if (!File.Exists(_logFilePath))
            {
                File.Create(_logFilePath);
            }
            _streamWriter = File.AppendText(_logFilePath);
        }

        public static void Log(LogType logType, string logMessage)
        {
            logMessage = DateTime.Now.ToString() + " " + LogType.Info + " " + logMessage;
            _streamWriter.WriteLine(logMessage);
        }
    }
}
