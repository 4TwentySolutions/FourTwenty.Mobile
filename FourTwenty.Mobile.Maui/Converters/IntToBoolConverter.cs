using System.Globalization;

namespace FourTwenty.Mobile.Maui.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return false;
                case int val:
                    return val > 0;
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
