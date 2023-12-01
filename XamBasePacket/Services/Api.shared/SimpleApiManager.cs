using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Refit;
using XamBasePacket.Bases;
using XamBasePacket.Interfaces.Api;

namespace XamBasePacket.Services.Api.shared
{
    public class SimpleApiManager : IApiManager
    {
        protected ILogger Logger;

        protected virtual async Task<IResponse> MakeRequest(Func<CancellationToken, Task> loadingFunction, CancellationToken cancellationToken)
        {
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

        protected virtual async Task<TTResponse> MakeGenericRequest<TTResponse>(Func<CancellationToken, Task<TTResponse>> apiCallFunction, CancellationToken cancellationToken = default)
            where TTResponse : IResponse
        {
            
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var task = apiCallFunction.Invoke(cts.Token);
            try
            {
                Logger?.LogInformation($"{nameof(ApiManager)} - Task run with ID: {task.Id}");
                var result = await task;
                return result;
            }
            finally
            {
                Logger?.LogInformation($"{nameof(ApiManager)} - Task with ID: {task.Id} removed from list");
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
                .Or<ApiException>()
                .Or<HttpRequestException>()
                .Or<OperationCanceledException>()
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
    }
}
