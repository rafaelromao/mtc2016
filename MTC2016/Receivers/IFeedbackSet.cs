using System;
using Lime.Protocol;
using MTC2016.Configuration;
using Takenet.Elephant;
using Takenet.Elephant.Sql;
using Takenet.Elephant.Sql.Mapping;

namespace MTC2016.Receivers
{
    public interface IFeedbackSet : ISet<Feedback>
    {
    }

    public class FeedbackSet : SqlSet<Feedback>, IFeedbackSet
    {
        public new static ITable Table = TableBuilder
            .WithName("Feedbacks")
            .WithKeyColumnFromType<Identity>("From")
            .WithKeyColumnFromType<DateTimeOffset>("When")
            .WithKeyColumnFromType<FeedbackType>("Type")
            .WithColumnFromType<string>("Text")
            .Build();

        public FeedbackSet(Settings settings, string connectionString) 
            : base(settings.ConnectionString, Table, new TypeMapper<Feedback>(Table))
        {
        }
    }
}