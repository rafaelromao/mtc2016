using MTC2016.Configuration;

namespace MTC2016.ArtificialInteligence
{
    public class ApiAiForStaticContent : ApiAi, IApiAiForStaticContent
    {
        public ApiAiForStaticContent(Settings settings)
            : base(settings, settings.ApiAiStaticDeveloperApiKey)
        {
        }
    }
}