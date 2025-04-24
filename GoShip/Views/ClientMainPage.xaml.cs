using System.Windows;
using System.Windows.Controls;
using GoShip.Models;
using GoShip.ViewModels;

namespace GoShip.Views
{
    public partial class ClientMainPage : Page
    {
        private readonly int userId;
        public ClientMainPage(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            DataContext = new ClientMainViewModel();
        }

        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ClientMainViewModel;
            var selectedProduct = productComboBox.SelectedItem as Product;
            string address = txtAddress.Text;

            if (selectedProduct == null || string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Выберите товар и укажите адрес доставки!");
                return;
            }

            viewModel.PlaceOrder(userId, selectedProduct.Id, address);
            MessageBox.Show("Заказ успешно оформлен!");
            txtAddress.Text = "";
        }
    }
}