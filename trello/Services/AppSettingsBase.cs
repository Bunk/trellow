using System.IO.IsolatedStorage;

namespace trello.Services
{
    public abstract class AppSettingsBase
    {
        private static readonly object Sync = new object();

        private static readonly IsolatedStorageSettings Settings = IsolatedStorageSettings.ApplicationSettings;

        protected static bool Exists(string key)
        {
            return Settings.Contains(key);
        }

        protected static T GetOrDefault<T>(string key)
        {
            if (Exists(key))
                lock(Sync)
                    if (Exists(key))
                    {
                        return (T) (Settings[key] ?? default(T));
                    }

            return default(T);
        }

        protected static void Set<T>(string key, T value)
        {
            Settings[key] = value;

            lock(Sync)
                Settings.Save();
        }
    }
}