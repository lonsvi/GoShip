using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GoShip.Models
{
    public class Order : INotifyPropertyChanged
    {
        private int _id;
        private int _userId;
        private string _address;
        private string _orderDate;
        private decimal _total;
        private string _status;
        private string _comment;
        private string _deliveryTime;
        private List<OrderItem> _items;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        public string OrderDate
        {
            get => _orderDate;
            set
            {
                _orderDate = value;
                OnPropertyChanged();
            }
        }

        public decimal Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged();
            }
        }

        public string DeliveryTime
        {
            get => _deliveryTime;
            set
            {
                _deliveryTime = value;
                OnPropertyChanged();
            }
        }

        public List<OrderItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}