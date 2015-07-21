using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Errlock.Lib;
using Errlock.Lib.AppConfig;
using Errlock.Lib.Helpers;
using Errlock.Lib.Repository;
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

    /// <summary>
    /// Основной класс консольного приложения Errlock
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Конфигурация приложения
        /// </summary>
        private static readonly AppConfig ErrlockConfig =
            new AppConfig(ErrlockConfigModel.Defaults);

        /// <summary>
        /// Настройки подключения
        /// </summary>
        private readonly ConnectionConfiguration _connectionConfig =
            ErrlockConfig.Model.ConnectionConfiguration;

        /// <summary>
        /// Консольный логгер
        /// </summary>
        private readonly ILogger _consoleLogger = new ConsoleLogger();

        /// <summary>
        /// Репозиторий сессий
        /// </summary>
        private readonly IRepository<Session> _repository =
            new SessionLiteDbRepository();

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
        private void AddSession()
        {
            var s = new Session();
            var urlReq = new ConsoleRequester<string>();
            urlReq.AddPredicate(u => ! WebHelpers.IsValidUrl(u), "Неверный URL");
            s.Url = urlReq
                .RequestValue("Введите URL включая протокол");
            s.Options.RecursionDepth = ConsoleRequester
                .RequestInt("Введите макс. глубину рекурсии", 3);
            s.Options.FetchPerPage = ConsoleRequester
                .RequestInt("Максимальное количество ссылок с одной страницы", 20);
            s.Options.IngoreAnchors = ConsoleRequester
                .RequestBool("Игнорировать якоря", true);
            s.Options.UseRandomLinks = ConsoleRequester
                .RequestBool("Собирать случайные ссылки", true);
            s.Options.MaxLinks = ConsoleRequester
                .RequestInt("Глобальное ограничение на количество ссылок", 100);
            this._repository.InsertOrUpdate(s);
        }

        private void RemoveSession()
        {
            var sessions = this._repository.EnumerateAll();
            const string title = "Укажите сессию, которую необходимо удалить";
            var session = ConsoleRequester.RequestListItem(sessions, s => s.Url, title);
            this._repository.Delete(session);
        }

        private XssScanner CreateXssScannerInstance()
        {
            var config = new XssScannerConfig();
            return new XssScanner(config, _connectionConfig);
        }

        private PublicFinder CreatePublicFinderInstance()
        {
            var config = new PublicFinderConfig {
                DetectSuspicious = ConsoleRequester
                    .RequestBool("Обнаруживать подозрительные страинцы", true),
                UseGetRequests = ConsoleRequester
                    .RequestBool("Использовать GET-запросы вместо HEAD", false),
                UsePermutations = ConsoleRequester
                    .RequestBool("Использовать перестановки", true)
            };
            return new PublicFinder(config, _connectionConfig);
        }

        private PasswordCracker CreatePasswordCrackerInstance()
        {
            var config = new PasswordCrackerConfig {
                Login = ConsoleRequester
                    .RequestString("Логин", "admin"),
                RequestParameters = ConsoleRequester
                    .RequestString("Параметры запроса", "login={{login}}&password={{password}}"),
                RequestUrl = ConsoleRequester
                    .RequestString("URL запроса без адреса ресурса", "login"),
                PasswordsCount = ConsoleRequester
                    .RequestInt("Количество паролей", 100),
                UseHeuristic = ConsoleRequester
                    .RequestBool("Использовать эвристику", true),
                StopAfterFirstMatch = ConsoleRequester
                    .RequestBool("Останавливать после первого совпадения", true)
            };
            return new PasswordCracker(config, _connectionConfig);
        }

        private ConfigurationTest CreateConfigurationTestInstance()
        {
            var config = new ConfigurationTestConfig();
            return new ConfigurationTest(config, _connectionConfig);
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

        private void DisplaySessionInformation(Session session)
        {
            var sb = new StringBuilder();
            sb.AppendFormatLine("Адрес ресурса: {0}", session.Url);
            sb.AppendFormatLine("--- Конфигурация ---");
            sb.AppendFormatLine("Количество ссылок, собираемых со страницы: {0}", 
                session.Options.FetchPerPage);
            sb.AppendFormatLine("Глобальное ограничение на количество ссылок: {0}",
                session.Options.MaxLinks);
            sb.AppendFormatLine("Максимальная глубина рекурсии: {0}",
                session.Options.RecursionDepth);
            sb.AppendFormatLine("Игнорировать якори в адресах: {0}",
                session.Options.IngoreAnchors);
            sb.AppendFormatLine("Использовать случайные ссылки: {0}",
                session.Options.UseRandomLinks);
            string message = sb.ToString();
            ConsoleHelpers.WriteColor(message, ConsoleColor.DarkGray);
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
                foreach (string message in result.Notices.Select(PrepareNoticeMessage)) {
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
                // Удаление сессии
                case "remove":
                    this.RemoveSession();
                    break;
                // Выбор сессии и запуск модуля
                case "go":
                    if(! _repository.EnumerateAll().Any()) {
                        ConsoleHelpers.ShowError("Сначала добавьте хотя бы одну сессию");
                        return;
                    }
                    var module = this.GetModule();
                    var session = this.GetSession();
                    this.StartModule(module, session);
                    break;
                case "info":
                    this.DisplaySessionInformation(this.GetSession());
                    break;
                // Информацию о программе
                case "about":
                    this.ShowAboutMessage();
                    break;
                // Отображение справки
                case "help":
                    this.DisplayHelp();
                    break;
                default:
                    const string msg = "Неизвестная команда. Для отображения списка доступых " +
                                       "комманд введите help";
                    ConsoleHelpers.ShowError(msg);
                    break;
            }
        }

        /// <summary>
        /// Отображает справку по командам программы
        /// </summary>
        private void DisplayHelp()
        {
            System.Console.WriteLine();
            ConsoleHelpers.WriteColorLine("СПРАВКА ПО КОМАНДАМ", ConsoleColor.Yellow);
            System.Console.WriteLine();

            ConsoleHelpers.WriteColor("add", ConsoleColor.Magenta);
            const string addHelp = " - запрашивает информацию и добавляет сессию в репозиторий";
            System.Console.WriteLine(addHelp);

            ConsoleHelpers.WriteColor("remove", ConsoleColor.Magenta);
            const string removeHelp = " - запрашивает сессию и удаляет её";
            System.Console.WriteLine(removeHelp);

            ConsoleHelpers.WriteColor("go", ConsoleColor.Magenta);
            const string goHelp = " - отображает диалоги выбора модуля и сессии " +
                                  "после чего запускает тестирование";
            System.Console.WriteLine(goHelp);

            ConsoleHelpers.WriteColor("info", ConsoleColor.Magenta);
            const string infoHelp = " - отображает информацию по существующей сессии";
            System.Console.WriteLine(infoHelp);

            ConsoleHelpers.WriteColor("about", ConsoleColor.Magenta);
            const string aboutHelp = " - отображает информацию о программе";
            System.Console.WriteLine(aboutHelp);

            ConsoleHelpers.WriteColor("help", ConsoleColor.Magenta);
            const string helpHelp = " - отображает эту справку";
            System.Console.WriteLine(helpHelp);
        }

        /// <summary>
        /// Выводит приветствие на экран
        /// </summary>
        private void ShowAboutMessage()
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
            ShowAboutMessage();
            var commandRequester = new ConsoleRequester<string>();
            commandRequester.AddPredicate(string.IsNullOrWhiteSpace, "Команда не должа быть пустой");
            string command = String.Empty;
            while (command != "exit") {
                command = commandRequester.RequestValue(color: ConsoleColor.Cyan);
                ProcessCommand(command);
                System.Console.WriteLine();
            }
        }
    }
}