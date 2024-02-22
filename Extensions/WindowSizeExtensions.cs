using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SicoreQMS.Extensions
{
    public class WindowSizeExtensions : IValueConverter
    {
     
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                double windowHeight = (double)value;
                double percentage = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
                return windowHeight * percentage;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
       


    }
}
