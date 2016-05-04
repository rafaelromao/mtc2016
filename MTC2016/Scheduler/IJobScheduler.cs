using System;
using System.Linq.Expressions;

namespace MTC2016.Scheduler
{
    public interface IJobScheduler
    {
        void Start();
        void Stop();
        void Schedule(Expression<Action> action, DateTimeOffset time);
    }
}
