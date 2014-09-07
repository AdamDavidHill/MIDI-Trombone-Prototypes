using System;
using System.Globalization;
using System.Windows.Data;
using VBone.Data;

namespace VBone.Converters
{
    /// <summary>
    /// Converts a Trombone Position to a one-based number
    /// </summary>
    public class PositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Position)
            {
                return (int)value + 1;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (Position)(((int)value) - 1);
            }
            catch
            {
                return Position.First;
            }
        }
    }
}
