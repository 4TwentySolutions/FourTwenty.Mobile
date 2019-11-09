using System;
using System.Globalization;
using Xamarin.Forms;

namespace XamBasePacket.Converters
{
	public class IntToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return false;
			if (value is int val)
			{
				return val > 0;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
