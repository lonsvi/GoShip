namespace GoShip.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Calories { get; set; }
        public decimal Proteins { get; set; }
        public decimal Fats { get; set; }
    }
}