using Errlock.Lib.Modules.ConfigurationTestModule;

namespace Errlock.ViewModels
{
    class ConfigurationTestViewModel : BaseModuleViewModel<ConfigurationTest, ConfigurationTestConfig>
    {
        public ConfigurationTestViewModel()
            : base(new ConfigurationTest(App.Config.Model.ConnectionConfiguration))
        { }
    }
}
