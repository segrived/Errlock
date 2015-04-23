using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Errlock.Lib.Modules;

namespace Errlock.Converters
{
    [ValueConversion(typeof(NoticePriority), typeof(Color))]
    public class NoticePriorityToColorConverter : IValueConverter
    {
        private readonly Dictionary<NoticePriority, Color> PropColors =
            new Dictionary<NoticePriority, Color> {
                { NoticePriority.Low, Colors.LightGreen },
                { NoticePriority.Medium, Colors.Yellow },
                { NoticePriority.High, Colors.LightPink },
                { NoticePriority.Info, Colors.LightBlue }
            };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (! (value is NoticePriority)) {
                throw new ArgumentException("value not of type StateValue");
            }
            var v = (NoticePriority)value;
            return new SolidColorBrush(PropColors[v]);
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}