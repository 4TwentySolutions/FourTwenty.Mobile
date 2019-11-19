using Fusillade;

namespace XamBasePacket.Services.Api
{
    public interface IApiService<out T>
    {
        T GetApi(Priority priority);
    }
}
