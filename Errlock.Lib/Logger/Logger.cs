using System;

namespace Errlock.Lib.Logger
{
    public enum LoggerMessageType
    {
        Info,
        Warn,
        Error
    }

    public delegate string MessageFormatter(string input, LoggerMessageType type);

    public class Logger : ILogger
    {

        private readonly MessageFormatter _formatter;

        public Logger(MessageFormatter formatter)
        {
            this._formatter = formatter;
        }

        public void Log(string message, LoggerMessageType messageType)
        {
            OnNewMessage(message, messageType);
        }

        public event EventHandler<LoggerEventArgs> NewMessage;

        protected virtual void OnNewMessage(string message, LoggerMessageType messageType)
        {
            var handle = this.NewMessage;
            string formattedMessage = this._formatter.Invoke(message, messageType);
            if (handle != null) {
                handle(this, new LoggerEventArgs(message, formattedMessage, messageType));
            }
        }
    }
}