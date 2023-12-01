using System.Globalization;

namespace FourTwenty.Mobile.Maui.Converters
{
    public class BoolToIconConverter : IValueConverter
    {
        public ImageSource? NullIcon { get; set; }
        public ImageSource? TrueIcon { get; set; }
        public ImageSource? FalseIcon { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return NullIcon;
                case bool val:
                    return val ? TrueIcon : FalseIcon;
                default:
                    return null;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
        }
    }


}
