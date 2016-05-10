using System;

namespace Takenet.MessagingHub.Client.Tester
{
    public class ApplicationTesterOptions
    {
        public TimeSpan DefaultTimeout;
        public bool EnableConsoleListener;
        public bool UseSeparateTestingAccount = true;
        public bool UseErrorStream;
        public Type TestServiceProviderType;
    }
}