using System.Collections.ObjectModel;
using GoShip.Models;
using GoShip.Services;

namespace GoShip.ViewModels
{
    public class ClientMainViewModel
    {
        public ObservableCollection<Product> Products { get; set; }
        private readonly DatabaseService db;

        public ClientMainViewModel()
        {
            db = new DatabaseService();
            Products = new ObservableCollection<Product>(db.GetProducts());
        }

        public void PlaceOrder(int userId, int productId, string address)
        {
            db.PlaceOrder(userId, productId, address);
        }
    }
}