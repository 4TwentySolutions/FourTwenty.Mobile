using System.Threading.Tasks;

namespace XamBasePacket.Interfaces.Storage
{
    public interface ISecureStorageService : IStorageAsyncService
    {
         Task<bool> Contains<T>(string key);
    }
}
