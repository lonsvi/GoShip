using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GoShip.Models;
using GoShip.Services;

namespace GoShip.ViewModels
{
    public class ClientMainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService db;
        private readonly int userId;

        public ObservableCollection<Product> Products { get; set; }

        private decimal _cartTotal;
        public decimal CartTotal
        {
            get => _cartTotal;
            set
            {
                _cartTotal = value;
                OnPropertyChanged();
            }
        }

        public ClientMainViewModel(int userId)
        {
            this.userId = userId;
            db = new DatabaseService();
            Products = new ObservableCollection<Product>(db.GetProducts());
            LoadCartTotal();
        }

        public void LoadCartTotal()
        {
            var cartItems = db.GetCartItems(userId);
            CartTotal = cartItems.Sum(item => item.Product.Price * item.Quantity);
        }

        public void AddToCart(int productId)
        {
            db.AddToCart(userId, productId, 1);
            LoadCartTotal();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}