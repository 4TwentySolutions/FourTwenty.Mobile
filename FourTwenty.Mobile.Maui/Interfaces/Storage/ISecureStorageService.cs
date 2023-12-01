namespace FourTwenty.Mobile.Maui.Interfaces.Storage
{
    public interface ISecureStorageService : IStorageAsyncService
    {
         Task<bool> Contains<T>(string key);
    }
}
