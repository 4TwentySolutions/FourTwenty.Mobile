using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Fusillade;
using Refit;
using XamBasePacket.Interfaces.Api;
using XamBasePacket.Models;

namespace XamBasePacket.Services.Api
{
    public class HttpClientProvider : IHttpClientProvider
    {
        private readonly ConcurrentDictionary<IHttpClientOptions, HttpClient> _clients = new ConcurrentDictionary<IHttpClientOptions, HttpClient>();

        private static readonly IHttpClientOptions NullOptions = new HttpClientOptions() { };

        public virtual HttpClient GetClient(IHttpClientOptions options = null)
        {
            if (options == null)
                return _clients.GetOrAdd(NullOptions, GetHttpClient);

            if (options.MessageHandler == null)
                options.MessageHandler = options.AuthTokenFunction != null
                    ? new AuthenticatedParameterizedHttpClientHandler(options.AuthTokenFunction, GetPriorityHandler(options.Priority))
                    : GetPriorityHandler(options.Priority);

            return _clients.GetOrAdd(options, GetHttpClient);
        }

        protected virtual HttpClient GetHttpClient(IHttpClientOptions clientOptions)
        {
            if (clientOptions == NullOptions)
                return new HttpClient(GetPriorityHandler(Priority.UserInitiated));

            var client = new HttpClient(clientOptions.MessageHandler);
            if (clientOptions.BaseAddress != null)
                client.BaseAddress = clientOptions.BaseAddress;
            return client;
        }

        public virtual Task<HttpClient> GetClientAsync(IHttpClientOptions options = null)
        {
            return Task.FromResult(GetClient(options));
        }

        protected virtual HttpMessageHandler GetPriorityHandler(Priority priority) => priority switch
        {
            Priority.Background => NetCache.Background,
            Priority.UserInitiated => NetCache.UserInitiated,
            Priority.Speculative => NetCache.Speculative,
            _ => NetCache.UserInitiated
        };

        public HttpClient CreateClient(string name)
        {
            return GetClient();
        }
    }
}
