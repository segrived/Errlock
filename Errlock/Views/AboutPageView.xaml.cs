using System;
using System.Reflection;
using System.Windows;
using Errlock.Lib.Helpers;

namespace Errlock.Views
{
    public partial class AboutPageView
    {
        public AboutPageView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            string appVersion = assemblyName.Version.ToString();

            string libVersion = AppHelpers.LibVersion;

            this.ErrlockVersion.Text = String.Format("{0} {1}", AppHelpers.ProjectName, appVersion);
            this.ErrlockLibVersion.Text = String.Format("{0} {1}", "Errlock.Lib", libVersion);
        }
    }
}
