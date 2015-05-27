using System;
using System.Threading;
using System.Windows;
using Errlock.Lib;
using Errlock.Lib.AppConfig;
using Errlock.Lib.Repository;
using Errlock.Lib.Logger;
using Errlock.Lib.Sessions;

namespace Errlock
{
    public partial class App
    {
        public static readonly AppConfig Config = new AppConfig(ErrlockConfigModel.Defaults);

        public static IRepository<Session> SessionRepository;

        public static readonly Logger Logger = new Logger((input, type) => {
            string time = DateTime.Now.ToString("HH:mm:ss");
            return String.Format("{0} | [{1}] {2}", time, type.GetDescription(), input);
        });

        protected override void OnStartup(StartupEventArgs e)
        {
            bool aIsNewInstance;
            new Mutex(true, "ErrlockApp", out aIsNewInstance);
            if (! aIsNewInstance) {
                MessageBox.Show("Приложение уже запущено", "Ошибка", MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                Current.Shutdown();
            }

            SessionRepository = new SessionLiteDbRepository();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Config.Save();
        }
    }
}