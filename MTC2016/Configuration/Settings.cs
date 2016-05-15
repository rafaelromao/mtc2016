using System.Dynamic;

namespace MTC2016.Configuration
{
    public class Settings
    {
        public string ApiAiUri { get; set; }
        public string ApiAiStaticDeveloperApiKey { get; set; }
        public string ApiAiDynamicDeveloperApiKey { get; set; }

        public string ConnectionString { get; set; }

        public string UsersEntity { get; set; }
        public string AtReplacement { get; set; }
        public string DolarReplacement { get; set; }
        public string SchedulePrefix { get; set; }
        public string GeneralError { get; set; }
        public string CouldNotUnderstand { get; set; }
        public string SubscriptionFailed { get; set; }
        public string ConfirmSubscription { get; set; }
        public string AlreadySubscribed { get; set; }
        public string NotSubscribed { get; set; }
        public string ConfirmSubscriptionCancellation { get; set; }
        public string UnsubscriptionFailed { get; set; }
        public string Quote { get; set; }
        public string FeedbackConfirmation { get; set; }
        public string FeedbackPrefix { get; set; }
        public string FeedbackFailed { get; set; }
        public string RatingText { get; set; }
        public string RatingConfirmation { get; set; }
        public string RatingFailed { get; set; }
        public string RatingPrefix { get; set; }
        public string BadRating { get; set; }
        public string RegularRating { get; set; }
        public string GoodRating { get; set; }
        public string ImageConfirmation { get; set; }

        public string EncodeIdentity(string text)
        {
            return text.Replace("@", AtReplacement).Replace("$", DolarReplacement).Replace(".", "_").Replace("#", "_");
        }
    }
}
