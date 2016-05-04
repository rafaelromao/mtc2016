using System;
using System.Linq.Expressions;
using Hangfire;
using Hangfire.MemoryStorage;
using MTC2016.Configuration;
using MTC2016.Scheduler;

namespace MTC2016.Tests.Mocks
{
    internal class TestJobScheduler : IJobScheduler
    {
        private readonly IServiceProvider _serviceProvider;
        private BackgroundJobServer _server;

        public TestJobScheduler(IServiceProvider serviceProvider)
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

        public void Stop()
        {
            _server.Dispose();
        }

        public void Schedule(Expression<Action> action, DateTimeOffset time)
        {
            BackgroundJob.Schedule(action, time);
        }
    }
}
