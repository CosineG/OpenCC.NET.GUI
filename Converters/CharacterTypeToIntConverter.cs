using System;
using System.Windows.Data;
using OpenCC.NET.GUI.Enums;

namespace OpenCC.NET.GUI.Converters
{
    public class CharacterTypeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)(CharacterType)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (CharacterType)(int)value;
        }
    }
}