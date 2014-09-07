using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using VBone.Data;
using VBone.Logic;

namespace VBone.Converters
{
    public class InvertedBooleanToVisibilityConverter : IValueConverter
    { 
        public bool Collapse { get; set; }
 
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                if (Collapse)
                    return Visibility.Collapsed;
                else
                    return Visibility.Hidden; 
            }
            else
            {
                return Visibility.Visible;
            }
        }
 
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
    }
}
