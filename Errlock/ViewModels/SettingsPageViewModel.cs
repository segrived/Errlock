using System.Collections.Generic;
using Errlock.Lib.AppConfig;
using Errlock.Lib.RequestWrapper;

namespace Errlock.ViewModels
{
    public class SettingsPageViewModel
    {
        public ErrlockConfigModel ConfigModel { get; set; }
        public List<string> UserAgentsList { get; set; }

        public SettingsPageViewModel()
        {
            ConfigModel = App.Config.Model;
            this.UserAgentsList = WebRequestWrapper.DefaultUserAgentList;
        }
    }
}
