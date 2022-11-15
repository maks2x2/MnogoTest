namespace Domain.Models
{
    public class Message
    {
        public Message(string body, int priority)
        {
            Body = body;
            Priority = priority;
        }

        public string Body { get; set; }

        public int Priority { get; set; }

    }
}