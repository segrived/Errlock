using System;
using System.Windows;
using Errlock.Lib.AppConfig;
using Errlock.Lib.Logger;

namespace Errlock
{
    public partial class App
    {
        private static readonly AppConfig Config = new AppConfig(ErrlockConfigModel.Defaults);

        public static readonly Logger Logger = new Logger((input, type) => {
            string time = DateTime.Now.ToString("HH:mm:ss");
            return String.Format("{0} | [{1}] {2}", time, type, input);
        });

        protected override void OnStartup(StartupEventArgs e)
        {
            Config.Model.LastStartTime = DateTime.Now;
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Config.Save();
        }
    }
}