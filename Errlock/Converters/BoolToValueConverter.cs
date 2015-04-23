using System;
using System.Globalization;
using System.Windows.Data;

namespace Errlock.Converters
{
    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) {
                return FalseValue;
            }
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(TrueValue);
        }
    }

    public class BoolToStringConverter : BoolToValueConverter<string> { }

    public class BoolToObjectConverter : BoolToValueConverter<object> { }
}