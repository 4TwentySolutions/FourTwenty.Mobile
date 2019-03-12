using System.Net;
using System.Net.Http;

namespace XamBasePacket.Bases
{
    public interface IResponse
    {
        bool IsSuccess { get; set; }
        string ErrorMessage { get; set; }
        HttpStatusCode StatusCode { get; set; }
    }

    public interface IResponse<T> : IResponse
    {
        T Content { get; set; }
        string RawContent { get; set; }
    }

    public class Response<T> : Response, IResponse<T>
    {
        public T Content { get; set; }
        public string RawContent { get; set; }

        public Response()
        {

        }

        public Response(bool isSuccess) : base(isSuccess)
        {

        }

        public Response(HttpResponseMessage response) : base(response)
        {
        }

    }

    public class Response : IResponse
    {
        public Response()
        {

        }

        public Response(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public Response(HttpResponseMessage response)
        {
            IsSuccess = response.IsSuccessStatusCode;
            StatusCode = response.StatusCode;
        }


        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
