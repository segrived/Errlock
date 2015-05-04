using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules.PasswordCracker.Notices;
using Errlock.Lib.Sessions;
using Errlock.Lib.SmartWebRequest;
using Errlock.Resources.ModulesData;

namespace Errlock.Lib.Modules.PasswordCracker
{
    public enum InvalidPasswordAction
    {
        [Description("Перенаправление обратно на страницу входа")]
        RedirectBack,

        [Description("Возврат 403 кода ошибки")]
        Render403
    }

    public class PasswordCracker : Module<PasswordCrackerConfig>
    {
        // Кэшируем для более высокой производительности
        private static readonly Lazy<List<string>> PasswordListCache = new Lazy<List<string>>(
            () => PasswordCrackerData.Passwords.Lines().ToList());

        public override bool IsSupportProgressReporting
        {
            get { return true; }
        }

        public PasswordCracker(
            PasswordCrackerConfig moduleConfig, ConnectionConfiguration connectionConfig)
            : base(moduleConfig, connectionConfig) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ProcessParameters(string password)
        {
            return this.ModuleConfig.RequestParameters
                       .Replace("{{login}}", this.ModuleConfig.Login)
                       .Replace("{{password}}", password);
        }

        private int _processedCount;
        private int _totalCount;

        private List<string> GetHeuristicVariations(string login)
        {
            return new List<string> {
                login,
                login.ToUpper(), login.ToLower(),
                login.ToLower().ReverseStr(), login.ToUpper().ReverseStr(),
                login.ToUpperFirstChar()
            };
        }

        protected override ModuleScanStatus Process(Session session, IProgress<int> progress)
        {
            var sessionUri = new Uri(session.Url);  

            this._totalCount = this.ModuleConfig.PasswordsCount;
            IEnumerable<string> passwordsSource = PasswordListCache.Value
                .Take(this.ModuleConfig.PasswordsCount);

            if (this.ModuleConfig.UseHeuristic) {
                var variations = this.GetHeuristicVariations(this.ModuleConfig.Login);
                this._totalCount += variations.Count;
                passwordsSource = variations.Concat(passwordsSource);
            }

            foreach (string password in passwordsSource) {
                if (this.Token.IsCancellationRequested) {
                    return ModuleScanStatus.Canceled;
                }

                string parameters = ProcessParameters(password);

                var uri = new Uri(sessionUri, this.ModuleConfig.RequestUrl);
                
                Func<SmartWebRequest.SmartWebRequest, HttpWebResponse> requestAction;
                if (ModuleConfig.RequestType == RequestType.Get) {
                    uri = new UriBuilder(uri) { Query = parameters }.Uri;
                    requestAction = p => p.GetRequest();
                } else {
                    requestAction = p => p.PostRequest(parameters);
                }
                var parser = new SmartWebRequest.SmartWebRequest(this.ConnectionConfiguration,
                    uri.AbsoluteUri);
                using (var response = requestAction.Invoke(parser)) {
                    string message = String.Format("Тест пароля `{0}`", password);
                    AddMessage(message, LoggerMessageType.Info);
                    progress.Report((int)((double)_processedCount / _totalCount * 100));
                    this._processedCount++;
                    if (this.ModuleConfig.InvalidPasswordAction == InvalidPasswordAction.Render403 &&
                        response.StatusCode == HttpStatusCode.Forbidden) {
                        continue;
                    }
                    if (this.ModuleConfig.InvalidPasswordAction == InvalidPasswordAction.RedirectBack &&
                        response.ResponseUri == uri) {
                        continue;
                    }
                    var notice = new PasswordMatchNotice(session, uri.AbsoluteUri, this.ModuleConfig.Login,
                        password);
                    AddNotice(notice);
                    string successMessage =
                        String.Format(
                            "Ни один из триггеров не сработал, возможно найден пароль. Пароль: `{0}` подошел",
                            password);
                    AddMessage(successMessage, LoggerMessageType.Info);
                    if (this.ModuleConfig.StopAfterFirstMatch) {
                        return ModuleScanStatus.Completed;
                    }
                }
            }
            return ModuleScanStatus.Completed;
        }
    }
}