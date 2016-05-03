using System.Linq;
using System.Text.RegularExpressions;
using MTC2016.Scheduler;

namespace MTC2016.Configuration
{
    public class Settings
    {
        public string ConnectionString { get; set; }

        public Messages Messages { get; set; }

        public ScheduledMessage[] ScheduledMessages { get; set; }

        public Question[] Questions { get; set; }

        public Question QuestionWithContent(string text)
        {
            return Questions.SingleOrDefault(q => Regex.IsMatch(text, q.Content, RegexOptions.Compiled | RegexOptions.IgnoreCase));
        }
    }
}
