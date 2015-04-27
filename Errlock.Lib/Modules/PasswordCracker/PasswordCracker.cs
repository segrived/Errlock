using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules.PasswordCracker.Notices;
using Errlock.Lib.Sessions;
using Errlock.Lib.WebParser;
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

        public PasswordCracker(PasswordCrackerConfig config) : base(config) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ProcessParameters(string password)
        {
            return this.Config.RequestParameters
                       .Replace("{{login}}", this.Config.Login)
                       .Replace("{{password}}", password);
        }

        protected override ModuleScanStatus Process(Session session, IProgress<int> progress)
        {
            var options = new ParserOptions {
                MaxRedirections = 10,
                Timeout = 3000
            };
            var sessionUri = new Uri(session.Url);
            for (int i = 0; i < this.Config.PasswordsCount; i++) {
                if (this.Token.IsCancellationRequested) {
                    return ModuleScanStatus.Canceled;
                }
                string password = PasswordListCache.Value[i];
                string parameters = ProcessParameters(password);

                var uri = new Uri(sessionUri, this.Config.RequestUrl);
                
                Func<Parser, HttpWebResponse> requestAction;
                if (Config.RequestType == RequestType.Get) {
                    uri = new UriBuilder(uri) { Query = parameters }.Uri;
                    requestAction = p => p.GetRequest();
                } else {
                    requestAction = p => p.PostRequest(parameters);
                }
                var parser = new Parser(options, uri.AbsoluteUri);
                using (var response = requestAction.Invoke(parser)) {
                    string message = String.Format("Тест пароля `{0}`", password);
                    AddMessage(message, LoggerMessageType.Info);
                    progress.Report((int)((double)i / this.Config.PasswordsCount * 100));
                    if (this.Config.InvalidPasswordAction == InvalidPasswordAction.Render403 &&
                        response.StatusCode == HttpStatusCode.Forbidden) {
                        continue;
                    }
                    if (this.Config.InvalidPasswordAction == InvalidPasswordAction.RedirectBack &&
                        response.ResponseUri == uri) {
                        continue;
                    }
                    var notice = new PasswordMatchNotice(session, uri.AbsoluteUri, this.Config.Login,
                        password);
                    AddNotice(notice);
                    string successMessage =
                        String.Format(
                            "Ни один из триггеров не сработал, возможно найден пароль. Пароль: `{0}` подошел",
                            password);
                    AddMessage(successMessage, LoggerMessageType.Info);
                    if (this.Config.StopAfterFirstMatch) {
                        return ModuleScanStatus.Completed;
                    }
                }
            }
            return ModuleScanStatus.Completed;
        }
    }
}