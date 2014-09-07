using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using VBone.Data;

namespace VBone.Converters
{
    /// <summary>
    /// Converts a Trombone Position and a parameter to a visibilty
    /// </summary>
    public class CurrentPositionVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int a = 0;
            int b = 0;

            if (value is Position)
            {
                a = (int)value;
                b = int.Parse(parameter.ToString());

                return a == b ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
