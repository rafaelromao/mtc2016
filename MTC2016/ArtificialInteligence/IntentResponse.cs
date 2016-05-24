using System.Collections.Generic;
using Newtonsoft.Json;

namespace MTC2016.ArtificialInteligence
{
    public class IntentResponse
    {
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Speech { get; set; }
    }
}