using System.Collections.Generic;
using System.Windows;
using Errlock.Converters;
using Errlock.Lib;
using Errlock.Lib.Modules;
using Errlock.Lib.Sessions;

namespace Errlock.ViewModels
{
    public class MainWindowViewModel : Bindable
    {
        private Session _selectedSession;
        private IModule _selectedModule;

        public IEnumerable<Session> Sessions
        {
            get { return Get<IEnumerable<Session>>(); }
            set { Set(value); }
        }

        public Session SelectedSession
        {
            get { return _selectedSession; }
            set
            {
                _selectedSession = value;
                OnPropertyChanged("SelectedSession");
                OnPropertyChanged("IsSelectedSession");
            }
        }

        public bool IsSelectedSession
        {
            get { return _selectedSession != null; }
        }

        public IModule SelectedModule
        {
            get { return _selectedModule; }
            set
            {
                _selectedModule = value;
                OnPropertyChanged("SelectedModule");
                OnPropertyChanged("IsModuleSelected");
            }
        }

        public bool IsModuleSelected
        {
            get { return _selectedModule != null; }
        }
    }
}