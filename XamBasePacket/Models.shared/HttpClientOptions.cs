using System;
using System.Net.Http;
using System.Threading.Tasks;
using Fusillade;
using XamBasePacket.Interfaces.Api;

namespace XamBasePacket.Models
{
    public class HttpClientOptions : IHttpClientOptions
    {
        public HttpClientOptions(Uri baseAddress, HttpMessageHandler messageHandler, Priority priority, Func<HttpRequestMessage, Task<string>> authToken)
        {
            BaseAddress = baseAddress;
            MessageHandler = messageHandler;
            Priority = priority;
            AuthTokenFunction = authToken;
        }

        public HttpClientOptions(Uri baseAddress) : this(baseAddress, null, Priority.UserInitiated, null) { }

        public HttpClientOptions(Uri baseAddress, HttpMessageHandler messageHandler) : this(baseAddress, messageHandler, Priority.UserInitiated, null) { }
        public HttpClientOptions(Uri baseAddress, HttpMessageHandler messageHandler, Priority priority) : this(baseAddress, messageHandler, priority, null) { }
        public HttpClientOptions() : this(null, null, Priority.UserInitiated, null) { }

        public string Name { get; set; }
        public Uri BaseAddress { get; set; }
        public HttpMessageHandler MessageHandler { get; set; }
        public Priority Priority { get; set; }
        public Func<HttpRequestMessage, Task<string>> AuthTokenFunction { get; set; }


        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int) 2166136261;

                hash = (hash * 16777619) ^ Name?.GetHashCode() ?? 0;
                hash = (hash * 16777619) ^ BaseAddress?.GetHashCode() ?? 0;
                hash = (hash * 16777619) ^ MessageHandler?.GetHashCode() ?? 0;
                hash = (hash * 16777619) ^ AuthTokenFunction?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            return Equals(other as HttpClientOptions);
        }
        public bool Equals(IHttpClientOptions other)
        {
            if (other == null)
                return false;
            return Name == other.Name
                   && BaseAddress == other.BaseAddress
                   && MessageHandler == other.MessageHandler;
        }
    }
}
