namespace MTC2016.ArtificialInteligence
{
    public class Intent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Templates { get; set; }
        public IntentResponse[] Responses { get; set; }
        public UserSays[] UserSays { get; set; }
    }
}