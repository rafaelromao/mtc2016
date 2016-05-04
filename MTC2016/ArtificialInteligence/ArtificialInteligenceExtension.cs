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
using MTC2016.Scheduler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        }


        private async Task<IEnumerable<Intent>> GetIntentsAsync()
        {
            var uri = new Uri($"{_settings.ApiaiUri}/intents");
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();
            var intentsResponse = JsonConvert.DeserializeObject<Intent[]>(json, JsonSerializerSettings);
            return intentsResponse;
        }
        private async Task<Intent> GetIntentAsync(string intentId)
        {
            var uri = new Uri($"{_settings.ApiaiUri}/intents/{intentId}");
            var response = await _httpClient.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();
            var intentResponse = JsonConvert.DeserializeObject<Intent>(json, JsonSerializerSettings);
            return intentResponse;
        }
        private async Task<QueryResponse> GetQueryAsync(string question)
        {
            var uri = new Uri($"{_settings.ApiaiUri}/query?v=20150910&lang=PT-BR&query={question}");
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

        private class Intent
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public IntentResponse[] Responses { get; set; }
        }
        private class IntentResponse
        {
            public string Speech { get; set; }
        }
        private class QueryResponse
        {
            public QueryResult Result { get; set; }
        }
        private class QueryResult
        {
            public QueryResultFulfillment Fulfillment { get; set; }
        }
        private class QueryResultFulfillment
        {
            public string Speech { get; set; }
        }
        private class Entity
        {
            public Entry[] Entries { get; set; }
        }

        private class Entry
        {
            public string Value { get; set; }
            public string[] Synonyms { get; set; }

            public Node ToIdentity(Settings settings)
                => Node.Parse(Value.Replace(settings.AtReplacement, "@").Replace(settings.DolarReplacement, "$"));

            public static Entry FromNode(Node node, Settings settings) => new Entry
            {
                Value = $"{node.ToString().Replace("@", settings.AtReplacement).Replace("$", settings.DolarReplacement)}"
            };
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
        public virtual async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync()
        {
            var result = new List<ScheduledMessage>();
            var intents = await GetIntentsAsync();
            var schedulePrefix = _settings.SchedulePrefix;
            var schedules = intents.Where(i => i.Name.StartsWith(schedulePrefix));
            foreach (var schedule in schedules)
            {
                result.Add(new ScheduledMessage
                {
                    Time = DateTimeOffset.Parse(schedule.Name.Substring(schedulePrefix.Length)),
                    Text = (await GetIntentAsync(schedule.Id)).Responses.First().Speech
                });
            }
            return result;
        }

        public async Task<bool> AddUserAsync(Node user)
        {
            var entries = new[] { Entry.FromNode(user, _settings) };

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
            var entries = new[] { Entry.FromNode(user, _settings).Value };

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
