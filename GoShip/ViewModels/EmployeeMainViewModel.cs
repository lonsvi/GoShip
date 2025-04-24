using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
            // Сортируем: сначала по статусу (Активный -> Завершён), затем по времени (свежие сверху)
            var sortedOrders = orders
                .OrderBy(o => o.Status == "Активный" ? 0 : 1) // Активные первые
                .ThenByDescending(o => DateTime.Parse(o.OrderDate)) // По времени, свежие сверху
                .ToList();
            Orders = new ObservableCollection<Order>(sortedOrders);
        }

        public void MarkAsActive(int orderId)
        {
            db.UpdateOrderStatus(orderId, "Активный");
            var order = Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = "Активный";
                // Пересортировываем список
                var sortedOrders = Orders
                    .OrderBy(o => o.Status == "Активный" ? 0 : 1)
                    .ThenByDescending(o => DateTime.Parse(o.OrderDate))
                    .ToList();
                Orders.Clear();
                foreach (var o in sortedOrders)
                {
                    Orders.Add(o);
                }
            }
        }

        public void MarkAsCompleted(int orderId)
        {
            db.UpdateOrderStatus(orderId, "Завершён");
            var order = Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = "Завершён";
                // Пересортировываем список
                var sortedOrders = Orders
                    .OrderBy(o => o.Status == "Активный" ? 0 : 1)
                    .ThenByDescending(o => DateTime.Parse(o.OrderDate))
                    .ToList();
                Orders.Clear();
                foreach (var o in sortedOrders)
                {
                    Orders.Add(o);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}