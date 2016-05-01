using System;

namespace Takenet.MessagingHub.Client.Tester
{
    public struct ApplicationTesterOptions
    {
        public TimeSpan DefaultTimeout;
        public bool EnableConsoleListener;
        public bool UseErrorStream;
        public Type TestServiceProviderType;
    }
}