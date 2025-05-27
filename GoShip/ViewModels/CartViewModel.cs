using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
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

        private decimal _lastOrderTotal;
        public decimal LastOrderTotal
        {
            get => _lastOrderTotal;
            set
            {
                _lastOrderTotal = value;
                OnPropertyChanged();
            }
        }

        public CartViewModel(int userId)
        {
            this.userId = userId;
            db = new DatabaseService();
            LoadCart();
            LoadPastOrders();
            RemoveFromCartCommand = new RelayCommand<int>(RemoveFromCart);
        }

        public void LoadCart()
        {
            var cartItems = db.GetCartItems(userId);
            CartItems = new ObservableCollection<CartItem>(cartItems);
            CartTotal = CartItems.Sum(item => item.Product.Price * item.Quantity);
        }

        public void LoadPastOrders()
        {
            var pastOrders = db.GetOrders(userId);
            PastOrders = new ObservableCollection<Order>(pastOrders);
        }

        public void RemoveFromCart(int productId)
        {
            db.RemoveFromCart(userId, productId);
            LoadCart();
            MessageBox.Show($"Продукт с ID {productId} удалён из корзины!");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand RemoveFromCartCommand { get; }
    }

    // Класс RelayCommand для команд
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;

        public void Execute(object parameter) => _execute((T)parameter);
    }
}