using System;

namespace Takenet.MessagingHub.Client.Tester
{
    public struct ApplicationTesterOptions
    {
        public string TesterIdentifier;
        public string TesterAccesskey;
        public TimeSpan DefaultTimeout;
        public bool EnableConsoleListener;
        public bool UseErrorStream;
        public Type TestServiceProviderType;
    }
}