using System.Collections.Generic;
using Errlock.Lib.AppConfig;
using Errlock.Lib.SmartWebRequest;

namespace Errlock.ViewModels
{
    public class SettingsWindowViewModel
    {
        public ErrlockConfigModel ConfigModel { get; set; }
        public List<string> UserAgentsList { get; set; }

        public SettingsWindowViewModel()
        {
            ConfigModel = App.Config.Model;
            this.UserAgentsList = SmartWebRequest.UserAgentList;
        }
    }
}
