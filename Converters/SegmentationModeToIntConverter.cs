using System;
using System.Globalization;
using System.Windows.Data;
using OpenCC.NET.GUI.Enums;

namespace OpenCC.NET.GUI.Converters
{
    public class SegmentationModeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = (SegmentationMode)value;
            return (int)source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = (int)value;
            return (SegmentationMode)source;
        }
    }
}
