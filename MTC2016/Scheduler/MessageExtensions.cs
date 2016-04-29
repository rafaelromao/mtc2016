using Lime.Protocol;

namespace MTC2016.Scheduler
{
    public static class MessageExtensions
    {
        public static string ToKey(this Message message)
        {
            return $"{message.To}|{message.Content}";
        }
    }
}