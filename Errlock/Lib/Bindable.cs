using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Errlock.Lib
{
    public class Bindable : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        public event PropertyChangedEventHandler PropertyChanged;

        protected T Get<T>([CallerMemberName] string name = null)
        {
            object value;
            if (_properties.TryGetValue(name, out value)) {
                return value == null ? default(T) : (T)value;
            }
            return default(T);
        }

        protected void Set<T>(T value, [CallerMemberName] string name = null)
        {
            if (Equals(value, Get<T>(name))) {
                return;
            }
            _properties[name] = value;
            OnPropertyChanged(name);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}