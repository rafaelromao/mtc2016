﻿using MTC2016.DistributionList;
using Takenet.MessagingHub.Client.Tester;

namespace MTC2016.Tests.Mocks
{
    public class TestServiceProvider : ClientTesterServiceProvider
    {
        public TestServiceProvider() 
        {
            RegisterService(typeof(IDistributionListExtension), new TestDistributionListExtension());
        }
    }
}
