using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DeviceControl.Converters
{
    public class ResultToTipColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() == "OK")
            {
                return "#73b500";
            }
            else if (value != null && value.ToString() == "NG")
            {
                return "#ff0013";
            }
            else
            {
                return "#007eff";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }
    }
}
