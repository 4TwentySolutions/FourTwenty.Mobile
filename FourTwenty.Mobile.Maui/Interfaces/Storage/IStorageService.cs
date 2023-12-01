namespace FourTwenty.Mobile.Maui.Interfaces.Storage
{
    public interface IGenericStorage
    {
        Task Remove(string key);
        Task Clear();
        Task<bool> Contains(string key);
    }
    public interface IStorageService : IGenericStorage
    {
        void Set<T>(string key, T value);
        T Get<T>(string key, T? defaultValue = default);

    }

    public interface IStorageAsyncService : IGenericStorage
    {
        Task SetAsync<T>(string key, T value);
        Task<T> GetAsync<T>(string key, T? defaultValue = default);

    }
}
