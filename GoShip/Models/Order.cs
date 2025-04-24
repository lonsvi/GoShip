namespace GoShip.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CardNumber { get; set; }
        public string CardDate { get; set; }
        public string CVV { get; set; }
        public string DeliveryTime { get; set; }
        public string Comment { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }
}