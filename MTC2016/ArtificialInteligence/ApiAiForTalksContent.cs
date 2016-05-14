using MTC2016.Configuration;

namespace MTC2016.ArtificialInteligence
{
    public class ApiAiForTalksContent : ApiAi, IApiAiForTalksContent
    {
        public ApiAiForTalksContent(Settings settings) : base(settings)
        {
            ApiAiDeveloperApiKey = Settings.ApiAiTalksDeveloperApiKey;
        }

        protected override string ApiAiDeveloperApiKey { get; }
    }
}