using System.Collections.Concurrent;
using FourTwenty.Mobile.Maui.Interfaces.Api;
using FourTwenty.Mobile.Maui.Models;
using Fusillade;

namespace FourTwenty.Mobile.Maui.Services.Api
{
    public class HttpClientProvider : IHttpClientProvider, IDisposable
    {
        private bool _disposed;

        private ConcurrentDictionary<IHttpClientOptions, HttpClient> _clients = new ConcurrentDictionary<IHttpClientOptions, HttpClient>();

        private static readonly IHttpClientOptions NullOptions = new HttpClientOptions() { };

        public virtual HttpClient GetClient(IHttpClientOptions? options = null)
        {
            if (options == null)
                return _clients.GetOrAdd(NullOptions, GetHttpClient);

            options.MessageHandler ??= GetPriorityHandler(options.Priority);

            return _clients.GetOrAdd(options, GetHttpClient);
        }

        protected virtual HttpClient GetHttpClient(IHttpClientOptions clientOptions)
        {
            if (clientOptions.Equals(NullOptions))
                return new HttpClient(GetPriorityHandler(Priority.UserInitiated), false);

            var client = clientOptions.MessageHandler != null
                ? new HttpClient(clientOptions.MessageHandler, false)
                : new HttpClient();

            if (clientOptions.BaseAddress != null)
                client.BaseAddress = clientOptions.BaseAddress;
            return client;
        }

        public virtual Task<HttpClient> GetClientAsync(IHttpClientOptions? options = null)
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
            var options = _clients.Keys.FirstOrDefault(d => d.Name == name) ?? new HttpClientOptions() { Name = name };
            return GetClient(options);
        }

        protected bool IsShouldDisposeHandler(HttpMessageHandler? handler)
        {
            if (handler == null)
                return false;

            bool isNetCache = handler == NetCache.Background
                   || handler == NetCache.UserInitiated
                   || handler == NetCache.Speculative
                   || handler == NetCache.UserInitiated
                   || handler == NetCache.Offline;
            if (!isNetCache && handler is DelegatingHandler delegatingHandler)
                return IsShouldDisposeHandler(delegatingHandler.InnerHandler);
            return !isNetCache;
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
                    if (IsShouldDisposeHandler(client.Key?.MessageHandler))
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
