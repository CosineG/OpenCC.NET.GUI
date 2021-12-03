using System;
using System.Windows.Data;
using OpenCC.NET.GUI.Enums;

namespace OpenCC.NET.GUI.Converters
{
    public class FileStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var icon = (FileStatus) value switch
            {
                FileStatus.Ready => string.Empty,
                FileStatus.Running => "\xE9F5",
                FileStatus.Success => "\xE930",
                FileStatus.Fail => "\xEA39",
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };

            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var status = value.ToString() switch
            {
                "" => FileStatus.Ready,
                "\xE9F5" => FileStatus.Running,
                "\xE930" => FileStatus.Success,
                "\xEA39" => FileStatus.Fail,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            return status;
        }
    }
}