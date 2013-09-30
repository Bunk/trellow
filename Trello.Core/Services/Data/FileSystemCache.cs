using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Caliburn.Micro;
using JetBrains.Annotations;
using ServiceStack.Text;

namespace Trellow.Services.Data
{
    [UsedImplicitly]
    public class FileSystemCache : InMemoryCache
    {
        private const string Filename = "TrellowCache.json";

        public FileSystemCache(IPhoneService phoneService)
        {
            phoneService.Deactivated += (sender, args) => SaveToDisk();
            phoneService.Closing += (sender, args) => SaveToDisk();
        }

        public override Task<bool> Initialize()
        {
            return Task.Factory.StartNew(() => LoadFromDisk());
        }

        private bool LoadFromDisk()
        {
            using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (iso.FileExists(Filename))
                {
                    using (var stream = iso.OpenFile(Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var deserialized =
                            TypeSerializer.DeserializeFromStream<Dictionary<string, CacheData>>(stream);
                        if (deserialized != null)
                        {
                            Cache = deserialized;
                            return true;
                        }
                    }
                }
            }

            Cache = new Dictionary<string, CacheData>();
            return false;
        }

        private void SaveToDisk()
        {
            using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    if (iso.FileExists(Filename))
                    {
                        iso.DeleteFile(Filename);
                    }

                    using (var stream = iso.CreateFile(Filename))
                    {
                        TypeSerializer.SerializeToStream(Cache, stream);
                    }
                }
                catch (Exception)
                {
                    // todo: Log this somewhere...
                }
            }
        }
    }
}