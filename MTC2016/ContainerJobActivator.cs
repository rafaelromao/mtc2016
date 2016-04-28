using System;
using Hangfire;

namespace MTC2016
{
    public class ContainerJobActivator : JobActivator
    {
        private IServiceProvider _container;

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