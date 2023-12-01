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

    public abstract class ExtendedApiService<T> : IExtendedApiService<T>
    {
        #region fields
        private readonly IHttpClientProvider _clientProvider;
        private readonly RefitSettings _refitSettings;

        private readonly ConcurrentDictionary<IHttpClientOptions, T> _createdApis =
            new ConcurrentDictionary<IHttpClientOptions, T>();
        #endregion

        protected ExtendedApiService(IHttpClientProvider clientProvider, RefitSettings refitSettings)
        {
            _clientProvider = clientProvider;
            _refitSettings = refitSettings;
        }

        protected ExtendedApiService(IHttpClientProvider clientProvider) : this(clientProvider, null)
        {

        }

        public T GetApi(IHttpClientOptions options)
        {
            return CreateClient(options);
        }

        public T GetApi(Priority priority)
        {
            return GetApi(new HttpClientOptions() { Priority = priority });
        }

        protected virtual T CreateClient(IHttpClientOptions options) =>
         _createdApis.GetOrAdd(options, _refitSettings != null
                ? RestService.For<T>(_clientProvider.GetClient(options), _refitSettings)
                : RestService.For<T>(_clientProvider.GetClient(options)));

    }


    public abstract class ApiService<T> : IApiService<T>
    {
        private readonly Func<HttpRequestMessage, Task<string>> _getTokenWithParameters;
        private readonly Func<Task<string>> _getToken;
        private readonly string _apiUrl;


        protected ApiService(string apiBaseAddress)
        {
            _apiUrl = apiBaseAddress;
        }

        protected ApiService(string apiBaseAddress, Func<HttpRequestMessage, Task<string>> getToken)
        {
            _getTokenWithParameters = getToken;
            _apiUrl = apiBaseAddress;
        }

        protected ApiService(string apiBaseAddress, Func<Task<string>> getToken)
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
            if (_getToken != null || _getTokenWithParameters != null)
            {

                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(_apiUrl)
                };

                return RestService.For<T>(client, new RefitSettings()
                {
                    AuthorizationHeaderValueGetter = _getToken,
                    AuthorizationHeaderValueWithParamGetter = _getTokenWithParameters
                });
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
