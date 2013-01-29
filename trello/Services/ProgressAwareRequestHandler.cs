using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json;
using RestSharp;
using Strilanc.Value;
using Windows.Storage;
using trello.ViewModels;
using ServiceStack.Text;
using trellow.api.Data;

namespace trello.Services
{
    public class ProgressAwareRequestHandler : IHandleRequests
    {
        private readonly IProgressService _progressService;

        public ProgressAwareRequestHandler(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public RequestContext<T>  BeforeRequest<T>(RequestContext<T> context)
        {
            _progressService.Show();
            return context;
        }

        public ResponseContext<T>  AfterRequest<T>(ResponseContext<T> context)
        {
            _progressService.Hide();
            return context;
        }
    }

    public interface ICache
    {
        bool Contains(string key);

        May<T> Get<T>(string key);

        void Set<T>(string key, May<T> value);

        Task<bool> Initialize();
    }

    public class CacheAwareRequestHandler : IHandleRequests
    {
        private readonly ICache _cache;

        public CacheAwareRequestHandler(ICache cache)
        {
            _cache = cache;
        }

        public RequestContext<T>  BeforeRequest<T>(RequestContext<T> context)
        {
            // GET requests only
            if (context.Request.Method != Method.GET)
                return context;

            var contains = _cache.Contains(context.RequestUri.PathAndQuery);
            if (!contains)
                return context;

            context.Data = _cache.Get<T>(context.RequestUri.PathAndQuery);

            return context;
        }

        public ResponseContext<T> AfterRequest<T>(ResponseContext<T> context)
        {
            if (context.Request.Method != Method.GET)
                return context;

            if (context.Data.HasValue)
            {
                var key = context.RequestUri.PathAndQuery;
                _cache.Set(key, context.Data);
            }

            return context;
        }
    }

    public class FileSystemCache : ICache
    {
        public const string Filename = "TrellowCache.json";

        private readonly StorageFolder _localFolder;
        private Dictionary<string, CacheData> _memoryCache;

        public FileSystemCache(IPhoneService phoneService)
        {
            _localFolder = ApplicationData.Current.LocalFolder;

            phoneService.Deactivated += (sender, args) => SaveToDisk();
            phoneService.Closing += (sender, args) => SaveToDisk();
        }

        public bool Contains(string key)
        {
            return _memoryCache.ContainsKey(key);
        }

        public May<T> Get<T>(string key)
        {
            var cache = _memoryCache;
            if (cache.ContainsKey(key))
            {
                var data = cache[key];
                return data.GetData<T>();
            }

            return May<T>.NoValue;
        }

        public void Set<T>(string key, May<T> value)
        {
            value.IfHasValueThenDo(x =>
            {
                _memoryCache[key] = new CacheData
                {
                    Data = x,
                    Expiration = DateTime.Now.AddDays(1)
                };
            });
        }

        public async Task<bool> Initialize()
        {
            return await LoadFromDisk();
        }

        private async Task<bool> LoadFromDisk()
        {
            try
            {
                using (var stream = await _localFolder.OpenStreamForReadAsync(Filename))
                {
                    if (stream.Length == 0)
                    {
                        _memoryCache = new Dictionary<string, CacheData>();
                        return false;
                    }

                    _memoryCache = TypeSerializer.DeserializeFromStream<Dictionary<string, CacheData>>(stream);
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                _memoryCache = new Dictionary<string, CacheData>();
                return false;
            }
            catch (Exception)
            {
                // Maybe want to flag here, but 
                _memoryCache = new Dictionary<string, CacheData>();
                return false;
            }
        }

        private async void SaveToDisk()
        {
            var jsv = TypeSerializer.SerializeToString(_memoryCache);
            var bytes = Encoding.UTF8.GetBytes(jsv);

            var file = await _localFolder.CreateFileAsync(Filename, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                stream.Write(bytes, 0, bytes.Length);
                await stream.FlushAsync();
            }
        }

        private class CacheData
        {
            public DateTime Expiration { get; set; }

            public object Data { get; set; }

            public T GetData<T>()
            {
                var value = Data as string;
                if (value != null)
                {
                    return TypeSerializer.DeserializeFromString<T>(value);
                }

                return (T) Data;
            }
        }
    }
}