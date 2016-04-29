using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Protocol.Util;
using MTC2016.DistributionList;

namespace MTC2016.Tests.Mocks
{
    public class TestDistributionListExtension : IDistributionListExtension
    {
        private readonly List<Identity> _storage = new List<Identity>();

        public Task<bool> AddAsync(Identity recipient, CancellationToken cancellationToken)
        {
            _storage.Add(recipient);
            return TaskUtil.TrueCompletedTask;
        }

        public Task<IEnumerable<Identity>> GetRecipientsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_storage.AsEnumerable());
        }

        public Task<bool> ContainsAsync(Identity recipient, CancellationToken cancellationToken)
        {
            return Task.FromResult(_storage.Contains(recipient));
        }

        public Task<bool> RemoveAsync(Identity recipient, CancellationToken cancellationToken)
        {
            _storage.Remove(recipient);
            return TaskUtil.TrueCompletedTask;
        }
    }
}