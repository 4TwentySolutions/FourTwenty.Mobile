using System;
using System.Globalization;
using Xamarin.Forms;

namespace XamBasePacket.Converters
{
    public partial class BoolToStringConverter : IValueConverter
    {
        public string NullValue { get; set; }
        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return !string.IsNullOrEmpty(NullValue) ? NullValue : FalseValue;
                case bool val:
                    return val ? TrueValue : FalseValue;
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
