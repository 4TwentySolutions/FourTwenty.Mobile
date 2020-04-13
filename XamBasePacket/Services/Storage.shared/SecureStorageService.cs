using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;
using XamBasePacket.Extensions;
using XamBasePacket.Interfaces.Storage;

namespace XamBasePacket.Services.Storage
{
    /// <summary>
    /// Secure storage in keychain. Not allowed to store complex models or large JSON.
    /// </summary>
    public class SecureStorageService : ISecureStorageService
    {
        private readonly ConcurrentDictionary<string, object> _inMemoryCache = new ConcurrentDictionary<string, object>();

        public async Task SetAsync<T>(string key, T value)
        {
            if (value == null)
            {
                _inMemoryCache.TryRemove(key, out _);
                SecureStorage.Remove(key);
                return;
            }
            _inMemoryCache[key] = value;
            await SecureStorage.SetAsync(key, typeof(T).IsSimpleType() ? value.ToString() : JsonConvert.SerializeObject(value));
        }

        public async Task<T> GetAsync<T>(string key, T defaultValue = default)
        {
            if (_inMemoryCache.TryGetValue(key, out object cached))
                return (T)cached;

            var result = await SecureStorage.GetAsync(key);
            if (string.IsNullOrWhiteSpace(result))
                return defaultValue;
            T mapped;

            if (typeof(T).IsSimpleType())
            {
                mapped = (T)Convert.ChangeType(result, typeof(T));
                _inMemoryCache[key] = mapped;
                return mapped;
            }

            mapped = JsonConvert.DeserializeObject<T>(result);
            if (mapped != null)
            {
                _inMemoryCache[key] = mapped;
                return mapped;
            }

            return defaultValue;
        }

        public Task Remove(string key)
        {
            _inMemoryCache.TryRemove(key, out _);
            SecureStorage.Remove(key);
            return Task.CompletedTask;
        }

        public Task Clear()
        {
            _inMemoryCache.Clear();
            SecureStorage.RemoveAll();
            return Task.CompletedTask;
        }

        public Task<bool> Contains(string key)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> Contains<T>(string key)
        {
            var item = await GetAsync<T>(key);
            return item != null;
        }
    }
}
