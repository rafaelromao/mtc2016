using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Protocol.Util;
using MTC2016.DistributionList;

namespace MTC2016.Tests.Mocks
{
    public class NoPersistenceDistributionListExtension : IDistributionListExtension
    {
        public Task<bool> AddAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return TaskUtil.TrueCompletedTask;
        }

        public Task<IEnumerable<Identity>> GetRecipientsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumerable.Empty<Identity>());
        }

        public Task<bool> ContainsAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return TaskUtil.FalseCompletedTask;
        }

        public Task<bool> RemoveAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return TaskUtil.FalseCompletedTask;
        }
    }
}