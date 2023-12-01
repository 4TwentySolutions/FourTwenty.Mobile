using System.ComponentModel;
using System.Reflection;

namespace FourTwenty.Mobile.Maui.Extensions
{
    public static class CommonExtensions
    {
        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        public static DateTime ChangeTime(this DateTime dateTime, TimeSpan timeSpan)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                timeSpan.Hours,
                timeSpan.Minutes,
                timeSpan.Seconds,
                timeSpan.Milliseconds,
                dateTime.Kind);
        }

        public static bool IsBetween<T>(this T item, T start, T end)
        {
            if (item == null)
                return false;
            return Comparer<T>.Default.Compare(item, start) >= 0
                   && Comparer<T>.Default.Compare(item, end) <= 0;
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static string GetHexString(this
            Color color)
        {
            int red = (int)(color.Red * 255);
            int green = (int)(color.Green * 255);
            int blue = (int)(color.Blue * 255);
            string hex = $"#{red:X2}{green:X2}{blue:X2}";

            return hex;
        }

        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }


        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable => listToClone.Select(item => (T)item.Clone()).ToList();

        public static string? GetDescription(this Enum value) => value?.GetType()?.GetMember(value.ToString())?
            .FirstOrDefault()
            ?.GetCustomAttribute<DescriptionAttribute>()
            ?.Description;

    }
}

