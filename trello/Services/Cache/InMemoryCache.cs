using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace trello.Services.Cache
{
    public class InMemoryCache : AbstractCache
    {
        protected Dictionary<string, CacheData> Cache = new Dictionary<string, CacheData>();

        public override bool Contains(string key)
        {
            return Cache.ContainsKey(key);
        }

        public override bool Expired(string key)
        {
            if (Contains(key))
            {
                return Cache[key].Expiration < DateTime.Now;
            }
            return false;
        }

        protected override T Retrieve<T>(string key)
        {
            return Cache[key].GetData<T>();
        }

        protected override void Store<T>(string key, T value)
        {
            Cache[key] = new CacheData(value);
        }

        public override Task<bool> Initialize()
        {
            return Task.Factory.StartNew(() => true);
        }

        protected class CacheData
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

            public CacheData()
            {
                /* for serializer */
            }

            public CacheData(object data)
            {
                Data = data;
                Expiration = DateTime.Now.AddDays(1);
            }
        }
    }
}