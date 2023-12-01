using FourTwenty.Mobile.Maui.Resources;

namespace FourTwenty.Mobile.Maui.Exceptions
{
    public class NoInternetException : Exception
    {

        public NoInternetException(string message) : base(message)
        {

        }

        public NoInternetException() : this(BaseResource.NoInternetConenction)
        {

        }
    }
}
