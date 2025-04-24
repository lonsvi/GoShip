using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GoShip.Models;
using GoShip.ViewModels;

namespace GoShip.Views
{
    public partial class ClientMainPage : Page
    {
        private readonly int userId;
        private readonly ClientMainViewModel viewModel;

        public ClientMainPage(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            viewModel = new ClientMainViewModel(userId);
            DataContext = viewModel; // Устанавливаем DataContext
        }

        private void Cart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage(userId));
        }

        private void Catalog_Click(object sender, RoutedEventArgs e)
        {
            // Уже находимся в каталоге
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Переход в поддержку (в разработке)");
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is int productId)
            {
                viewModel.PlaceOrder(userId, productId, "В корзине");
                MessageBox.Show($"Товар с ID {productId} добавлен в корзину!");
            }
        }
    }
}