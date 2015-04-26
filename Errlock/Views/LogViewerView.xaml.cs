using Errlock.Lib.Sessions;
using Errlock.ViewModels;

namespace Errlock.Views
{
    public partial class LogViewerView
    {
        private readonly LogViewerViewModel _viewModel = new LogViewerViewModel();

        public LogViewerView()
        {
            InitializeComponent();
            this.DataContext = _viewModel;
        }

        public LogViewerView(SessionLogFile logData) : this()
        {
            this._viewModel.LogData = logData;
        }
    }
}