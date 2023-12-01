using System.Collections.Concurrent;
using FourTwenty.Mobile.Maui.Interfaces.Api;
using FourTwenty.Mobile.Maui.Models;
using Fusillade;
using Refit;

namespace FourTwenty.Mobile.Maui.Services.Api
{

    public abstract class ExtendedApiService<T>(IHttpClientProvider clientProvider, RefitSettings? refitSettings = null)
        : IExtendedApiService<T>
    {
        #region fields

        private readonly ConcurrentDictionary<IHttpClientOptions, T> _createdApis =
            new ConcurrentDictionary<IHttpClientOptions, T>();
        #endregion

        public T GetApi(IHttpClientOptions options)
        {
            return CreateClient(options);
        }

        public T GetApi(Priority priority)
        {
            return GetApi(new HttpClientOptions() { Priority = priority });
        }

        protected virtual T CreateClient(IHttpClientOptions options) =>
         _createdApis.GetOrAdd(options, refitSettings != null
                ? RestService.For<T>(clientProvider.GetClient(options), refitSettings)
                : RestService.For<T>(clientProvider.GetClient(options)));

    }


    public abstract class ApiService<T>(string apiBaseAddress,
            Func<HttpRequestMessage, CancellationToken, Task<string>>? getToken = null)
        : IApiService<T>
    {
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
            if (getToken != null)
            {

                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(apiBaseAddress)
                };

                return RestService.For<T>(client, new RefitSettings()
                {
                    AuthorizationHeaderValueGetter = getToken,
                });
            }
            else
            {
                var client = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri(apiBaseAddress)
                };
                return RestService.For<T>(client);
            }
        }
    }
}
