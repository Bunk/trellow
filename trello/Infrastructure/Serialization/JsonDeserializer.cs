using System;
using Newtonsoft.Json;

namespace trello.Infrastructure.Serialization
{
//    public class JsonDeserializer : IDeserializer
//    {
//        private readonly JsonSerializerSettings _settings;
//
//        public JsonDeserializer(JsonSerializerSettings settings = null)
//        {
//            _settings = settings ?? new JsonSerializerSettings
//            {
//                MissingMemberHandling = MissingMemberHandling.Ignore,
//                NullValueHandling = NullValueHandling.Ignore,
//                PreserveReferencesHandling = PreserveReferencesHandling.All
//            };
//        }
//
//        public object Deserialize(RestResponseBase response, Type type)
//        {
//            return JsonConvert.DeserializeObject(response.Content, type, _settings);
//        }
//
//        public T Deserialize<T>(RestResponseBase response)
//        {
//            return JsonConvert.DeserializeObject<T>(response.Content, _settings);
//        }
//    }
}
