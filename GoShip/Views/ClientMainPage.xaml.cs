using System.Windows;
using System.Windows.Controls;
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
            DataContext = viewModel;
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
                viewModel.AddToCart(productId);
                MessageBox.Show($"Товар с ID {productId} добавлен в корзину!");
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                Window.GetWindow(this)?.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }
    }
}