using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace DeviceControl.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                // 根据需求选择：
                // 方案1：false 时返回 Collapsed（不占空间）
                return isVisible ? Visibility.Visible : Visibility.Collapsed;

                // 方案2：false 时返回 Hidden（占空间但不显示）[4](@ref)
                // return isVisible ? Visibility.Visible : Visibility.Hidden;
            }
            return Visibility.Visible; // 默认行为
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
