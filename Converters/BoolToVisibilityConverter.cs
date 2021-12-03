using System;
using System.Windows;
using System.Windows.Data;

namespace OpenCC.NET.GUI.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = (bool) value switch
            {
                true => Visibility.Visible,
                false => Visibility.Hidden
            };
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var boolVisibility = (Visibility) value switch
            {
                Visibility.Visible => true,
                Visibility.Hidden => false,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            return boolVisibility;
        }
    }
}