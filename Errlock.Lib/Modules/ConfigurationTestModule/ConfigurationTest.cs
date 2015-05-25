using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Errlock.Lib.Modules.ConfigurationTestModule.Notices;
using Errlock.Lib.Sessions;
using Errlock.Lib.SmartWebRequest;

namespace Errlock.Lib.Modules.ConfigurationTestModule
{
    public class ConfigurationTest : Module<ConfigurationTestConfig>
    {
        private static readonly ConfigurationTestConfig DefaultConfig 
            = new ConfigurationTestConfig { };

        public override bool IsSupportProgressReporting
        {
            get { return false; }
        }

        protected override ModuleScanStatus Process(Session session, IProgress<int> progress)
        {
            var url = session.Url;
            var req = new SmartRequest(this.ConnectionConfiguration, url);

            // Попытка обнаружения непредназначенного для продакшена сервера
            var nonProductionServerPatterns = new List<string> {
                "WEBrick/.*", "Python/.*" /* можно добавить и другие */
            };

            using (var res = req.HeadRequest()) {
                string server = res.Server;
                bool isNonProd = nonProductionServerPatterns.Any(r => Regex.IsMatch(server, r));
                if (isNonProd) {
                    var notice = new NonProductionServerNotice(session, url, server);
                    AddNotice(notice);
                }
            }

            return ModuleScanStatus.Completed;
        }

        public ConfigurationTest(ConfigurationTestConfig moduleConfig, ConnectionConfiguration connectionConfig) 
            : base(moduleConfig, connectionConfig) { }

        public ConfigurationTest(ConnectionConfiguration connectionConfig)
            : base(DefaultConfig, connectionConfig) { }

    }
}
