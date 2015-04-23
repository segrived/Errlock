using System;

namespace Errlock.Lib.Logger
{
    public class LoggerEventArgs : EventArgs
    {
        public string OriginalMessage { get; private set; }
        public string FormattedMessage { get; private set; }
        public LoggerMessageType MessageType { get; private set; }

        public LoggerEventArgs(
            string originalMessage, string formattedMessage,
            LoggerMessageType messageType)
        {
            this.OriginalMessage = originalMessage;
            this.FormattedMessage = formattedMessage;
            this.MessageType = messageType;
        }
    }
}