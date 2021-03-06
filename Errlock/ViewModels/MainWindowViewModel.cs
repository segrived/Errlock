﻿using System;
using System.Collections.Generic;
using Errlock.Lib;
using Errlock.Lib.Modules;
using Errlock.Lib.Sessions;

namespace Errlock.ViewModels
{
    public class MainWindowViewModel : Bindable
    {
        private Func<IModule> _selectedModule;
        private Session _selectedSession;

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
                OnPropertyChanged();
                OnPropertyChanged("IsSelectedSession");
            }
        }

        public bool IsSelectedSession
        {
            get { return _selectedSession != null; }
        }

        //public SessionLogFile SelectedLogFile
        //{
        //    get { return Get<SessionLogFile>(); }
        //    set { Set(value); }
        //}

        public Func<IModule> SelectedModule
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