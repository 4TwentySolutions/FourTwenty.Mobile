using System;
using System.Threading.Tasks;
using Fusillade;
using XamBasePacket.Bases;
using XamBasePacket.Interfaces.Api;
using XamBasePacket.Models;
using XamBasePacket.Services.Api;
using XamBasePacket.Services.Api.shared;
using XamBasePacket.Tests.Services.Api;

namespace XamBasePacket.Tests.Services
{
    public abstract class BaseApiTest
    {

        protected ITestApiManager GetApiManager() => new TestApiManager(new GoogleApiService(), new PlaceholderApiService(), new PlaceholderExtendedApiService(new HttpClientProvider()));
        protected ITestApiManager GetApiManager(HttpClientProvider provider) => new TestApiManager(new GoogleApiService(), new PlaceholderApiService(), new PlaceholderExtendedApiService(provider));



        #region implementation
        protected class PlaceholderExtendedApiService : ExtendedApiService<IPlaceholderApi>
        {
            public PlaceholderExtendedApiService(IHttpClientProvider clientProvider) : base(clientProvider)
            {

            }
        }

        protected class PlaceholderApiService : ApiService<IPlaceholderApi>
        {
            public PlaceholderApiService() : base("https://jsonplaceholder.typicode.com/")
            {
            }

        }
        protected class GoogleApiService : ApiService<IGoogleApi>
        {
            public GoogleApiService() : base("https://google.com")
            {
            }

        }


        protected interface ITestApiManager : IApiManager
        {
            Task<IResponse<string>> GetRawGoogle();
            Task<IResponse<string>> GetPlaceholderTodos();
            Task<IResponse<string>> GetPlaceholderComments(string baseUrl);
            Task<IResponse<string>> Failure();
        }

        protected class TestApiManager : SimpleApiManager, ITestApiManager
        {
            private readonly IApiService<IGoogleApi> _googleService;
            private readonly IApiService<IPlaceholderApi> _placeholderService;
            private readonly IExtendedApiService<IPlaceholderApi> _placeholderExtendedApiService;
            public TestApiManager(
                IApiService<IGoogleApi> googleService,
                IApiService<IPlaceholderApi> placeholderService) : this(googleService, placeholderService, null)
            {
            }

            public TestApiManager(
                IApiService<IGoogleApi> googleService,
                IApiService<IPlaceholderApi> placeholderService,
                IExtendedApiService<IPlaceholderApi> placeholderExtendedApiService)
            {
                _googleService = googleService;
                _placeholderService = placeholderService;
                _placeholderExtendedApiService = placeholderExtendedApiService;
            }

            public async Task<IResponse<string>> GetRawGoogle()
            {

                return await MakeGenericRequest((ct) =>
                    MakeRequest((innerCt) => _googleService.GetApi(Priority.Background).GetRawGoogle(innerCt), ct));
            }

            public async Task<IResponse<string>> GetPlaceholderTodos()
            {

                return await MakeGenericRequest((ct) =>
                    MakeRequest((innerCt) => _placeholderService.GetApi(Priority.Background).GetTodos(innerCt), ct));
            }

            public async Task<IResponse<string>> GetPlaceholderComments(string baseUrl)
            {

                return await MakeGenericRequest((ct) =>
                    MakeRequest((innerCt) => _placeholderExtendedApiService.GetApi(new HttpClientOptions(new Uri(baseUrl), null, Priority.UserInitiated)).Comments(innerCt), ct));
            }
            public async Task<IResponse<string>> Failure()
            {

                return await MakeGenericRequest((ct) =>
                    MakeRequest((innerCt) => _placeholderService.GetApi(Priority.UserInitiated).Failure(innerCt), ct));
            }
        }
        #endregion
    }
}
