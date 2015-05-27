using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CsQuery;
using Errlock.Lib.Modules.ConfigurationTestModule.Notices;
using Errlock.Lib.RequestWrapper;
using Errlock.Lib.Sessions;

namespace Errlock.Lib.Modules.ConfigurationTestModule
{
    public class ConfigurationTest : Module<ConfigurationTestConfig>
    {
        private static readonly ConfigurationTestConfig DefaultConfig 
            = new ConfigurationTestConfig {
                CheckNonProductionServer = true,
                CheckSpecialHeaders = true,
                CheckTooManyScripts = true,
                CheckXXSSProtection = true
            };

        public override bool IsSupportProgressReporting
        {
            get { return false; }
        }

        protected override ModuleScanStatus Process(Session session, IProgress<int> progress)
        {
            string url = session.Url;
            var req = new WebRequestWrapper(this.ConnectionConfiguration, url);

            // Попытка обнаружения непредназначенного для продакшена сервера
            var nonProductionServerPatterns = new List<string> {
                "WEBrick/.*", "Python/.*" /* можно добавить и другие */
            };

            using (var res = req.GetRequest()) {

                // Информацию о используемых специальных заголовках
                if (this.Config.CheckSpecialHeaders) {
                    var specialHeaders = res.Headers.AllKeys.Where(k => k.StartsWith("X-")).ToList();
                    if (specialHeaders.Any()) {
                        var specHeadersNotice = new SpecialHeadersNotice(url, specialHeaders);
                        this.AddNotice(specHeadersNotice);
                    }
                }

                // Проверка на заголовок X-Xss-Protection 
                if (this.Config.CheckXXSSProtection) {
                    if (res.Headers["X-Xss-Protection"] == "0") {
                        var disabledXssNotice = new XssProtectionDisabled(url);
                        this.AddNotice(disabledXssNotice);
                    }
                }

                // Проверка на использовать сервера, не предназначенного для продакшена
                if (this.Config.CheckNonProductionServer) {
                    string server = res.Server;
                    bool isNonProd = nonProductionServerPatterns.Any(r => Regex.IsMatch(server, r));
                    if (isNonProd) {
                        var notice = new NonProductionServerNotice(url, server);
                        this.AddNotice(notice);
                    }
                }

                // Проверка на количетсво подключенных скриптов на странице
                if (this.Config.CheckTooManyScripts) {
                    var responseHtml = res.Download();
                    var dom = new CQ(responseHtml);
                    var externalScriptsCount = dom["script"]
                        .Count(e => e.GetAttribute("src") != null);
                    if (externalScriptsCount >= 5) {
                        var scriptNotice = new TooManyScriptsNotice(url, externalScriptsCount);
                        this.AddNotice(scriptNotice);
                    }
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
