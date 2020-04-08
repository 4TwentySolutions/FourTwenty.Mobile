using System;
using System.Net.Http;
using System.Threading.Tasks;
using Fusillade;
using Refit;
using XamBasePacket.Interfaces.Api;
using XamBasePacket.Models;

namespace XamBasePacket.Services.Api
{

    public abstract class ExtendedApiService<T> : IExtendedApiService<T>
    {
        #region fields
        private readonly IHttpClientProvider _clientProvider;
        #endregion

        protected ExtendedApiService(IHttpClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public T GetApi(IHttpClientOptions options)
        {
            return CreateClient(options);
        }

        public T GetApi(Priority priority)
        {
            return GetApi(new HttpClientOptions() { Priority = priority });
        }

        protected virtual T CreateClient(IHttpClientOptions options) => RestService.For<T>(_clientProvider.GetClient(options));
    }


    public abstract class ApiService<T> : IApiService<T>
    {
        private readonly Func<HttpRequestMessage, Task<string>> _getToken;
        private readonly string _apiUrl;


        protected ApiService(string apiBaseAddress) : this(apiBaseAddress, null) { }
        protected ApiService(string apiBaseAddress, Func<HttpRequestMessage, Task<string>> getToken)
        {
            _getToken = getToken;
            _apiUrl = apiBaseAddress;
        }


        private T Background
        {
            get
            {
                return new Lazy<T>(() => CreateClient(
                    NetCache.Background)).Value;
            }
        }

        private T UserInitiated
        {
            get
            {
                return new Lazy<T>(() => CreateClient(
              NetCache.UserInitiated)).Value;
            }
        }

        private T Speculative
        {
            get
            {
                return new Lazy<T>(() => CreateClient(
              NetCache.Speculative)).Value;
            }
        }

        public T GetApi(Priority priority)
        {
            switch (priority)
            {
                case Priority.Background:
                    return Background;
                case Priority.UserInitiated:
                    return UserInitiated;
                case Priority.Speculative:
                    return Speculative;
                default:
                    return UserInitiated;
            }
        }

        protected virtual T CreateClient(HttpMessageHandler messageHandler)
        {
            if (_getToken != null)
            {
                var client = new HttpClient(new AuthenticatedParameterizedHttpClientHandler(_getToken, messageHandler))
                {
                    BaseAddress = new Uri(_apiUrl)
                };

                return RestService.For<T>(client);
            }
            else
            {
                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(_apiUrl)
                };
                return RestService.For<T>(client);
            }
        }
    }
}
