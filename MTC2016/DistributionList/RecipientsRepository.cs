using Lime.Protocol;
using MTC2016.Configuration;
using Takenet.Elephant.Sql;
using Takenet.Elephant.Sql.Mapping;

namespace MTC2016.DistributionList
{
    internal class RecipientsRepository : SqlSet<Identity>, IRecipientsRepository
    {
        private new static readonly ITable Table = TableBuilder
            .WithName(nameof(Identity))
            .WithKeyColumnFromType<string>(nameof(Identity.Name))
            .WithKeyColumnFromType<string>(nameof(Identity.Domain))
            .Build();

        public RecipientsRepository(Settings settings) 
            : base(settings.ConnectionString, Table, new TypeMapper<Identity>(Table))
        {
        }
    }
}