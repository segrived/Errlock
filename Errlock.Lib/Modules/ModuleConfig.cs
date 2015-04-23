using Errlock.Lib.Logger;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules
{
    public abstract class ModuleConfig
    {
        private Session Session { get; set; }
        private ILogger Logger { get; set; }
    }
}