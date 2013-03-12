using System.Threading.Tasks;
using ServiceStack.Text;
using Strilanc.Value;

namespace trello.Services
{
    public interface ICache
    {
        bool Contains(string key);

        bool Expired(string key);

        May<T> Get<T>(string key);

        May<T> Set<T>(string key, May<T> value);

        Task<bool> Initialize();
    }

    public abstract class AbstractCache : ICache
    {
        public abstract bool Contains(string key);

        public abstract bool Expired(string key);

        protected abstract T Retrieve<T>(string key);

        protected abstract void Store<T>(string key, T value);

        public May<T> Get<T>(string key)
        {
            return Contains(key)
                       ? Retrieve<T>(key)
                       : May<T>.NoValue;
        }

        public May<T> Set<T>(string key, May<T> value)
        {
            value.IfHasValueThenDo(x =>
            {
                var existing = Get<T>(key);
                if (!existing.HasValue || IsDifferent(existing.ForceGetValue(), x))
                    Store(key, x);
            });

            return value;
        }

        public virtual Task<bool> Initialize()
        {
            return Task.Factory.StartNew(() => true);
        }

        protected virtual bool IsDifferent(object lhs, object rhs)
        {
            var jsvLhs = lhs.ToJsv();
            var jsvRhs = rhs.ToJsv();
            return !jsvLhs.Equals(jsvRhs);
        }
    }
}