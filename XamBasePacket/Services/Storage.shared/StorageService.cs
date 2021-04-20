using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;
using Xamarin.Essentials;
using XamBasePacket.Extensions;
using XamBasePacket.Interfaces.Storage;

namespace XamBasePacket.Services.Storage
{
    /// <summary>
    /// Not allowed to store complex models or large JSON.
    /// </summary>
    public class StorageService : IStorageService
    {
        private readonly ConcurrentDictionary<string, object> _inMemoryCache = new ConcurrentDictionary<string, object>();

        public void Set<T>(string key, T value)
        {
            if (value == null)
            {
                _inMemoryCache.TryRemove(key, out _);
                Preferences.Remove(key);
                return;
            }
            _inMemoryCache[key] = value;
            if (typeof(T).IsSimpleType())
            {
                var typeCode = Convert.GetTypeCode(value);
                switch (typeCode)
                {
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        Preferences.Set(key, Convert.ToDouble(value));
                        break;
                    case TypeCode.Single:
                        Preferences.Set(key, Convert.ToSingle(value));
                        break;
                    case TypeCode.String:
                        Preferences.Set(key, Convert.ToString(value));
                        break;
                    case TypeCode.Boolean:
                        Preferences.Set(key, Convert.ToBoolean(value));
                        break;
                    case TypeCode.Int32:
                        Preferences.Set(key, Convert.ToInt32(value));
                        break;
                    case TypeCode.DateTime:
                        Preferences.Set(key, Convert.ToDateTime(value));
                        break;
                    case TypeCode.Int64:
                        Preferences.Set(key, Convert.ToInt64(value));
                        break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                Preferences.Set(key, JsonSerializer.Serialize(value));
            }
        }

        public T Get<T>(string key, T defaultValue = default)
        {
            if (_inMemoryCache.TryGetValue(key, out object cached))
                return (T)cached;

            T mapped = defaultValue;
            if (typeof(T).IsSimpleType())
            {
                var typeCode = Type.GetTypeCode(typeof(T));
                switch (typeCode)
                {
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        _inMemoryCache[key] = mapped = (T)(object)Preferences.Get(key, Convert.ToDouble(defaultValue));
                        break;
                    case TypeCode.Single:
                        _inMemoryCache[key] = mapped = (T)(object)Preferences.Get(key, Convert.ToSingle(defaultValue));
                        break;
                    case TypeCode.String:
                        _inMemoryCache[key] = mapped = (T)(object)Preferences.Get(key, Convert.ToString(defaultValue));
                        break;
                    case TypeCode.Boolean:
                        _inMemoryCache[key] = mapped = (T)(object)Preferences.Get(key, Convert.ToBoolean(defaultValue));
                        break;
                    case TypeCode.Int32:
                        _inMemoryCache[key] = mapped = (T)(object)Preferences.Get(key, Convert.ToInt32(defaultValue));
                        break;
                    case TypeCode.DateTime:
                        _inMemoryCache[key] = mapped = (T)(object)Preferences.Get(key, Convert.ToDateTime(defaultValue));
                        break;
                    case TypeCode.Int64:
                        _inMemoryCache[key] = mapped = (T)(object)Preferences.Get(key, Convert.ToInt64(defaultValue));
                        break;

                    default: throw new ArgumentOutOfRangeException();
                }

                return mapped;
            }

            var result = Preferences.Get(key, null);
            if (result != null)
            {
                mapped = JsonSerializer.Deserialize<T>(result);
                _inMemoryCache[key] = mapped;
                return mapped;
            }

            return mapped;
        }

        public Task Remove(string key)
        {
            _inMemoryCache.TryRemove(key, out _);
            Preferences.Remove(key);
            return Task.CompletedTask;
        }

        public Task Clear()
        {
            _inMemoryCache.Clear();
            Preferences.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> Contains(string key)
        {
            return Task.FromResult(Preferences.ContainsKey(key));
        }        
    }
}
