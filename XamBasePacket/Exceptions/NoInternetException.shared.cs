using System;
using XamBasePacket.Resources;

namespace XamBasePacket.Exceptions
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
