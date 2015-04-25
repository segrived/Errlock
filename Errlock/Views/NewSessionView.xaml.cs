using System;
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
            this._viewModel.Session = new Session {
                Options = _defaultOptions
            };
            this._viewModel.ClosingRequest += (sender, e) => this.Close();
            this.DataContext = this._viewModel;
        }
    }
}