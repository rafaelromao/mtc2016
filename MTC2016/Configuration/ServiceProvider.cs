using System;
using MTC2016.ArtificialInteligence;
using MTC2016.DistributionList;
using MTC2016.FeedbackAndRating;
using MTC2016.Receivers;
using MTC2016.Scheduler;
using SimpleInjector;
using Takenet.MessagingHub.Client.Host;

namespace MTC2016.Configuration
{
    public class ServiceProvider : IServiceContainer
    {
        public Container Container { get; }

        public ServiceProvider()
        {
            Container = new Container();

            Container.Options.AllowOverridingRegistrations = true;

            Container.RegisterSingleton<IApiAiForStaticContent, ApiAiForStaticContent>();
            Container.RegisterSingleton<IApiAiForDynamicContent, ApiAiForDynamicContent>();

            Container.RegisterSingleton<IJobScheduler, JobScheduler>();
            Container.RegisterSingleton<ISchedulerExtension, SchedulerExtension>();

            Container.RegisterSingleton<IRecipientsRepository, RecipientsRepository>();
            Container.RegisterSingleton<IDistributionListExtension, DistributionListExtension>();

            Container.RegisterSingleton<IFeedbackRepository, FeedbackRepository>();
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