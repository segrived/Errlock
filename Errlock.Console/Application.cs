﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Errlock.Lib;
using Errlock.Lib.AppConfig;
using Errlock.Lib.Helpers;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules;
using Errlock.Lib.Modules.ConfigurationTestModule;
using Errlock.Lib.Modules.PasswordCrackerModule;
using Errlock.Lib.Modules.PublicFinderModule;
using Errlock.Lib.Modules.XssScannerModule;
using Errlock.Lib.RequestWrapper;
using Errlock.Lib.Sessions;

namespace Errlock.Console
{
    /// <summary>
    /// Логгер для консольного приложения
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Log(string message, LoggerMessageType messsageType)
        {
            System.Console.WriteLine("[{0}] {1}", messsageType, message);
        }
    }

    public class Application
    {
        /// <summary>
        /// Конфигурация приложения
        /// </summary>
        private static readonly AppConfig ErrlockConfig = 
            new AppConfig(ErrlockConfigModel.Defaults);

        /// <summary>
        /// Консольный логгер
        /// </summary>
        private readonly ILogger _consoleLogger = new ConsoleLogger();

        private readonly IRepository<Session> _repository = 
            new SessionLiteDbRepository();

        /// <summary>
        /// Настройки подключения
        /// </summary>
        private readonly ConnectionConfiguration ConnectionConfig =
            ErrlockConfig.Model.ConnectionConfiguration;

        /// <summary>
        /// Запускает консольное приложение
        /// </summary>
        public static void Run()
        {
            var app = new Application();
            app.MainLoop();
        }

        /// <summary>
        /// Запрашивает информацию о сессии и добавляет её в репозиторий
        /// </summary>
        /// <returns>Добавленная сессия</returns>
        private void AddSession() {
            var s = new Session();
            var urlReq = new ConsoleRequester<string>();
            urlReq.AddPredicate(u => !WebHelpers.IsValidUrl(u), "Неверный URL: ");
            s.Url = urlReq
                .RequestValue("Введите URL (включая протокол, например http://google.ru): ");
            s.Options.RecursionDepth = ConsoleRequester
                .RequestInt("Введите макс. глубину рекурсии: ");
            s.Options.FetchPerPage = ConsoleRequester
                .RequestInt("Максимальное количество ссылок с одной страницы: ");
            s.Options.IngoreAnchors = ConsoleRequester
                .RequestBool("Игнорировать якоря");
            s.Options.UseRandomLinks = ConsoleRequester
                .RequestBool("Собирать случайные ссылки: ");
            s.Options.MaxLinks = ConsoleRequester
                .RequestInt("Глобальное ограничение на количество ссылок: ");
            this._repository.InsertOrUpdate(s);
        }

        private XssScanner CreateXssScannerInstance()
        {
            var config = new XssScannerConfig();
            return new XssScanner(config, ConnectionConfig);
        }

        private PublicFinder CreatePublicFinderInstance()
        {
            var config = new PublicFinderConfig {
                DetectSuspicious = ConsoleRequester
                    .RequestBool("Обнаруживать подозрительные страинцы: "),
                UseGetRequests = ConsoleRequester
                    .RequestBool("Использовать GET-запросы вместо HEAD: "),
                UsePermutations = ConsoleRequester
                    .RequestBool("Использовать перестановки: ")
            };
            return new PublicFinder(config, ConnectionConfig);
        }

        private PasswordCracker CreatePasswordCrackerInstance()
        {  
            var config = new PasswordCrackerConfig {
                Login = ConsoleRequester
                    .RequestString("Логин: "),
                RequestParameters = ConsoleRequester
                    .RequestString("Параметры запроса: "),
                RequestUrl = ConsoleRequester
                    .RequestString("URL запроса (без адреса ресурса): "),
                PasswordsCount = ConsoleRequester
                    .RequestInt("Количество паролей: "),
                UseHeuristic = ConsoleRequester
                    .RequestBool("Использовать эвристику: "),
                StopAfterFirstMatch = ConsoleRequester
                    .RequestBool("Останавливать после первого совпадения: ")
            };
            return new PasswordCracker(config, ConnectionConfig);
        }

        private ConfigurationTest CreateConfigurationTestInstance()
        {
            var config = new ConfigurationTestConfig();
            return new ConfigurationTest(config, ConnectionConfig);
        }

        /// <summary>
        /// Выводит список сессий на экран и возвращает выбранную сессию
        /// </summary>
        /// <returns></returns>
        private Session GetSession()
        {
            var sessionList = this._repository.EnumerateAll();
            const string title = "Cессия, для которой необходимо запустить модуль";
            return ConsoleRequester.RequestListItem(sessionList, s => s.Url, title);
        }

        private IModule GetModule()
        {
            var modList = new Dictionary<string, Func<IModule>> {
                    { "Поиск доступных директории", CreatePublicFinderInstance },
                    { "Подбор пароля к ресурсу", CreatePasswordCrackerInstance },
                    { "Поиск XSS-уязвимостей", CreateXssScannerInstance },
                    { "Проверка конфигурации", CreateConfigurationTestInstance }
            };
            const string title = "Необходимый модуль";
            var item = ConsoleRequester.RequestListItem(modList, i => i.Key, title);
            return item.Value.Invoke();
            
        }

        /// <summary>
        /// Подготавливает сообщение у уязвимости для вывода на экран
        /// </summary>
        /// <param name="source">Экземпля класса ModuleNotice</param>
        /// <returns>Строка, содержащая сообщение о уязвимости</returns>
        private string PrepareNoticeMessage(ModuleNotice source)
        {
            var sb = new StringBuilder();
            sb.AppendFormatLine("Важность: {0}", source.Priority.GetDescription());
            sb.AppendFormatLine("Сообщение: {0}", source.Text);
            sb.AppendFormatLine("URL: {0}", source.LinkedUrl);
            return sb.ToString();
        }

        private void StartModule(IModule module, Session session)
        {
            module.SetLogger(this._consoleLogger);
            module.Completed += (sender, args) => {
                var result = args.ScanResult;
                foreach (var notice in result.Notices) {
                    var message = PrepareNoticeMessage(notice);
                    ConsoleHelpers.WriteColorLine(message, ConsoleColor.DarkRed);
                }
            };
            module.Start(session);
        }

        /// <summary>
        /// Парсит и выполняет указанную команду
        /// </summary>
        /// <param name="command"></param>
        private void ProcessCommand(string command)
        {
            // удаление пробелов и преобразование в низний регистр
            command = command.Trim().ToLower(CultureInfo.InvariantCulture);
            switch (command) {
                // Добавление сессии
                case "add":
                    this.AddSession();
                    break;
                // Выбор сессии и запуск модуля
                case "start":
                    var module = this.GetModule();
                    var session = this.GetSession();
                    this.StartModule(module, session);
                    break;
                default:
                    ConsoleHelpers.ShowError("Неизвестная команда");
                    break;
            }
        }

        /// <summary>
        /// Выводит приветствие на экран
        /// </summary>
        private void ShowWelcomeMessage()
        {
            const string message = @"
 ___  __   __        __   __           __   __        __   __        ___
|__  |__) |__) |    /  \ /  ` |__/    /  ` /  \ |\ | /__` /  \ |    |__  
|___ |  \ |  \ |___ \__/ \__, |  \    \__, \__/ | \| .__/ \__/ |___ |___ 
                                                                          ";
            ConsoleHelpers.WriteColorLine(message, ConsoleColor.DarkYellow);
            string appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var sb = new StringBuilder();
            sb.AppendFormatLine("Версия программы: {0}", appVersion);
            sb.AppendLine("Распространяется по лицензии MIT");
            sb.AppendLine("Для получения информации по командам введите help");
            sb.AppendLine();
            ConsoleHelpers.WriteColor(sb.ToString(), ConsoleColor.DarkGray);
        }

        /// <summary>
        /// Основной цикл работы приложения, выход из приложение осуществляется вводом 
        /// команды exit
        /// </summary>
        private void MainLoop()
        {
            ShowWelcomeMessage();
            var commandRequester = new ConsoleRequester<string>();
            commandRequester.AddPredicate(string.IsNullOrWhiteSpace, "Команда не должа быть пустой");
            string command = String.Empty;
            while (command != "exit") {
                command = commandRequester.RequestValue("Команда: ");
                ProcessCommand(command);
            }
        }
    }
}