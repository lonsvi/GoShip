using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GoShip.Models;
using GoShip.Services;

namespace GoShip.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Order> CartItems { get; set; }
        public ObservableCollection<Order> PastOrders { get; set; }

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

        private readonly DatabaseService db;
        private readonly int userId;

        public CartViewModel(int userId)
        {
            this.userId = userId;
            db = new DatabaseService();
            LoadCart();
            LoadPastOrders();
        }

        private void LoadCart()
        {
            var orders = db.GetOrders(userId).Where(o => o.Address == "В корзине").ToList();
            CartItems = new ObservableCollection<Order>(orders);
            CartTotal = CartItems.Sum(order => order.Product.Price);
        }

        private void LoadPastOrders()
        {
            var pastOrders = db.GetOrders(userId).Where(o => o.Address != "В корзине").ToList();
            PastOrders = new ObservableCollection<Order>(pastOrders);
        }

        public void RemoveFromCart(int orderId)
        {
            db.RemoveOrder(orderId);
            LoadCart();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}