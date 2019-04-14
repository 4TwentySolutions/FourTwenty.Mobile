using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using XamBasePacket.Bases;

namespace XamBasePacket.Helpers
{
    public class ResponseStatusRule
    {
        public Action<IResponse, ViewModelBase> RuleExecution { get; set; }
        public HttpStatusCode Code { get; set; }
    }

    public static class TaskWrapperHelper
    {
        public static List<ResponseStatusRule> ResponseStatusRules { get; } = new List<ResponseStatusRule>();


        public static async Task<T> WrapApiTaskDefault<T>(this Task<T> task, ViewModelBase viewModel, bool displayError = true) where T : class, IResponse
        {
            return await task.WrapTaskWithApiResponse(viewModel).WrapTaskWithLoading(viewModel).WrapWithExceptionHandling(viewModel, displayError);
        }
        public static async Task<T> WrapTaskDefault<T>(this Task<T> task, ViewModelBase viewModel, bool displayError = true)
        {
            return await task.WrapTaskWithLoading(viewModel).WrapWithExceptionHandling(viewModel, displayError);
        }
        public static async Task WrapTaskDefault(this Task task, ViewModelBase viewModel, bool displayError = true)
        {
            await task.WrapTaskWithLoading(viewModel).WrapWithExceptionHandling(viewModel, displayError);
        }

        public static async Task<T> WrapTaskWithApiResponse<T>(this Task<T> task, ViewModelBase viewModel) where T : class, IResponse
        {
            T response = await task;
            if (response == null)
                return null;
            if (response.IsSuccess)
                return response;
            return HandleErrors(response, ref viewModel);
        }

        public static async Task<T> WrapTaskWithLoading<T>(this Task<T> task, ViewModelBase viewModel)
        {
            try
            {
                viewModel.IsBusy = true;
                return await task;
            }
            finally
            {
                viewModel.IsBusy = false;
            }
        }

        public static async Task WrapTaskWithLoading(this Task task, ViewModelBase viewModel)
        {
            try
            {
                viewModel.IsBusy = true;
                await task;
            }
            finally
            {
                viewModel.IsBusy = false;
            }
        }
        public static async Task WrapWithExceptionHandling(this Task task, ViewModelBase viewModel, bool displayError = true)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                // do nothing, this is an expected behaviour
            }
            catch (Exception e)
            {
                viewModel.DisplayError(e, displayError);
            }
        }
        public static async Task<T> WrapWithExceptionHandling<T>(this Task<T> task, ViewModelBase viewModel, bool displayError = true)
        {
            try
            {
                return await task;
            }
            catch (OperationCanceledException)
            {
                // do nothing, this is an expected behaviour
            }
            catch (Exception e)
            {
                viewModel.DisplayError(e, displayError);
            }
            return default;
        }

        private static T HandleErrors<T>(T response, ref ViewModelBase viewModel) where T : IResponse
        {
            if (ResponseStatusRules.Any(x => x.Code == response.StatusCode))
            {
                ResponseStatusRules.Find(rule => rule.Code == response.StatusCode)?.RuleExecution
                    ?.Invoke(response, viewModel);
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    viewModel.UnauthorizedApiCall();
                    return response;
                }
            }

            return response;
        }

    }
}
