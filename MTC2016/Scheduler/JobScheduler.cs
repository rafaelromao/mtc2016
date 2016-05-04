using System;
using System.Linq.Expressions;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using MTC2016.Configuration;

namespace MTC2016.Scheduler
{
    public class JobScheduler : IJobScheduler
    {
        private readonly IServiceProvider _serviceProvider;
        private BackgroundJobServer _server;

        public JobScheduler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Start()
        {
            GlobalConfiguration.Configuration
                .UseActivator(new ContainerJobActivator(_serviceProvider))
                .UseStorage(new MemoryStorage());

            _server = new BackgroundJobServer();
        }

        public void Schedule(Expression<Action> action, DateTimeOffset time)
        {
            BackgroundJob.Schedule(action, time);
        }

        public void Stop()
        {
            _server.Dispose();
        }
    }
}