using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XamBasePacket.Bases;
using XamBasePacket.Services;

namespace XamBasePacket.Tests.Services
{
    public class WebApiTypesService : BaseApiService
    {
        public class SomeModel
        {
            public string Data { get; set; }
        }
        public Task Dummy()
        {
            return Task.FromResult(true);
        }

        public Task<IResponse<Stream>> GetFile()
        {
            return MakeRequest<Stream>("emptyUrl", HttpMethod.Get);
        }

        public Task<IResponse<MemoryStream>> GetFileMemory()
        {
            return MakeRequest<MemoryStream>("emptyUrl", HttpMethod.Get);
        }

        public Task<IResponse<SomeModel>> GetModelData()
        {
            return MakeRequest<SomeModel>("emptyUrl", HttpMethod.Get);
        }

        protected override Task<IResponse<T>> MakeRequest<T>(string url, HttpMethod httpMethod, HttpContent content = null, string accessToken = null,
            string mediaType = "application/json", CancellationToken token = default(CancellationToken),
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead, Dictionary<string, string> requestHeaders = null,
            string defaultScheme = "Bearer")
        {
            object result;
            if (typeof(T) != typeof(Stream) && !typeof(T).IsSubclassOf(typeof(Stream)))
            {
                var data = new SomeModel() { Data = "SomeData" };
                result = data;

            }
            else
            {
                object data = new MemoryStream();
                result = (T)data;
            }

            return Task.FromResult((IResponse<T>)new Response<T>(true) { Content = (T)result });
        }

        public WebApiTypesService()
        {

        }

    }
}
