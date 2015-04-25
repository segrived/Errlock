using Errlock.Lib;
using Errlock.Lib.Sessions;

namespace Errlock.ViewModels
{
    public class LogViewerViewModel : Bindable
    {
        public SessionLogFile LogData
        {
            get { return Get<SessionLogFile>(); } 
            set { Set(value); }
        }
    }
}
