using System.Windows;
using Errlock.Lib.Sessions;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class NewSession
    {
        private readonly NewSessionViewModel _viewModel = 
            new NewSessionViewModel();

        private readonly SessionScanOptions _defaultOptions = new SessionScanOptions {
            FetchPerPage = 20,
            MaxLinks = 100,
            RecursionDepth = 3,
            UseRandomLinks = true,
            IngoreAnchors = true
        };

        public NewSession()
        {
            InitializeComponent();
            this.DataContext = this._viewModel;
            this._viewModel.Session = new Session();
            this._viewModel.Session.Options = _defaultOptions;
        }

        public NewSession(Session session) : this()
        {
            this._viewModel.Session = session;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            this._viewModel.Session.Save();
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}