namespace GoShip.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string Address { get; set; }
        public string OrderDate { get; set; }
    }
}