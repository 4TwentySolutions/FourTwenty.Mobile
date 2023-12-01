using System;
using System.Globalization;
using Xamarin.Forms;

namespace XamBasePacket.Converters
{
    public partial class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
                return !val;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
                return !val;

            return null;
        }
    }
}
