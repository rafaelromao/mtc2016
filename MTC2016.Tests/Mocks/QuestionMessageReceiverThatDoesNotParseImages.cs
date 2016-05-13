using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.ArtificialInteligence;
using MTC2016.Configuration;
using MTC2016.Receivers;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Tests.Mocks
{
    class QuestionMessageReceiverThatDoesNotParseImages : QuestionMessageReceiver
    {
        public QuestionMessageReceiverThatDoesNotParseImages(IMessagingHubSender sender, IArtificialInteligenceExtension artificialInteligenceExtension, Settings settings) 
            : base(sender, artificialInteligenceExtension, settings)
        {
        }

        protected override Task<string> ExtractAndSendImagesAsync(string answer, Node from, CancellationToken cancellationToken)
        {
            return Task.FromResult(answer);
        }
    }
}
