using MTC2016.Configuration;

namespace MTC2016.ArtificialInteligence
{
    public class ApiAiForStaticContent : ApiAi, IApiAiForStaticContent
    {
        public ApiAiForStaticContent(Settings settings) : base(settings)
        {
            ApiAiDeveloperApiKey = Settings.ApiAiStaticDeveloperApiKey;
        }

        protected override string ApiAiDeveloperApiKey { get; }
    }
}