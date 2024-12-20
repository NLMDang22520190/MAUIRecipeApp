using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIRecipeApp.Converter
{
    public class TimerEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Kiểm tra giá trị Timer
            if (value is string timer)
            {
                return timer == "00:00"; // Nếu Timer là "00:00" thì trả về true, ngược lại trả về false
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Không cần sử dụng ConvertBack
        }
    }
}
