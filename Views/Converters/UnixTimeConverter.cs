using System;
using System.Globalization;
using System.Windows.Data;

namespace EvernoteClone.Views.Converters
{
    [ValueConversion(typeof(long), typeof(string))]
    internal class UnixTimeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter,
            CultureInfo culture)
        {
            return value is long ut
                ? DateTimeOffset.FromUnixTimeSeconds(ut).LocalDateTime
                    .ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                : "N/A";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
