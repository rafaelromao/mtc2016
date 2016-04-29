using MTC2016.Configuration;
using MTC2016.DistributionList;
using MTC2016.Receivers;
using Takenet.MessagingHub.Client.Sender;

namespace MTC2016.Tests.Mocks
{
    public class TestUnsubscribeMessageReceiver : UnsubscribeMessageReceiver
    {
        public TestUnsubscribeMessageReceiver(IMessagingHubSender sender, IDistributionListExtension distributionListExtension, Settings settings) 
            : base(sender, new TestDistributionListExtension(), settings)
        {
        }
    }
}
