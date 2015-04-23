using Errlock.Lib;
using Errlock.Lib.Modules.PublicFinder;

namespace Errlock.ViewModels
{
    public class PublicFinderViewModel : Bindable
    {
        public bool InProcess
        {
            get { return Get<bool>(); }
            set { Set(value); }
        }

        public PublicFinderConfig Config { get; set; }

        public PublicFinderViewModel()
        {
            this.InProcess = false;
            this.Config = new PublicFinderConfig {
                UsePermutations = true,
                DetectSuspicious = true,
                UseGetRequests = false
            };
        }
    }
}