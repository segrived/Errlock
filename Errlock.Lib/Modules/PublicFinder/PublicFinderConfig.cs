namespace Errlock.Lib.Modules.PublicFinder
{
    public class PublicFinderConfig : ModuleConfig
    {
        public bool UsePermutations { get; set; }
        public bool DetectSuspicious { get; set; }
        public bool UseGetRequests { get; set; }
        public string UserWordsList { get; set; }
    }
}