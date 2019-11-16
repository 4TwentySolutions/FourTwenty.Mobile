using System;
using System.Collections;
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
            switch (value)
            {
                case string str:
                    return string.IsNullOrEmpty(str) ? WhenNull : !WhenNull;
                case IList list:
                    return list.Count > 0 ? !WhenNull : WhenNull;
                case ICollection collection:
                    return collection.Count > 0 ? !WhenNull : WhenNull;
                default:
                    return value == null ? WhenNull : !WhenNull;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
