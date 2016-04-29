using System;
using MTC2016.DistributionList;
using MTC2016.Scheduler;
using SimpleInjector;
using Takenet.MessagingHub.Client.Host;

namespace MTC2016.Configuration
{
    class ServiceProvider : IServiceContainer
    {
        private Container Container { get; }

        public ServiceProvider()
        {
            Container = new Container();
            Container.RegisterSingleton<ISchedulerExtension, SchedulerExtension>();
            Container.RegisterSingleton<IDistributionListExtension, DistributionListExtension>();
            Container.RegisterSingleton<IDistributionListRecipientsList, DistributionListRecipientsList>();
        }

        public object GetService(Type serviceType)
        {
            return Container.GetInstance(serviceType);
        }

        public void RegisterService(Type serviceType, object instance)
        {
            Container.RegisterSingleton(serviceType, instance);
        }
    }
}