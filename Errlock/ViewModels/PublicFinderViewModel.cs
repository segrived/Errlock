using Errlock.Lib;
using Errlock.Lib.Modules.PublicFinder;

namespace Errlock.ViewModels
{
    public class PublicFinderViewModel : Bindable
    {
        public PublicFinderConfig Config { get; set; }

        public PublicFinderViewModel()
        {
            this.Config = new PublicFinderConfig {
                UsePermutations = true,
                DetectSuspicious = true,
                UseGetRequests = false,
                UserWordsList = ""
            };
        }
    }
}