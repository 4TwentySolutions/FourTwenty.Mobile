using System;
using System.Globalization;
using Xamarin.Forms;

namespace XamBasePacket.Converters
{
    public class BoolToIconConverter : IValueConverter
    {
        public ImageSource NullIcon { get; set; }
        public ImageSource TrueIcon { get; set; }
        public ImageSource FalseIcon { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return NullIcon;
            if (value is bool val)
            {
                return val ? TrueIcon : FalseIcon;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


}
