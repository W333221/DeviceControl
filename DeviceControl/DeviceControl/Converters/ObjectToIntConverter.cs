using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DeviceControl.Converters
{
    public class ObjectToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 将 object 类型转为 decimal（支持 int/double/float 等）
            if (value is int intValue) return intValue;
            if (value is double doubleValue) return (int)doubleValue;
            if (value is float floatValue) return (int)floatValue;
            if (value is decimal decimalValue) return (int)decimalValue;

            return 0m; // 默认值
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 将 decimal 转回原始类型（根据目标类型动态转换）
            if (targetType == typeof(decimal)) return System.Convert.ToInt32(value);
            if (targetType == typeof(double)) return System.Convert.ToInt32(value);
            if (targetType == typeof(float)) return System.Convert.ToInt32(value);
            return value; // 默认返回 decimal
        }
    }
}
