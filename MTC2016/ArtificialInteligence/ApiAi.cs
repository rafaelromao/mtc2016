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
    public abstract class ApiAi : IApiAI, IDisposable
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        const string MediaType = "application/json";
        private readonly HttpClient _httpClient;
        private readonly ObjectCache _cache = new MemoryCache(nameof(MTC2016));

        protected abstract string ApiAiDeveloperApiKey { get; }

        public Settings Settings { get; }

        protected ApiAi(Settings settings)
        {
            Settings = settings;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAiDeveloperApiKey);
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
                AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)
            });
        }





        private async Task<IEnumerable<Entry>> GetEntityEntriesAsync(string entity)
        {
            var uri = new Uri($"{Settings.ApiAiUri}/entities/{entity}");
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();
            var entryResponse = JsonConvert.DeserializeObject<Entity>(json, JsonSerializerSettings);
            return entryResponse.Entries;
        }

        public async Task<bool> AddIntentAsync(Intent intent)
        {
            var uri = new Uri($"{Settings.ApiAiUri}/intents");
            var json = JsonConvert.SerializeObject(intent, JsonSerializerSettings);
            var response = await _httpClient.PostAsync(uri, new StringContent(json, Encoding.UTF8, MediaType));
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> DeleteIntent(string intentQuestion)
        {
            var queryResponse = await GetQueryAsync(intentQuestion);
            if (queryResponse?.Result?.Metadata?.IntentId == null)
                return false;

            var uri = new Uri($"{Settings.ApiAiUri}/intents/{queryResponse.Result.Metadata.IntentId}");
            var response = await _httpClient.DeleteAsync(uri);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> AddFeedbackAsync(string feedbackId, string feedback)
        {
            var intent = new Intent
            {
                Name = feedbackId,
                Templates = new []
                {
                    feedbackId
                },
                UserSays = new []
                {
                    new UserSays
                    {
                        Data = new []
                        {
                            new UserSaysData
                            {
                                Text = feedbackId.Replace("_", " ")
                            }
                        },
                        IsTemplate = false
                    }
                },
                Responses = new[]
                {
                    new IntentResponse
                    {
                        Speech = feedback.Replace("#", "")
                    }
                }
            };
            return await AddIntentAsync(intent);
        }

        //public async Task<bool> AddUserAsync(Node user)
        //{
        //    var entries = new[] { Settings.EncodeIdentity(user.ToNode().ToString()) };

        //    var uri = new Uri($"{Settings.ApiAiUri}/entities/{Settings.UsersEntity}/entries");
        //    var json = JsonConvert.SerializeObject(entries, JsonSerializerSettings);
        //    var response = await _httpClient.PostAsync(uri, new StringContent(json, Encoding.UTF8, MediaType));
        //    return response.StatusCode == HttpStatusCode.OK;
        //}

        //public async Task<IEnumerable<Node>> GetUsersAsync()
        //{
        //    var entries = await GetEntityEntriesAsync(Settings.UsersEntity);
        //    var users = entries.Select(e => e.ToIdentity(Settings));
        //    return users;
        //}

        //public async Task<bool> ContainsUserAsync(Node user)
        //{
        //    var users = await GetUsersAsync();
        //    return users.Any(u => u.Equals(user));
        //}

        //public async Task<bool> RemoveUserAsync(Node user)
        //{
        //    var entries = new[] { Settings.EncodeIdentity(user.ToNode().ToString()) };

        //    var uri = new Uri($"{Settings.ApiAiUri}/entities/{Settings.UsersEntity}/entries");
        //    var json = JsonConvert.SerializeObject(entries, JsonSerializerSettings);
        //    var response = await _httpClient.SendAsync(new HttpRequestMessage
        //    {
        //        Method = HttpMethod.Delete,
        //        RequestUri = uri,
        //        Content = new StringContent(json, Encoding.UTF8, MediaType)
        //    });
        //    return response.StatusCode == HttpStatusCode.OK;
        //}

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
