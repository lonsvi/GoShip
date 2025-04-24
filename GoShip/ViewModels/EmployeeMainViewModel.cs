using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq; // Добавляем для LINQ
using System.Runtime.CompilerServices;
using GoShip.Models;
using GoShip.Services;

namespace GoShip.ViewModels
{
    public class EmployeeMainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService db;

        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged();
            }
        }

        public EmployeeMainViewModel()
        {
            db = new DatabaseService();
            LoadOrders();
        }

        private void LoadOrders()
        {
            var orders = db.GetAllOrders();
            Orders = new ObservableCollection<Order>(orders);
        }

        public void MarkAsActive(int orderId)
        {
            db.UpdateOrderStatus(orderId, "Активный");
            var order = Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = "Активный"; // Обновляем статус в коллекции
            }
        }

        public void MarkAsCompleted(int orderId)
        {
            db.UpdateOrderStatus(orderId, "Завершён");
            var order = Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = "Завершён"; // Обновляем статус в коллекции
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}