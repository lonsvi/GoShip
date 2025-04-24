using System;
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
        private readonly DatabaseService db;
        private readonly int userId;

        public ObservableCollection<CartItem> CartItems { get; set; }
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

        public CartViewModel(int userId)
        {
            this.userId = userId;
            db = new DatabaseService();
            LoadCart();
            LoadPastOrders();
        }

        private void LoadCart()
        {
            var cartItems = db.GetCartItems(userId);
            CartItems = new ObservableCollection<CartItem>(cartItems);
            CartTotal = CartItems.Sum(item => item.Product.Price * item.Quantity);
        }

        private void LoadPastOrders()
        {
            var pastOrders = db.GetOrders(userId);
            PastOrders = new ObservableCollection<Order>(pastOrders);
        }

        public void RemoveFromCart(int productId)
        {
            db.RemoveFromCart(userId, productId);
            LoadCart();
        }

        public void ConfirmOrder(string address)
        {
            var items = CartItems.ToList();
            if (!items.Any())
                return;

            decimal total = items.Sum(item => item.Product.Price * item.Quantity);
            string orderDate = DateTime.Now.ToString("yyyy-MM-dd");
            int orderId = db.CreateOrder(userId, address, total, orderDate);

            foreach (var item in items)
            {
                db.AddOrderItem(orderId, item.ProductId, item.Product.Name, item.Product.Price, item.Quantity);
            }

            db.ClearCart(userId);
            LoadCart();
            LoadPastOrders();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}