using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Refit;
using Xamarin.Essentials;
using XamBasePacket.Bases;
using XamBasePacket.Exceptions;
using XamBasePacket.Interfaces.Api;

namespace XamBasePacket.Services.Api
{

    public class ApiManager<T> : ApiManager
    {
        protected IApiService<T> ApiService { get; }

        public ApiManager(IApiService<T> apiService, ILogger logger) : base(logger)
        {
            ApiService = apiService;
        }
    }

    public class ApiManager : IApiManager, IDisposable
    {
        private bool _disposed;

        protected ConcurrentDictionary<int, CancellationTokenSource> RunningTasks = new ConcurrentDictionary<int, CancellationTokenSource>();
        protected bool IsConnected => Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet;
        protected ILogger Logger;


        public ApiManager(ILogger logger)
        {
            Logger = logger;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        protected virtual void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet) return;

            //Cancel All Running Task
            var items = RunningTasks.ToList();
            foreach (var item in items)
            {
                item.Value.Cancel();
                if (RunningTasks.TryRemove(item.Key, out var cts))
                    cts.Dispose();
            }
        }

        protected virtual async Task<IResponse> MakeRequest(Func<CancellationToken, Task> loadingFunction, CancellationToken cancellationToken)
        {
            CheckInternet();
            IResponse response = CreateResponse();
            return await ExecuteRequest(response, async () =>
            {
                await SetUpPolicy()
                    .ExecuteAsync(loadingFunction, cancellationToken);
                response.IsSuccess = true;
                return response;
            });

        }

        protected virtual async Task<IResponse<T>> MakeRequest<T>(Func<CancellationToken, Task<T>> loadingFunction, CancellationToken cancellationToken)
        {
            CheckInternet();
            IResponse<T> response = CreateResponse<T>();

            return await ExecuteRequest(response, async () =>
            {
                response.Content = await SetUpPolicy()
                   .ExecuteAsync(loadingFunction, cancellationToken);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return response;
            });
        }

        protected virtual async Task<TTResponse> MakeGenericRequest<TTResponse>(Func<CancellationToken, Task<TTResponse>> apiCallFunction)
            where TTResponse : IResponse
        {
            using (var cts = new CancellationTokenSource())
            {

                var task = apiCallFunction.Invoke(cts.Token);
                try
                {
                    Logger?.LogInformation($"{nameof(ApiManager)} - Task run with ID: {task.Id}");
                    RunningTasks.TryAdd(task.Id, cts);
                    var result = await task;
                    return result;
                }
                finally
                {

                    Logger?.LogInformation($"{nameof(ApiManager)} - Task with ID: {task.Id} removed from list");
                    RunningTasks.TryRemove(task.Id, out _);
                }
            }
        }

        protected virtual async Task<T> ExecuteRequest<T>(T response, Func<Task<T>> funcToExecute) where T : IResponse
        {
            try
            {
                return await funcToExecute.Invoke();
            }
            catch (ApiException ex)
            {
                Logger?.LogError(ex, ex.Message);
                response.Error = ex;
                response.ErrorMessage = ex.Content;
                response.StatusCode = ex.StatusCode;
                response.IsSuccess = false;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, ex.Message);
                response.Error = ex;
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        protected virtual AsyncPolicy SetUpPolicy()
        {

            return Policy
                .Handle<WebException>()
                .Or<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        protected virtual IResponse CreateResponse()
        {
            return new Response();
        }

        protected virtual IResponse<T> CreateResponse<T>()
        {
            return new Response<T>();
        }


        protected virtual void CheckInternet()
        {
            Logger?.LogInformation($"{nameof(ApiManager)} - Internet is available: {IsConnected}");
            if (!IsConnected)
            {
                throw new NoInternetException();
            }
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
            try
            {
                Logger?.LogInformation($"{nameof(Dispose)} | {GetType().Name}, {nameof(disposing)}={disposing}");
                if (_disposed)
                    return;
                if (disposing)
                {

                    Logger?.LogInformation($"{nameof(RunningTasks)}.{nameof(RunningTasks.Count)} = {RunningTasks.Count}");
                    foreach (var item in RunningTasks)
                    {
                        item.Value.Cancel();
                    }
                    RunningTasks.Clear();
                    // Free any other managed objects here.
                    Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
                }
                // Free any unmanaged objects here.            
                _disposed = true;
            }
            finally
            {
                Logger?.LogInformation($"{nameof(Dispose)} Ended | {GetType().Name}, {nameof(disposing)}={disposing}");
            }
        }

        ~ApiManager()
        {
            Dispose(false);
        }
        #endregion
    }
}
