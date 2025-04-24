using System.Windows;
using System.Windows.Controls;
using GoShip.ViewModels;

namespace GoShip.Views
{
    public partial class CartPage : Page
    {
        private readonly int userId;
        private readonly CartViewModel viewModel;

        public CartPage(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            viewModel = new CartViewModel(userId);
            DataContext = viewModel;
        }

        private void Cart_Click(object sender, RoutedEventArgs e)
        {
            // Уже находимся в корзине
        }

        private void Catalog_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ClientMainPage(userId));
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Переход в поддержку (в разработке)");
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CheckoutPage(userId));
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is int orderId)
            {
                viewModel.RemoveFromCart(orderId);
                MessageBox.Show("Товар удалён из корзины!");
            }
        }
    }
}