using Lime.Protocol;
using MTC2016.Configuration;

namespace MTC2016.ArtificialInteligence
{
    internal class Entry
    {
        public string Value { get; set; }

        public Node ToIdentity(Settings settings)
            => Node.Parse(Value.Replace(settings.AtReplacement, "@").Replace(settings.DolarReplacement, "$"));
    }
}