using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XamBasePacket.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        public bool WhenNull { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
                return string.IsNullOrEmpty(str) ? WhenNull : !WhenNull;
            return value == null ? WhenNull : !WhenNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
