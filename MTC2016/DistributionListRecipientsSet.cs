using System.Collections.Generic;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.Elephant.Sql;
using Takenet.Elephant.Sql.Mapping;

namespace MTC2016
{
    public class DistributionListRecipientsSet : SqlSet<Identity>, IDistributionListRecipientsSet
    {
        private const string TABLE_NAME = "DistributionListRecipients";
        private const string RECIPIENT_IDENTITY_COLUMN_NAME = "RecipientIdentity";

        private new static readonly ITable Table = TableBuilder
            .WithName(TABLE_NAME)
            .WithKeyColumnFromType<Identity>(RECIPIENT_IDENTITY_COLUMN_NAME)
            .Build();

        public DistributionListRecipientsSet(Settings settings)
            : base(settings.ConnectionString, Table, new ValueMapper<Identity>(RECIPIENT_IDENTITY_COLUMN_NAME))
        {
        }
    }
}