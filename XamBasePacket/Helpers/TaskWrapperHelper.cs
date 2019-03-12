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
        public Action<IResponse, ViewModelBase, bool> RuleExecution { get; set; }
        public HttpStatusCode Code { get; set; }
    }

    public static class TaskWrapperHelper
    {
        public static List<ResponseStatusRule> ResponseStatusRules { get; } = new List<ResponseStatusRule>();

        public static async Task<IResponse<T>> WrapTaskWithApiResponse<T>(this Task<IResponse<T>> task, ViewModelBase viewModel, bool displayError = false)
        {
            IResponse<T> response = null;
            try
            {
                response = await task;
            }
            catch (Exception ex)
            {
                if (displayError)
                    viewModel.ErrorText = ex.ToString();
            }

            if (response == null)
                return null;
            if (response.IsSuccess)
                return response;


            return HandleErrors(response, ref viewModel, displayError);
        }

        public static async Task<IResponse> WrapTaskWithApiResponse(this Task<IResponse> task, ViewModelBase viewModel, bool displayError = false)
        {
            IResponse response = null;
            try
            {
                response = await task;
            }
            catch (Exception ex)
            {
                if (displayError)
                    viewModel.ErrorText = ex.ToString();
            }

            if (response == null)
                return null;
            if (response.IsSuccess)
                return response;


            return HandleErrors(response, ref viewModel, displayError);
        }


        private static T HandleErrors<T>(T response, ref ViewModelBase viewModel, bool displayError) where T : IResponse
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                viewModel.UnauthorizedApiCall();
                return response;
            }

            if (ResponseStatusRules.Any(x => x.Code == response.StatusCode))
            {
                ResponseStatusRules.Find(rule => rule.Code == response.StatusCode)?.RuleExecution
                    ?.Invoke(response, viewModel, displayError);
            }
            else
            {
                if (displayError)
                    viewModel.DisplayError(response.ErrorMessage);
            }

            return response;
        }

    }
}
