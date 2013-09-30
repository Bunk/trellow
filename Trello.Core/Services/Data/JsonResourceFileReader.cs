using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Strilanc.Value;

namespace Trellow.Services.Data
{
    [UsedImplicitly]
    public class JsonResourceFileReader : IFileReader
    {
        private readonly JsonConverter[] _converters;

        public JsonResourceFileReader()
        {
            _converters = new JsonConverter[]
            {
                new VersionConverter()
            };
        }

        public May<T> Read<T>(Uri uri)
        {
            var ri = Application.GetResourceStream(uri);
            if (ri == null)
                return May<T>.NoValue;

            using (var reader = new StreamReader(ri.Stream))
            {
                var content = reader.ReadToEnd();
                return Deserialize<T>(content);
            }
        }

        public IList<T> ReadList<T>(Uri uri)
        {
            var ri = Application.GetResourceStream(uri);
            if (ri == null)
                return new List<T>();

            using (var reader = new StreamReader(ri.Stream))
            {
                var content = reader.ReadToEnd();
                return Deserialize<List<T>>(content);
            }
        }

        private T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, _converters);
        }
    }
}