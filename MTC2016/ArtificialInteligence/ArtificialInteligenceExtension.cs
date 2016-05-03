using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MTC2016.Configuration;
using MTC2016.Scheduler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace MTC2016.ArtificialInteligence
{
    public class ArtificialInteligenceExtension : IArtificialInteligenceExtension
    {
        private readonly Settings _settings;
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };


        public ArtificialInteligenceExtension(Settings settings)
        {
            _settings = settings;
        }


        private async Task<IEnumerable<Intent>> GetIntentsAsync()
        {
            using (var httpClient = NewHttpClient())
            {
                var uri = new Uri($"{_settings.ApiaiUri}/intents");
                var response = await httpClient.GetAsync(uri);
                var json = await response.Content.ReadAsStringAsync();
                var intentsResponse = JsonConvert.DeserializeObject<Intent[]>(json, JsonSerializerSettings);
                return intentsResponse;
            }
        }
        private async Task<Intent> GetIntentAsync(string intentId)
        {
            using (var httpClient = NewHttpClient())
            {
                var uri = new Uri($"{_settings.ApiaiUri}/intents/{intentId}");
                var response = await httpClient.GetAsync(uri);
                var json = await response.Content.ReadAsStringAsync();
                var intentResponse = JsonConvert.DeserializeObject<Intent>(json, JsonSerializerSettings);
                return intentResponse;
            }
        }
        private async Task<QueryResponse> GetQueryAsync(string question)
        {
            using (var httpClient = NewHttpClient())
            {
                var uri = new Uri($"{_settings.ApiaiUri}/query?v=20150910&lang=PT-BR&query={question}");
                var response = await httpClient.GetAsync(uri);
                var json = await response.Content.ReadAsStringAsync();
                var queryResponse = JsonConvert.DeserializeObject<QueryResponse>(json, JsonSerializerSettings);
                return queryResponse;
            }
        }
        private HttpClient NewHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiaiClientToken);
            return httpClient;
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
        public async Task<IEnumerable<ScheduledMessage>> GetScheduledMessagesAsync()
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
    }
}
