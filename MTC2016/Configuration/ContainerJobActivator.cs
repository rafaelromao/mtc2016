using System;
using Hangfire;

namespace MTC2016.Configuration
{
    public class ContainerJobActivator : JobActivator
    {
        private readonly IServiceProvider _container;

        public ContainerJobActivator(IServiceProvider container)
        {
            _container = container;
        }

        public override object ActivateJob(Type type)
        {
             return _container.GetService(type);
        }
    }
}