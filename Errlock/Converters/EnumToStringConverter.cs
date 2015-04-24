﻿using System;
using System.Globalization;
using System.Windows.Data;
using Errlock.Lib;

namespace Errlock.Converters
{
    [ValueConversion(typeof(Enum), typeof(String))]
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Enum)value).GetDescription();
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
