using System;
using Errlock.Lib.Logger;

namespace ErrlockConsole
{
    public class ConsoleLogFormatter
    {
        public string FormatMessage(string message, LoggerMessageType messageType)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            string type = messageType.ToString();
            return String.Format("[{0}] {1}: {2}", time, type, message);
        }
    }
}