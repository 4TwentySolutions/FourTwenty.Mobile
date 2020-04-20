using System;
using System.Net.Http;
using System.Threading.Tasks;
using Fusillade;

namespace XamBasePacket.Interfaces.Api
{
    public interface IHttpClientOptions : IEquatable<IHttpClientOptions>
    {
        string Name { get; set; }
        Uri BaseAddress { get; set; }
        HttpMessageHandler MessageHandler { get; set; }
        Priority Priority { get; set; }
        Func<HttpRequestMessage, Task<string>> AuthTokenFunction { get; set; }
    }

    public interface IHttpClientProvider : IHttpClientFactory
    {
        HttpClient GetClient(IHttpClientOptions options = null);
        Task<HttpClient> GetClientAsync(IHttpClientOptions options = null);
    }
}
