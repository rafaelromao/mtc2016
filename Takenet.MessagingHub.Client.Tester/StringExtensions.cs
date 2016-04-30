namespace Takenet.MessagingHub.Client.Tester
{
    public static class StringExtensions
    {
        public static string ToStringFormatRegex(this string text, int formatPlaceholderCount = 10)
        {
            // Ol�, {0}. N�o se esque�a da sua consulta.     =>     (\W|^)Ol�, [\w ]*[^\W_][\w ]*. N�o se esque�a da sua consulta.(\W|$)

            for (var i = 0; i < formatPlaceholderCount; i++)
                text = text.Replace($"{{{i}}}", "[\\w ]*[^\\W_][\\w ]*");

            text = $"(\\W|^){text}(\\W|$)";
            return text;
        }
    }
}