using Lime.Protocol;

namespace MTC2016
{
    public static class MessageExtensions
    {
        public static string ToKey(this Message message)
        {
            return $"{message.To}|{message.Content}";
        }
    }
}