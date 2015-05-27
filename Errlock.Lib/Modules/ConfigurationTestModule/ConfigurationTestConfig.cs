namespace Errlock.Lib.Modules.ConfigurationTestModule
{
    public class ConfigurationTestConfig : ModuleConfig
    {
        public bool CheckNonProductionServer { get; set; }
        public bool CheckSpecialHeaders { get; set; }
        public bool CheckXXSSProtection { get; set; }
        public bool CheckTooManyScripts { get; set; }
    }
}