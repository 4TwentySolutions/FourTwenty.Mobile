using System;
using System.Globalization;
using Xamarin.Forms;

namespace XamBasePacket.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            if (value is bool val)
            {
                return val ? TrueValue : FalseValue;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
