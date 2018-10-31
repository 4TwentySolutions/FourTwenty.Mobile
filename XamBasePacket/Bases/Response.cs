using System.Net;
using System.Net.Http;

namespace XamBasePacket.Bases
{
    public class Response<T> : Response
    {
        public T Content { get; set; }

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

    public class Response
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
