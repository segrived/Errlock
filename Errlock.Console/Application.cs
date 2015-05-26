using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Errlock.Lib;
using Errlock.Lib.AppConfig;
using Errlock.Lib.Helpers;
using Errlock.Lib.Logger;
using Errlock.Lib.Modules;
using Errlock.Lib.Modules.ConfigurationTestModule;
using Errlock.Lib.Modules.PasswordCrackerModule;
using Errlock.Lib.Modules.PublicFinderModule;
using Errlock.Lib.Modules.XssScannerModule;
using Errlock.Lib.Sessions;

namespace ErrlockConsole
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message, LoggerMessageType messsageType)
        {
            Console.WriteLine("[{0}] {1}", messsageType, message);
        }
    }

    public class Application
    {
        private static readonly AppConfig ErrlockConfig = new AppConfig(ErrlockConfigModel.Defaults);

        private readonly ILogger _consoleLogger = new ConsoleLogger();

        private IRepository<Session> _repository = new SessionLiteDbRepository();

        private Session SelectedSession { get; set; }

        private bool IsSessionSelected
        {
            get { return SelectedSession != null; }
        }

        public static void Run()
        {
            var app = new Application();
            app.MainLoop();
        }

        private Session AddSession() {
            var s = new Session();
            var urlReq = new ConsoleRequester<string>();
            urlReq.AddPredicate(u => ! WebHelpers.IsValidUrl(u), "Неверный URL");
            s.Url = urlReq
                .RequestValue("Введите URL (включая протокол, например http://google.ru)");
            s.Options.RecursionDepth = ConsoleRequester
                .RequestInt("Введите макс. глубину рекурсии");
            s.Options.FetchPerPage = ConsoleRequester
                .RequestInt("Максимальное количество ссылок с одной страницы");
            s.Options.IngoreAnchors = ConsoleRequester
                .RequestBool("Игнорировать якоря");
            s.Options.UseRandomLinks = ConsoleRequester
                .RequestBool("Собирать случайные ссылки");
            s.Options.MaxLinks = ConsoleRequester
                .RequestInt("Глобальное ограничение на количество ссылок");
            this._repository.InsertOrUpdate(s);
            return s;
        }

        private Session ReadSession()
        {
            var sessionList = this._repository.EnumerateAll();
            return ConsoleRequester.RequestListItem(sessionList, s => s.Url);
        }

        private static XssScanner CreateXssScannerInstance()
        {
            var connectionConfig = ErrlockConfig.Model.ConnectionConfiguration;
            var config = new XssScannerConfig();
            return new XssScanner(config, connectionConfig);
        }

        private static PublicFinder CreatePublicFinderInstance()
        {
            var connectionConfig = ErrlockConfig.Model.ConnectionConfiguration;
            var config = new PublicFinderConfig {
                DetectSuspicious = ConsoleRequester
                    .RequestBool("Обнаруживать подозрительные страинцы: "),
                UseGetRequests = ConsoleRequester
                    .RequestBool("Использовать GET-запросы вместо HEAD: "),
                UsePermutations = ConsoleRequester
                    .RequestBool("Использовать перестановки: ")
            };
            return new PublicFinder(config, connectionConfig);
        }

        private static PasswordCracker CreatePasswordCrackerInstance()
        {
            var connectionConfig = ErrlockConfig.Model.ConnectionConfiguration;     
            var config = new PasswordCrackerConfig {
                Login = ConsoleRequester
                    .RequestString("Логин: "),
                RequestParameters = ConsoleRequester
                    .RequestString("Параметры запроса"),
                RequestUrl = ConsoleRequester
                    .RequestString("URL запроса (без адреса ресурса)"),
                PasswordsCount = ConsoleRequester
                    .RequestInt("Количество паролей: "),
                UseHeuristic = ConsoleRequester
                    .RequestBool("Использовать эвристику: "),
                StopAfterFirstMatch = ConsoleRequester
                    .RequestBool("Останавливать после первого совпадения")
            };
            return new PasswordCracker(config, connectionConfig);
        }

        private void ProcessCommand(string command, List<string> arguments)
        {
            switch (command) {
                case "add":
                    this.AddSession();
                    break;
                case "setsession":
                    try {
                        this.SelectedSession = ReadSession();
                    } catch (EmptyCollectionException ex) {
                        var msg = "Нет созданных сессий, сначала создайте сессию с помощью команды add";
                        ConsoleHelpers.ShowError(msg);
                    }
                    break;
                case "start":
                    if (arguments.Count != 1) {
                        ConsoleHelpers.ShowError("Неверное количество аргументов");
                        return;
                    }
                    if (! this.IsSessionSelected) {
                        ConsoleHelpers.ShowError("Не выбрана сессия, для начала выберите сессию");
                        return;
                    }
                    string modName = arguments.First();
                    var connectionConfig = ErrlockConfig.Model.ConnectionConfiguration;
                    IModule module = null;
                    switch (modName) {
                        case "xss":
                            module = CreateXssScannerInstance();
                            break;
                        case "configuration":
                            module = new ConfigurationTest(connectionConfig);
                            break;
                        case "public":
                            module = CreatePublicFinderInstance();
                            break;
                        case "password":
                            module = CreatePasswordCrackerInstance();
                            break;
                        default:
                            ConsoleHelpers.ShowError("Неизвестный модуль");
                            break;
                    }
                    if (module == null) { } else {
                        module.SetLogger(this._consoleLogger);
                        module.Completed += (sender, args) => {
                            var result = args.ScanResult;
                            foreach (var notice in result.Notices) {
                                ConsoleHelpers.ShowOkMessage(notice.ToString());
                            }
                        };
                        module.Start(this.SelectedSession);
                    }
                    break;
                default:
                    Console.WriteLine("Неизвестная команда");
                    break;
            }
        }

        private void ProcessCommand(string command)
        {
            var splitRegex = new Regex(@"\s+", RegexOptions.Compiled);
            // удаление пробелов и преобразование в низний регистр
            command = command.Trim().ToLower(CultureInfo.InvariantCulture);
            var parts = splitRegex.Split(command);
            var commandPart = parts[0];
            var arguments = parts.Skip(1).ToList();
            ProcessCommand(commandPart, arguments);
        }

        private void ShowWelcomeMessage()
        {
            string message = @"
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