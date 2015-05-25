using System;

namespace Errlock.Lib.Logger
{
    public class LoggerEventArgs : EventArgs
    {
        public string FormattedMessage { get; private set; }
        public LoggerMessageType MessageType { get; private set; }

        public LoggerEventArgs(string formattedMessage, LoggerMessageType messageType)
        {
            this.FormattedMessage = formattedMessage;
            this.MessageType = messageType;
        }
    }
}