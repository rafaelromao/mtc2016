using System;
using SimpleInjector;
using Takenet.MessagingHub.Client.Host;

namespace MTC2016
{
    class ServiceProvider : IServiceContainer
    {
        private static Container Container { get; }

        static ServiceProvider()
        {
            Container = new Container();
        }

        public ServiceProvider()
        {
            Container.RegisterSingleton<ISchedulerExtension, SchedulerExtension>();
            Container.RegisterSingleton<IDistributionListExtension, DistributionListExtension>();
            Container.RegisterSingleton<IDistributionListRecipientsSet, DistributionListRecipientsSet>();
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