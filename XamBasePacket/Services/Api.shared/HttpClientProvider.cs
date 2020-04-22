using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using Fusillade;
using Refit;
using XamBasePacket.Interfaces.Api;
using XamBasePacket.Models;

namespace XamBasePacket.Services.Api
{
    public class HttpClientProvider : IHttpClientProvider, IDisposable
    {
        private bool _disposed;

        private ConcurrentDictionary<IHttpClientOptions, HttpClient> _clients = new ConcurrentDictionary<IHttpClientOptions, HttpClient>();

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
            if (clientOptions.Equals(NullOptions))
                return new HttpClient(GetPriorityHandler(Priority.UserInitiated), false);

            var client = new HttpClient(clientOptions.MessageHandler, false);
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

        #region IDisposable
        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                foreach (var client in _clients)
                {
                    client.Key?.MessageHandler?.Dispose();
                    client.Value?.Dispose();
                }
                _clients.Clear();
                _clients = null;
            }
            // Free any unmanaged objects here.            
            _disposed = true;

        }

        ~HttpClientProvider()
        {
            Dispose(false);
        }
        #endregion
    }
}
