using System;

namespace Takenet.MessagingHub.Client.Tester
{
    public struct ClientTesterOptions
    {
        public string TesterIdentifier;
        public string TesterAccesskey;
        public bool EnableConsoleListener;
        public bool UseErrorStream;
        public Type TestServiceProviderType;
    }
}