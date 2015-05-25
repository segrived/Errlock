using System.Collections.Generic;
using Errlock.Lib.Helpers;
using Errlock.Lib.Sessions;
using GalaSoft.MvvmLight.Command;

namespace Errlock.ViewModels
{
    public class NewSessionViewModel : CloseableViewModel
    {
        private string _protocolPart;
        private string _urlPart;
        public RelayCommand SaveSessionCommand { get; private set; }

        public Session Session
        {
            get { return Get<Session>(); }
            set { Set(value); }
        }

        public List<string> ProtocolList
        {
            get { return new List<string> { "http://", "https://" }; }
        }

        public string ProtocolPart
        {
            get { return this._protocolPart; }
            set
            {
                this._protocolPart = value;
                OnPropertyChanged();
                this.SaveSessionCommand.RaiseCanExecuteChanged();
            }
        }

        public string UrlPart
        {
            get { return this._urlPart; }
            set
            {
                this._urlPart = value;
                OnPropertyChanged();
                this.SaveSessionCommand.RaiseCanExecuteChanged();
            }
        }

        public string FullUrl
        {
            get { return this.ProtocolPart + this.UrlPart; }
        }

        public NewSessionViewModel()
        {
            this.SaveSessionCommand = new RelayCommand(SaveSession, CanSaveSession);
        }

        public void SaveSession()
        {
            this.Session.Url = this.FullUrl;
            App.SessionRepository.InsertOrUpdate(this.Session);
            this.OnClosingRequest();
        }

        public bool CanSaveSession()
        {
            if (this.ProtocolPart == null || this.UrlPart == null) {
                return false;
            }
            return WebHelpers.IsValidUrl(this.FullUrl);
        }
    }
}