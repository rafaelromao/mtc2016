using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
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
        private readonly Settings _settings;

        protected ApiAi(Settings settings, string apiAiDeveloperApiKey)
        {
            _settings = settings;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiAiDeveloperApiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue();
        }

        private async Task<QueryResponse> GetQueryAsync(string question)
        {
            if (_cache.Contains(question) && question != _settings.CouldNotUnderstand)
                return _cache[question] as QueryResponse;

            var query = new QueryRequest()
            {
                Query = question
            };

            var requestJson = JsonConvert.SerializeObject(query, JsonSerializerSettings);

            var uri = new Uri($"{_settings.ApiAiUri}/query?v=20150910");
            var response = await _httpClient.PostAsync(uri, new StringContent(requestJson, Encoding.UTF8, MediaType));
            var responseJson = await response.Content.ReadAsStringAsync();

            var queryResponse = JsonConvert.DeserializeObject<QueryResponse>(responseJson, JsonSerializerSettings);
            AddToCache(question, queryResponse);
            return queryResponse;
        }

        public async Task<IEnumerable<Intent>> GetIntentsAsync()
        {
            var uri = new Uri($"{_settings.ApiAiUri}/intents");
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Intent[]>(json, JsonSerializerSettings);
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

            var uri = new Uri($"{_settings.ApiAiUri}/intents/{intentId}");
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
                return _settings.GeneralError;
            }
        }

        private void AddToCache(string key, object value)
        {
            _cache.Add(new CacheItem(key, value), new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_settings.CacheExpirationInMinutes)
            });
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
