using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XamBasePacket.Bases;
using System.IO;
using System.Net;

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

        protected virtual async Task<IResponse<T>> MakeRequest<T>(string url,
            HttpMethod httpMethod,
            HttpContent content = null,
            string accessToken = null,
            string mediaType = "application/json",
            CancellationToken token = default(CancellationToken),
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            Dictionary<string, string> requestHeaders = null,
            string defaultScheme = "Bearer")
        {
            IResponse<T> response = CreateResponse<T>();
            try
            {
                using (var responseMessage = await Execute(url, httpMethod, content, accessToken, mediaType, token,
                    completionOption, requestHeaders))
                {
                    response.IsSuccess = responseMessage.IsSuccessStatusCode;
                    response.StatusCode = responseMessage.StatusCode;
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        if (typeof(T) != typeof(Stream) && !typeof(T).IsSubclassOf(typeof(Stream)))
                        {
                            try
                            {
                                var data = await responseMessage.Content.ReadAsStringAsync();
                                response.RawContent = data;
                                response.Content = JsonConvert.DeserializeObject<T>(data);
                            }
                            catch (JsonException ex)
                            {
                                response.ErrorMessage = ex.Message;
                                response.IsSuccess = false;
                            }
                        }
                        else
                        {
                            object data = await responseMessage.Content.ReadAsStreamAsync();
                            response.Content = (T)data;
                        }
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
                response.Error = e;
            }
            catch (WebException e)
            {
                response.IsSuccess = false;
                response.ErrorMessage = e.Message;
                response.Error = e;
            }
            return response;
        }

        protected virtual async Task<IResponse> MakeRequest(string url,
            HttpMethod httpMethod,
            HttpContent content = null,
            string accessToken = null,
            string mediaType = "application/json",
            CancellationToken token = default(CancellationToken),
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
            Dictionary<string, string> requestHeaders = null,
            string defaultScheme = "Bearer")
        {
            IResponse response = CreateResponse();
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
                response.Error = e;
            }
            catch (WebException e)
            {
                response.IsSuccess = false;
                response.ErrorMessage = e.Message;
                response.Error = e;
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

        protected virtual async Task<T> HandleErrors<T>(HttpResponseMessage responseMessage, T response) where T : IResponse
        {
            var data = await responseMessage.Content.ReadAsStringAsync();
            response.ErrorMessage = data;
            return response;
        }

        protected virtual IResponse CreateResponse()
        {
            return new Response();
        }

        protected virtual IResponse<T> CreateResponse<T>()
        {
            return new Response<T>();
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
