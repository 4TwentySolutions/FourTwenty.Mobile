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
        public Func<Response, ViewModelBase, bool, Response> RuleExecution { get; set; }
        public HttpStatusCode Code { get; set; }
    }

    public static class TaskWrapperHelper
    {
        public static List<ResponseStatusRule> ResponseStatusRules { get; } = new List<ResponseStatusRule>();

        public static async Task<Response<T>> WrapTaskWithApiResponse<T>(this Task<Response<T>> task, ViewModelBase viewModel, bool displayError = false)
        {
            Response<T> response = null;
            try
            {
                response = await task;
            }
            catch (Exception ex)
            {
                if (displayError)
                    viewModel.ErrorText = ex.ToString();
            }
            //response = await task;
            if (response == null)
                return CreateDefaultResponse<Response<T>>();
            if (response.IsSuccess)
                return response;


            return HandleErrors(response, ref viewModel, displayError);
        }

        public static async Task<Response> WrapTaskWithApiResponse(this Task<Response> task, ViewModelBase viewModel, bool displayError = false)
        {
            Response response = null;
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
                return CreateDefaultResponse<Response>();
            if (response.IsSuccess)
                return response;


            return HandleErrors(response, ref viewModel, displayError);
        }

        private static T CreateDefaultResponse<T>() where T : new()
        {
            return new T();
        }


        private static T HandleErrors<T>(T response, ref ViewModelBase viewModel, bool displayError) where T : Response
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
