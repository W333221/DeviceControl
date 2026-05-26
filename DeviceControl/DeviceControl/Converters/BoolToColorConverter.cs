using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DeviceControl.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSuccess && isSuccess)
            {
                return Application.Current.Resources["SuccessColor"] != null ? new SolidColorBrush((Color)Application.Current.Resources["SuccessColor"]) : "#FF006400";
            }

            return Application.Current.Resources["DangerColor"] != null ? new SolidColorBrush((Color)Application.Current.Resources["DangerColor"]) : "#FFFF0C0C";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

    }
}
