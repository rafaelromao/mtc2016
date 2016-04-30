using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Takenet.MessagingHub.Client.Host;

namespace Takenet.MessagingHub.Client.Tester
{
    public abstract class ClientTesterServiceProvider : IServiceContainer
    {
        private readonly IDictionary<Type, object> _testServiceTypes = new ConcurrentDictionary<Type, object>();

        public object GetService(Type serviceType)
        {
            var testService = _testServiceTypes.ContainsKey(serviceType) ? _testServiceTypes[serviceType] : null;
            return testService ?? ClientTester.ApplicationServiceProvider?.GetService(serviceType);
        }

        public void RegisterService(Type serviceType, object instance)
        {
            var applicationServiceContainer = ClientTester.ApplicationServiceProvider as IServiceContainer;
            if (applicationServiceContainer != null)
                applicationServiceContainer.RegisterService(serviceType, instance);
            else
                _testServiceTypes[serviceType] = instance;
        }
    }
}