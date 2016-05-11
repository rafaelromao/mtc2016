using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MTC2016.ArtificialInteligence
{
    public class ArtificialInteligenceExtension : IArtificialInteligenceExtension, IDisposable
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        const string MediaType = "application/json";
        private readonly Settings _settings;
        private readonly HttpClient _httpClient;

        public ArtificialInteligenceExtension(Settings settings)
        {
            _settings = settings;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiaiDeveloperApiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue();
        }


        private async Task<QueryResponse> GetQueryAsync(string question)
        {
            var uri = new Uri($"{_settings.ApiaiUri}/query?v=20150910&query={question}&lang=pt-br");
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();
            var queryResponse = JsonConvert.DeserializeObject<QueryResponse>(json, JsonSerializerSettings);
            return queryResponse;
        }
        private async Task<IEnumerable<Entry>> GetEntityEntriesAsync(string entity)
        {
            var uri = new Uri($"{_settings.ApiaiUri}/entities/{entity}");
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();
            var entryResponse = JsonConvert.DeserializeObject<Entity>(json, JsonSerializerSettings);
            return entryResponse.Entries;
        }

        public async Task<IEnumerable<Intent>> GetIntentsAsync()
        {
            var uri = new Uri($"{_settings.ApiaiUri}/intents");
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();
            var intentsResponse = JsonConvert.DeserializeObject<Intent[]>(json, JsonSerializerSettings);
            return intentsResponse;
        }

        public async Task<Intent> GetIntentAsync(string intentId)
        {
            var uri = new Uri($"{_settings.ApiaiUri}/intents/{intentId}");
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();
            var intentResponse = JsonConvert.DeserializeObject<Intent>(json, JsonSerializerSettings);
            return intentResponse;
        }

        public async Task<bool> AddIntentAsync(Intent intent)
        {
            var uri = new Uri($"{_settings.ApiaiUri}/intents");
            var json = JsonConvert.SerializeObject(intent, JsonSerializerSettings);
            var response = await _httpClient.PostAsync(uri, new StringContent(json, Encoding.UTF8, MediaType));
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> DeleteIntent(string intentQuestion)
        {
            var queryResponse = await GetQueryAsync(intentQuestion);
            if (queryResponse?.Result?.Metadata?.IntentId == null)
                return false;

            var uri = new Uri($"{_settings.ApiaiUri}/intents/{queryResponse.Result.Metadata.IntentId}");
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

        public async Task<bool> AddUserAsync(Node user)
        {
            var entries = new[] { _settings.EncodeIdentity(user.ToNode().ToString()) };

            var uri = new Uri($"{_settings.ApiaiUri}/entities/{_settings.UsersEntity}/entries");
            var json = JsonConvert.SerializeObject(entries, JsonSerializerSettings);
            var response = await _httpClient.PostAsync(uri, new StringContent(json, Encoding.UTF8, MediaType));
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<IEnumerable<Node>> GetUsersAsync()
        {
            var entries = await GetEntityEntriesAsync(_settings.UsersEntity);
            var users = entries.Select(e => e.ToIdentity(_settings));
            return users;
        }

        public async Task<bool> ContainsUserAsync(Node user)
        {
            var users = await GetUsersAsync();
            return users.Any(u => u.Equals(user));
        }

        public async Task<bool> RemoveUserAsync(Node user)
        {
            var entries = new[] { _settings.EncodeIdentity(user.ToNode().ToString()) };

            var uri = new Uri($"{_settings.ApiaiUri}/entities/{_settings.UsersEntity}/entries");
            var json = JsonConvert.SerializeObject(entries, JsonSerializerSettings);
            var response = await _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = uri,
                Content = new StringContent(json, Encoding.UTF8, MediaType)
            });
            return response.StatusCode == HttpStatusCode.OK;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
