using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using MTC2016.Receivers;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Tests.Mocks
{
    class QuestionMessageReceiverForOmni : QuestionMessageReceiver
    {
        public QuestionMessageReceiverForOmni(IMessagingHubSender sender, IApiAiForDynamicContent apiAi, Settings settings) 
            : base(sender, apiAi, settings)
        {
        }

        protected override string GetDomain(Identity from)
        {
            return Domains.Omni;
        }
    }

    class QuestionMessageReceiverForTangram : QuestionMessageReceiver
    {
        public QuestionMessageReceiverForTangram(IMessagingHubSender sender, IApiAiForDynamicContent apiAi, Settings settings)
            : base(sender, apiAi, settings)
        {
        }

        protected override string GetDomain(Identity from)
        {
            return Domains.Tangram;
        }
    }
}
