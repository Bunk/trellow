using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RestSharp;
using TrelloNet;
using TrelloNet.Internal;
using trellow.api.OAuth;

namespace trello.Services
{
    [UsedImplicitly]
    public class JsonFileRestClient : IRequestClient
    {
        private const string Rootpath = "SampleData";

        public Task<Uri> GetAuthorizationUri(string applicationName, Scope scope, Expiration expiration, Uri callbackUri = null)
        {
            throw new NotSupportedException("Should never try to get the authorization url when disconnected.");
        }

        public Task<OAuthToken> Verify(string verifier)
        {
            return new Task<OAuthToken>(() => new OAuthToken());
        }

        public void Authorize(OAuthToken accessToken)
        {
            // noop
        }

        public void Deauthorize()
        {
            // noop
        }

        public Task<IRestResponse> RequestAsync(IRestRequest request)
        {
            return Task.Factory.StartNew(() => (IRestResponse) new RestResponse());
        }

        public async Task<T> RequestAsync<T>(IRestRequest request) where T : class, new()
        {
            var filename = ParseFilename(request);
            if (filename != null)
                return await ReadFromFile<T>(filename);
            return default(T);
        }

        public async Task<IEnumerable<T>> RequestListAsync<T>(IRestRequest request)
        {
            var filename = ParseFilename(request);
            if (filename != null)
                return await ReadListFromFile<T>(filename);
            return Enumerable.Empty<T>();
        }

        private static string ParseFilename(IRestRequest request)
        {
            var resource = request.Resource;
            if (resource.EndsWith("/"))
                resource = resource.Substring(0, resource.Length - 1);

            foreach (var parm in request.Parameters.Where(p => p.Type == ParameterType.UrlSegment))
            {
                var placeholder = "{" + parm.Name + "}";
                resource = resource.Replace(placeholder, parm.Value.ToString());
            }

            var split = resource.Split('/');
            if (split.Length == 3) // ie, "members/me/boards" -- typically a list
            {
                return string.Format("{0}\\{1}\\{2}-{3}.json", //SampleData\members\me-boards.json
                                     Rootpath,
                                     split[0],
                                     split[1],
                                     split[2]);
            }
            if (split.Length == 2) // ie, "members/me" -- typically a single object
            {
                return string.Format("{0}\\{1}\\{2}.json", //SampleData\members\me.json
                                     Rootpath,
                                     split[0],
                                     split[1]);
            }
            return null;
        }

        private static async Task<T> ReadFromFile<T>(string filename)
        {
            if (!File.Exists(filename))
                return default(T);

            using (var stream = File.OpenText(filename))
            {
                var content = await stream.ReadToEndAsync();
                return Deserialize<T>(content);
            }
        }

        private static async Task<IEnumerable<T>> ReadListFromFile<T>(string filename)
        {
            if (!File.Exists(filename))
                return Enumerable.Empty<T>();

            using (var stream = File.OpenText(filename))
            {
                var content = await stream.ReadToEndAsync();
                return Deserialize<List<T>>(content);
            }
        }

        private static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, new NotificationConverter(), new ActionConverter());
        }
    }
}
