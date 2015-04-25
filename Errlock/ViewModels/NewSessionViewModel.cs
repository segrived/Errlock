using Errlock.Lib;
using Errlock.Lib.Sessions;
using GalaSoft.MvvmLight.Command;
using Errlock.Lib.Helpers;

namespace Errlock.ViewModels
{
    public class NewSessionViewModel : CloseableViewModel
    {
        public RelayCommand SaveSessionCommand { get; private set; }

        public Session Session
        {
            get { return Get<Session>(); }
            set { Set(value); }
        }

        private string _protocolPart;
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

        private string _urlPart;
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
            this.Session.Save();
            this.OnClosingRequest();
        }

        public bool CanSaveSession()
        {
            if (this.ProtocolPart == null || this.UrlPart == null) {
                return false;
            } else {
                return WebHelpers.IsValidUrl(this.FullUrl);
            }
        }
    }
}