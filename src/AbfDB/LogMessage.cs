namespace AbfDB
{
    public class LogMessage
    {
        public readonly string Verb;
        public readonly string Noun;
        public readonly DateTime Timestamp;

        public LogMessage(string verb, string noun)
        {
            Verb = verb;
            Noun = noun;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{Timestamp}] {Verb} {Noun}";
        }
    }
}
