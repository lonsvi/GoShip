using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GoShip.Models;
using GoShip.Services;

namespace GoShip.ViewModels
{
    public class EmployeeMainViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService db;

        public ObservableCollection<Order> Orders { get; set; }

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
            LoadOrders();
        }

        public void MarkAsCompleted(int orderId)
        {
            db.UpdateOrderStatus(orderId, "Завершён");
            LoadOrders();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}