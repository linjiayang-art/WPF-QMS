using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace SicoreQMS.Common.Models.Basic
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString)
            {
                return (Brush)new BrushConverter().ConvertFromString(colorString);
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
