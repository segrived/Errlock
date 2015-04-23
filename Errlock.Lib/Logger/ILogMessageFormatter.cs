namespace Errlock.Lib.Logger
{
    public interface ILogMessageFormatter
    {
        string FormatMessage(string message, LoggerMessageType messageType);
    }
}