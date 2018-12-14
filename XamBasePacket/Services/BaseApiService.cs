using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XamBasePacket.Bases;
using System.IO;

namespace XamBasePacket.Services
{
    public abstract class BaseApiService : IDisposable
    {
        #region fields
        protected readonly HttpClient HttpClient;
        private bool _isDisposed;
        #endregion

        #region ctor
        protected BaseApiService() : this(string.Empty)
        {

        }

        protected BaseApiService(string baseUrl) : this(baseUrl, new HttpClientHandler())
        {
        }

        protected BaseApiService(string baseUrl, HttpClientHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException($"{nameof(handler)} can't be null");
            HttpClient = new HttpClient(handler, true);
            if (!string.IsNullOrEmpty(baseUrl))
                HttpClient.BaseAddress = new Uri(baseUrl);

        }
        #endregion

        protected virtual async Task<Response<Stream>> MakeStreamRequest(string url,
           HttpMethod httpMethod,
           HttpContent content = null,
           string accessToken = null,
           string mediaType = "application/json",
           CancellationToken token = default(CancellationToken),
           HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
           Dictionary<string, string> requestHeaders = null,
           string defaultScheme = "Bearer")
        {
            Response<Stream> response = new Response<Stream>();
            try
            {
                using (var responseMessage = await Execute(url, httpMethod, content, accessToken, mediaType, token,
                    completionOption, requestHeaders))
                {
                    response.IsSuccess = responseMessage.IsSuccessStatusCode;
                    response.StatusCode = responseMessage.StatusCode;
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var data = await responseMessage.Content.ReadAsStreamAsync();
                        response.Content = data;
                    }
                    else
                    {
                        response = await HandleErrors(responseMessage, response);
                    }
                }

            }
            catch (HttpRequestException e)
            {
                response.IsSuccess = false;
                response.ErrorMessage = e.Message;
            }
            return response;
        }


        protected virtual async Task<Response<T>> MakeRequest<T>(string url,
            HttpMethod httpMethod,
            HttpContent content = null,
            string accessToken = null,
            string mediaType = "application/json",
            CancellationToken token = default(CancellationToken),
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            Dictionary<string, string> requestHeaders = null,
            string defaultScheme = "Bearer")
        {
            Response<T> response = new Response<T>();
            try
            {
                using (var responseMessage = await Execute(url, httpMethod, content, accessToken, mediaType, token,
                    completionOption, requestHeaders))
                {
                    response.IsSuccess = responseMessage.IsSuccessStatusCode;
                    response.StatusCode = responseMessage.StatusCode;
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var data = await responseMessage.Content.ReadAsStringAsync();
                        response.Content = JsonConvert.DeserializeObject<T>(data);
                    }
                    else
                    {
                        response = await HandleErrors(responseMessage, response);
                    }
                }

            }
            catch (HttpRequestException e)
            {
                response.IsSuccess = false;
                response.ErrorMessage = e.Message;
            }
            return response;
        }

        protected virtual async Task<Response> MakeRequest(string url,
            HttpMethod httpMethod,
            HttpContent content = null,
            string accessToken = null,
            string mediaType = "application/json",
            CancellationToken token = default(CancellationToken),
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            Dictionary<string, string> requestHeaders = null,
            string defaultScheme = "Bearer")
        {
            Response response = new Response();
            try
            {
                using (var responseMessage = await Execute(url, httpMethod, content, accessToken, mediaType, token,
                    completionOption, requestHeaders))
                {
                    response.IsSuccess = responseMessage.IsSuccessStatusCode;
                    response.StatusCode = responseMessage.StatusCode;
                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        response = await HandleErrors(responseMessage, response);
                    }

                }
            }
            catch (HttpRequestException e)
            {
                response.IsSuccess = false;
                response.ErrorMessage = e.Message;
            }
            return response;
        }

        private Task<HttpResponseMessage> Execute(string url,
            HttpMethod httpMethod,
            HttpContent content = null,
            string accessToken = null,
            string mediaType = "application/json",
            CancellationToken token = default(CancellationToken),
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            Dictionary<string, string> requestHeaders = null,
            string defaultScheme = "Bearer")
        {
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            if (!string.IsNullOrEmpty(accessToken))
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(defaultScheme, accessToken);
            if (requestHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in requestHeaders)
                {
                    HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            return HttpClient.SendAsync(new HttpRequestMessage(httpMethod, url) { Content = content }, completionOption, token);

        }

        protected virtual async Task<T> HandleErrors<T>(HttpResponseMessage responseMessage, T response) where T : Response
        {
            var data = await responseMessage.Content.ReadAsStringAsync();
            response.ErrorMessage = data;
            return response;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        /// <seealso cref="M:System.IDisposable.Dispose()"/>
        public void Dispose() => this.Dispose(true);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        /// release only unmanaged resources.</param>
        internal virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    HttpClient?.Dispose();
                }
                _isDisposed = true;
            }
        }
    }
}
