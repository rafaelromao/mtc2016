using System.Threading.Tasks;

namespace MTC2016.Receivers
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IFeedbackSet _feedbackSet;

        public FeedbackRepository(IFeedbackSet feedbackSet)
        {
            _feedbackSet = feedbackSet;
        }

        public async Task<bool> AddFeedbackAsync(Feedback feedback)
        {
            try
            {
                await _feedbackSet.AddAsync(feedback);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}