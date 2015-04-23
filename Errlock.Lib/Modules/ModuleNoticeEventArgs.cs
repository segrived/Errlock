using System;

namespace Errlock.Lib.Modules
{
    public class ModuleNoticeEventArgs : EventArgs
    {
        public ModuleNotice Notice { get; private set; }

        public ModuleNoticeEventArgs(ModuleNotice notice)
        {
            this.Notice = notice;
        }
    }
}