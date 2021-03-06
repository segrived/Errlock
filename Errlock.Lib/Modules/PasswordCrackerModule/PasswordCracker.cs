﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules.PasswordCrackerModule.Notices;
using Errlock.Lib.RequestWrapper;
using Errlock.Lib.Sessions;
using Errlock.Resources.ModulesData;

namespace Errlock.Lib.Modules.PasswordCrackerModule
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

        private static readonly PasswordCrackerConfig DefaultConfig = new PasswordCrackerConfig {
            Login = @"admin",
            RequestUrl = @"login",
            RequestParameters = @"login={{login}}&password={{password}}",
            RequestMethod = RequestMethod.Post,
            InvalidPasswordAction = InvalidPasswordAction.Render403,
            PasswordsCount = 100,
            StopAfterFirstMatch = true,
            UseHeuristic = true
        };

        public PasswordCracker(ConnectionConfiguration connectionConfig)
            : base(DefaultConfig, connectionConfig) { }

        public PasswordCracker(
            PasswordCrackerConfig moduleConfig, ConnectionConfiguration connectionConfig)
            : base(moduleConfig, connectionConfig) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ProcessParameters(string password)
        {
            return this.Config.RequestParameters
                       .Replace("{{login}}", this.Config.Login)
                       .Replace("{{password}}", password);
        }

        private int _processedCount;
        private int _totalCount;

        private static List<string> GetHeuristicVariations(string login)
        {
            var def = new List<string> {
                login,
                login.ToUpper(), login.ToLower(),
                login.ToLower().ReverseStr(), login.ToUpper().ReverseStr(),
                login.ToUpperFirstChar(),
            };
            for (int i = 2; i < 5; i++) {
                string loginRepeated = String.Concat(Enumerable.Repeat(login, i));
                def.Add(loginRepeated);
            }
            return def.Concat(def.Select(Extensions.LatinToCyrillic)).ToList();
        }

        protected override ModuleScanStatus Process(Session session, IProgress<int> progress)
        {
            float currentProgress = 0;

            var sessionUri = new Uri(session.Url);  

            this._totalCount = this.Config.PasswordsCount;
            IEnumerable<string> passwordsSource = PasswordListCache
                .Value
                .Take(this.Config.PasswordsCount);

            if (this.Config.UseHeuristic) {
                var variations = GetHeuristicVariations(this.Config.Login);
                this._totalCount += variations.Count;
                passwordsSource = variations.Concat(passwordsSource);
            }

            foreach (string password in passwordsSource) {
                if (this.Token.IsCancellationRequested) {
                    return ModuleScanStatus.Canceled;
                }

                string parameters = ProcessParameters(password);

                var uri = new Uri(sessionUri, this.Config.RequestUrl);
                
                Func<WebRequestWrapper, HttpWebResponse> requestAction;
                if (Config.RequestMethod == RequestMethod.Get) {
                    uri = new UriBuilder(uri) { Query = parameters }.Uri;
                    requestAction = p => p.GetRequest();
                } else {
                    requestAction = p => p.PostRequest(parameters);
                }
                var parser = new WebRequestWrapper(this.ConnectionConfiguration, uri.AbsoluteUri);
                using (var response = requestAction.Invoke(parser)) {
                    string message = String.Format("Тест пароля `{0}`", password);
                    AddMessage(message, LoggerMessageType.Info);

                    int percentProgress = (int)((float)_processedCount / _totalCount * 100.0);
                    if (percentProgress > currentProgress) {
                        progress.Report(percentProgress);
                        currentProgress = percentProgress;
                    }

                    this._processedCount++;
                    if (this.Config.InvalidPasswordAction == InvalidPasswordAction.Render403 &&
                        response.StatusCode == HttpStatusCode.Forbidden) {
                        continue;
                    }
                    if (this.Config.InvalidPasswordAction == InvalidPasswordAction.RedirectBack &&
                        response.ResponseUri == uri) {
                        continue;
                    }
                    var notice = new PasswordMatchNotice(uri.AbsoluteUri, this.Config.Login,
                        password);
                    AddNotice(notice);

                    const string msg = "Ни один из триггеров не сработал, возможно найден пароль. " +
                                       "Пароль: `{0}` подошел";
                    string successMessage = String.Format(msg, password);
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