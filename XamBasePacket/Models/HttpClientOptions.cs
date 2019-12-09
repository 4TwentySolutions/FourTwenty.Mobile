using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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

        public Uri BaseAddress { get; set; }
        public HttpMessageHandler MessageHandler { get; set; }
        public Priority Priority { get; set; }
        public Func<HttpRequestMessage, Task<string>> AuthTokenFunction { get; set; }
    }
}
