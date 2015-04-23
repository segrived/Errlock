using Errlock.Lib;
using Errlock.Lib.Sessions;

namespace Errlock.ViewModels
{
    public class NewSessionViewModel : Bindable
    {
        public Session Session
        {
            get { return Get<Session>(); }
            set { Set(value); }
        }
    }
}