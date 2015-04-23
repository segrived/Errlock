using System;
using System.Collections.Generic;
using Errlock.Lib.Modules;

namespace Errlock.Lib.LogFile
{
    public class LogFile
    {
        public Guid SessionId { get; set; }
        public List<string> LogMessages { get; set; }
        public List<ModuleNotice> Notices { get; set; }
    }
}