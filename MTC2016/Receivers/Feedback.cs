using System;
using Lime.Protocol;

namespace MTC2016.Receivers
{
    public class Feedback
    {
        public Identity From { get; set; }
        public DateTimeOffset When { get; set; }
        public string Text { get; set; }
        public FeedbackType Type { get; set; }
    }
}