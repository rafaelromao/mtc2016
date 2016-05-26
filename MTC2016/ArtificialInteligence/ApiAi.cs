using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MTC2016.ArtificialInteligence
{
    public abstract class ApiAi : IApiAi, IDisposable
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        const string MediaType = "application/json";
        private readonly HttpClient _httpClient;
        private readonly ObjectCache _cache = new MemoryCache(nameof(MTC2016));

        public Settings Settings { get; }

        protected ApiAi(Settings settings, string apiAiDeveloperApiKey)
        {
            Settings = settings;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiAiDeveloperApiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue();
        }

        private async Task<QueryResponse> GetQueryAsync(string question)
        {
            if (_cache.Contains(question))
                return _cache[question] as QueryResponse;

            var uri = new Uri($"{Settings.ApiAiUri}/query?v=20150910&query={question}&lang=pt-br");
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();

            var queryResponse = JsonConvert.DeserializeObject<QueryResponse>(json, JsonSerializerSettings);
            AddToCache(question, queryResponse);
            return queryResponse;
        }

        public async Task<IEnumerable<Intent>> GetIntentsAsync()
        {
            if (_cache.Contains(typeof(IEnumerable<Intent>).Name))
                return _cache[typeof(IEnumerable<Intent>).Name] as IEnumerable<Intent>;

            var uri = new Uri($"{Settings.ApiAiUri}/intents");
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();

            var intentsResponse = JsonConvert.DeserializeObject<Intent[]>(json, JsonSerializerSettings);
            AddToCache(typeof(Intent[]).Name, intentsResponse);
            return intentsResponse;
        }

        public async Task<Intent> GetIntentAsync(string intentId)
        {
            Guid guid;
            if (!Guid.TryParse(intentId, out guid))
            {
                var queryResponse = await GetQueryAsync(intentId);
                intentId = queryResponse.Result.Metadata.IntentId;
            }

            if (_cache.Contains(intentId))
                return _cache[intentId] as Intent;

            var uri = new Uri($"{Settings.ApiAiUri}/intents/{intentId}");
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();

            var intentResponse = JsonConvert.DeserializeObject<Intent>(json, JsonSerializerSettings);
            AddToCache(intentId, intentResponse);
            return intentResponse;
        }

        public async Task<string> GetAnswerAsync(string question)
        {
            try
            {
                var answer = (await GetQueryAsync(question)).Result.Fulfillment.Speech;
                return answer;
            }
            catch
            {
                return Settings.GeneralError;
            }
        }

        private void AddToCache(string key, object value)
        {
            _cache.Add(new CacheItem(key, value), new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(Settings.CacheExpirationInMinutes)
            });
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
