using System;
using Errlock.Lib.Logger;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules
{
    public interface IModule
    {
        void SetLogger(ILogger logger);
        ModuleScanResult Start(Session session);
        void Stop();
        event EventHandler<ModuleNoticeEventArgs> NewNotice;
        event EventHandler Started;
        event EventHandler<ModuleScanResultEventArgs> Completed;
    }
}