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

        #region Func helpers
        public static async Task<T> WrapApiTaskDefault<T>(this Func<Task<T>> task, ViewModelBase viewModel, bool displayError = true) where T : class, IResponse
        {
            try
            {
                viewModel.ShowLoader();
                T response = await task.Invoke();
                if (response == null)
                    return null;
                if (response.IsSuccess)
                    return response;
                return HandleErrors(response, viewModel, displayError);
            }
            catch (Exception e)
            {
                viewModel.DisplayError(e, null, displayError);
                return default;
            }
            finally
            {
                viewModel.HideLoader();
            }

        }

        public static async Task<T> WrapTaskWithApiResponse<T>(this Func<Task<T>> task, ViewModelBase viewModel, bool displayApiError = true) where T : class, IResponse
        {
            T response = await task.Invoke();
            if (response == null)
                return null;
            if (response.IsSuccess)
                return response;
            return HandleErrors(response, viewModel, displayApiError);
        }


        public static async Task WrapTaskDefault(this Func<Task> task, ViewModelBase viewModel, bool displayError = true)
        {
            try
            {
                viewModel.ShowLoader();
                await task.Invoke();
            }
            catch (Exception e)
            {
                viewModel.DisplayError(e, null, displayError);
            }
            finally
            {
                viewModel.HideLoader();
            }

        }
        public static async Task<T> WrapTaskDefault<T>(this Func<Task<T>> task, ViewModelBase viewModel, bool displayError = true)
        {
            try
            {
                viewModel.ShowLoader();
                return await task.Invoke();
            }
            catch (Exception e)
            {
                viewModel.DisplayError(e, null, displayError);
            }
            finally
            {
                viewModel.HideLoader();
            }

            return default;
        }


        public static async Task<T> WrapTaskWithLoading<T>(this Func<Task<T>> task, ViewModelBase viewModel)
        {
            try
            {
                viewModel.ShowLoader();
                return await task.Invoke();
            }
            finally
            {
                viewModel.HideLoader();
            }
        }

        public static async Task WrapTaskWithLoading(this Func<Task> task, ViewModelBase viewModel)
        {
            try
            {
                viewModel.ShowLoader();
                await task.Invoke();
            }
            finally
            {
                viewModel.HideLoader();
            }
        }

        public static async Task WrapWithExceptionHandling(this Func<Task> task, ViewModelBase viewModel, bool displayError = true)
        {
            try
            {
                await task.Invoke();
            }
            catch (OperationCanceledException)
            {
                // do nothing, this is an expected behaviour
            }
            catch (Exception e)
            {
                viewModel.DisplayError(e, null, displayError);
            }
        }
        public static async Task<T> WrapWithExceptionHandling<T>(this Func<Task<T>> task, ViewModelBase viewModel, bool displayError = true)
        {
            try
            {
                return await task.Invoke();
            }
            catch (OperationCanceledException)
            {
                // do nothing, this is an expected behaviour
            }
            catch (Exception e)
            {
                viewModel.DisplayError(e, null, displayError);
            }
            return default;
        }

        #endregion



        public static async Task WrapTaskDefault(this Task task, ViewModelBase viewModel, bool displayError = true)
        {
            await task.WrapTaskWithLoading(viewModel).WrapWithExceptionHandling(viewModel, displayError);
        }

        public static async Task<T> WrapApiTaskDefault<T>(this Task<T> task, ViewModelBase viewModel, bool displayError = true) where T : class, IResponse
        {
            return await task.WrapTaskWithApiResponse(viewModel).WrapTaskWithLoading(viewModel).WrapWithExceptionHandling(viewModel, displayError);
        }
        public static async Task<T> WrapTaskDefault<T>(this Task<T> task, ViewModelBase viewModel, bool displayError = true)
        {
            return await task.WrapTaskWithLoading(viewModel).WrapWithExceptionHandling(viewModel, displayError);
        }


        public static async Task<T> WrapTaskWithApiResponse<T>(this Task<T> task, ViewModelBase viewModel, bool displayApiError = true) where T : class, IResponse
        {
            T response = await task;
            if (response == null)
                return null;
            if (response.IsSuccess)
                return response;
            return HandleErrors(response, viewModel, displayApiError);
        }

        public static async Task<T> WrapTaskWithLoading<T>(this Task<T> task, ViewModelBase viewModel)
        {
            try
            {
                viewModel.ShowLoader();
                return await task;
            }
            finally
            {
                viewModel.HideLoader();
            }
        }

        public static async Task WrapTaskWithLoading(this Task task, ViewModelBase viewModel)
        {
            try
            {
                viewModel.ShowLoader();
                await task;
            }
            finally
            {
                viewModel.HideLoader();
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
                viewModel.DisplayError(e, null, displayError);
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
                viewModel.DisplayError(e, null, displayError);
            }
            return default;
        }

        private static T HandleErrors<T>(T response, ViewModelBase viewModel, bool displayError = true) where T : IResponse
        {
            if (ResponseStatusRules.Any(x => x.Code == response.StatusCode))
            {
                ResponseStatusRules.Find(rule => rule.Code == response.StatusCode)?.RuleExecution
                    ?.Invoke(response, viewModel);
            }
            viewModel.DisplayError(response.Error, response.ErrorMessage, displayError);
            return response;
        }

    }
}
