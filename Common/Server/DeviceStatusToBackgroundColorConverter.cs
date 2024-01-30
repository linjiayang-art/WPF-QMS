using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SicoreQMS.Common.Server
{
    public class DeviceStatusToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 假设设备状态是一个字符串，您可以根据不同的状态返回不同的背景颜色
            if (value == null)
            {
                return new SolidColorBrush(Colors.Gray);
            }
            int status = (int)value;
            if (status == 0)
            {
                return new SolidColorBrush(Colors.Gray); // 正常状态背景颜色为绿色
            }
            if (status == 1)
            {
                return new SolidColorBrush(Colors.Green); // 正常状态背景颜色为绿色
            }
            if (status == 2)
            {
                return new SolidColorBrush(Colors.Yellow); // 正常状态背景颜色为绿色
            }
            else if (status == 3)
            {
                return new SolidColorBrush(Colors.Red); // 故障状态背景颜色为红色
            }
            else
            {
                return new SolidColorBrush(Colors.Gray); // 其他状态背景颜色为灰色
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
