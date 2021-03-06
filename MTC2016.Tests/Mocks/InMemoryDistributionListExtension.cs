using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Protocol.Util;
using MTC2016.DistributionList;

namespace MTC2016.Tests.Mocks
{
    public class InMemoryDistributionListExtension : IDistributionListExtension
    {
        private readonly ConcurrentBag<Identity> _storage = new ConcurrentBag<Identity>();

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
            return Task.FromResult(_storage.Any(r => r.Equals(recipient)));
        }

        public Task<bool> RemoveAsync(Identity recipient, CancellationToken cancellationToken)
        {
            var rec = _storage.SingleOrDefault(r => r.Equals(recipient));
            if (rec != null)
                _storage.TryTake(out rec);
            return TaskUtil.TrueCompletedTask;
        }
    }
}