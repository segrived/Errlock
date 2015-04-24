using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Errlock.Lib.Helpers;

namespace Errlock.Views
{
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            string name = AppHelpers.ProjectName;
            string version = assemblyName.Version.ToString();

            var names = Assembly
                .GetExecutingAssembly()
                .GetReferencedAssemblies()
                .First(a => a.Name == "Errlock.Lib");
            this.ErrlockVersion.Text = String.Format("{0} {1}", name, version);
            this.ErrlockLibVersion.Text = String.Format("{0} {1}", names.Name, names.Version);
        }
    }
}