using System;
using System.Windows.Data;
using OpenCC.NET.GUI.Enums;

namespace OpenCC.NET.GUI.Converters
{
    public class VariantTypeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)(VariantType)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (VariantType)(int)value;
        }
    }
}