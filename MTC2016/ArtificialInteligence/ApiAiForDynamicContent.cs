using MTC2016.Configuration;

namespace MTC2016.ArtificialInteligence
{
    public class ApiAiForDynamicContent : ApiAi, IApiAiForDynamicContent
    {
        public ApiAiForDynamicContent(Settings settings) 
            : base(settings, settings.ApiAiDynamicDeveloperApiKey)
        {
        }
    }
}