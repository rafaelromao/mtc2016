using MTC2016.Configuration;

namespace MTC2016.ArtificialInteligence
{
    public class ApiAiForSpeakersContent : ApiAi, IApiAiForSpeakersContent
    {
        public ApiAiForSpeakersContent(Settings settings) : base(settings)
        {
            ApiAiDeveloperApiKey = Settings.ApiAiSpeakersDeveloperApiKey;
        }

        protected override string ApiAiDeveloperApiKey { get; }
    }
}