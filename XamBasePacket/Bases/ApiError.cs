using Newtonsoft.Json;

namespace XamBasePacket.Bases
{
    public class ApiError
    {
        [JsonRequired]
        public string Message { get; set; }
    }
}
