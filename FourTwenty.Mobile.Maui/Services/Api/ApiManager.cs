using System.Collections.Concurrent;
using FourTwenty.Mobile.Maui.Exceptions;
using FourTwenty.Mobile.Maui.Interfaces.Api;
using FourTwenty.Mobile.Maui.Models.Base;
using FourTwenty.Mobile.Maui.Services.Api.shared;
using Microsoft.Extensions.Logging;

namespace FourTwenty.Mobile.Maui.Services.Api
{

    public abstract class ApiManager<T> : ApiManager
    {
        protected IApiService<T> ApiService { get; }

        public ApiManager(IApiService<T> apiService, ILogger logger) : base(logger)
        {
            ApiService = apiService;
        }
    }

    public abstract class ApiManager : SimpleApiManager, IDisposable
    {
        private bool _disposed;

        protected ConcurrentDictionary<int, CancellationTokenSource> RunningTasks = new ConcurrentDictionary<int, CancellationTokenSource>();
        protected bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;


        public ApiManager(ILogger logger)
        {
            Logger = logger;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        protected virtual void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet) return;

            //Cancel All Running Task
            var items = RunningTasks.ToList();
            foreach (var item in items)
            {
                item.Value.Cancel();
                if (RunningTasks.TryRemove(item.Key, out var cts))
                    cts.Dispose();
            }
        }

        protected override Task<IResponse> MakeRequest(Func<CancellationToken, Task> loadingFunction, CancellationToken cancellationToken)
        {
            CheckInternet();
            return base.MakeRequest(loadingFunction, cancellationToken);
        }

        protected override Task<IResponse<T>> MakeRequest<T>(Func<CancellationToken, Task<T>> loadingFunction, CancellationToken cancellationToken)
        {
            CheckInternet();
            return base.MakeRequest(loadingFunction, cancellationToken);
        }



        protected override async Task<TTResponse> MakeGenericRequest<TTResponse>(Func<CancellationToken, Task<TTResponse>> apiCallFunction, CancellationToken cancellationToken = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
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
