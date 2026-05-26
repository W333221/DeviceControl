using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DeviceControl.Converters
{
    public class BoolToRadioConverter : IValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    // 将 ViewModel 属性与参数比对（如 "True" 字符串）
        //    return value?.ToString() == parameter?.ToString();
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    // 选中时返回参数对应的值（如 bool 类型）
        //    return (bool)value ? System.Convert.ChangeType(parameter, targetType) : Binding.DoNothing;
        //}

        public object Convert(object value, Type t, object param, CultureInfo c) => value is bool b && b == System.Convert.ToBoolean(param);

        public object ConvertBack(object value, Type t, object param, CultureInfo c)
            => value is true ? System.Convert.ToBoolean(param) : Binding.DoNothing;
    }
}
