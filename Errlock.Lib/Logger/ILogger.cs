namespace Errlock.Lib.Logger
{
    public interface ILogger
    {
        void Log(string message, LoggerMessageType messsageType);
    }
}