using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Fusillade;

namespace XamBasePacket.Interfaces.Api
{
    public interface IHttpClientOptions
    {
        Uri BaseAddress { get; set; }
        HttpMessageHandler MessageHandler { get; set; }
        Priority Priority { get; set; }
        Func<HttpRequestMessage, Task<string>> AuthTokenFunction { get; set; }
    }

    public interface IHttpClientProvider
    {
        HttpClient GetClient(IHttpClientOptions options = null);
        Task<HttpClient> GetClientAsync(IHttpClientOptions options = null);
    }
}
