using System;

namespace GoShip.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}