using Errlock.Lib.Modules;

namespace Errlock.ViewModels
{
    public class BaseModuleViewModel<T, TU> where T : Module<TU> where TU : ModuleConfig
    {
        public Module<TU> Module { get; set; }
        public TU Config { get; set; }

        public BaseModuleViewModel(Module<TU> module)
        {
            this.Module = module;
            this.Config = this.Module.Config;
        }
    }
}
