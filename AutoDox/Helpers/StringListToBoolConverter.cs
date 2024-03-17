using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace AutoDox.UI.Helpers
{
    internal class StringListToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((List<string>)value).Count == 0 || ((List<string>)value)[0] == "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
