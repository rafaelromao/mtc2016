using System;
using System.Threading.Tasks;
using Lime.Protocol;
using MTC2016.Configuration;
using Takenet.Elephant.Sql;
using Takenet.Elephant.Sql.Mapping;

namespace MTC2016.Receivers
{
    public class FeedbackRepository : SqlSet<Feedback>, IFeedbackRepository
    {
        private new static readonly ITable Table = TableBuilder
            .WithName(nameof(Feedback))
            .WithKeyColumnFromType<Identity>(nameof(Feedback.From))
            .WithKeyColumnFromType<DateTimeOffset>(nameof(Feedback.When))
            .WithKeyColumnFromType<FeedbackType>(nameof(Feedback.Type))
            .WithColumnFromType<string>(nameof(Feedback.Text))
            .Build();

        public FeedbackRepository(Settings settings) 
            : base(settings.ConnectionString, Table, new TypeMapper<Feedback>(Table))
        {
        }
    }
}